using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Domain.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public class ExportDocument : IDocument
{
    private readonly Dictionary<(int num, Guid groupId, string? subgroupName), Lesson> _lessons;
    private readonly List<StudyGroup> _cols;
    private readonly List<LessonNumber> _rows;
    private readonly int _totalColumn;
    private readonly List<ColumnMap> _flatColumns;

    public ExportDocument(
        Dictionary<(int num, Guid groupId, string? subgroupName), Lesson> lessons,
        List<StudyGroup> studyGroups,
        List<LessonNumber> lessonNumbers
    )
    {
        _lessons = lessons;
        _rows = lessonNumbers.OrderBy(x => x.Number).ToList();
        _cols = studyGroups
            .OrderBy(g => Regex.Replace(g.Name, @"\d+", m => m.Value.PadLeft(10, '0')))
            .ToList();
        _totalColumn = _cols.Sum(c => c.StudySubgroups.Any() ? c.StudySubgroups.Count : 1);
        _flatColumns = new List<ColumnMap>();
        foreach (var group in _cols)
        {
            if (group.StudySubgroups.Any())
            {
                foreach (var sub in group.StudySubgroups)
                {
                    _flatColumns.Add(new ColumnMap
                    {
                        GroupId = group.Id,
                        SubgroupName = sub.Name
                    });
                }
            }
            else
            {
                _flatColumns.Add(new ColumnMap
                {
                    GroupId = group.Id,
                    SubgroupName = null
                });
            }
        }
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(20);
            page.Content().Element(ComposeTable);
        });
    }

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50);
                for (var i = 0; i < _totalColumn; i++)
                    columns.RelativeColumn();
            });

            table.Cell().RowSpan(2).Column(1).Element(HeaderStyle).Text("");
            foreach (var group in _cols)
            {
                var subCount = group.StudySubgroups.Count;

                if (subCount > 0)
                {
                    table.Cell()
                        .ColumnSpan((uint)subCount)
                        .Element(HeaderStyle)
                        .Text(group.Name);
                }
                else
                {
                    table.Cell()
                        .RowSpan(2)
                        .Element(HeaderStyle)
                        .Text(group.Name);
                }
            }

            foreach (var group in _cols)
            {
                if (group.StudySubgroups.Count > 0)
                {
                    foreach (var sub in group.StudySubgroups)
                    {
                        table.Cell()
                            .Element(HeaderStyle)
                            .Text(sub.Name);
                    }
                }
            }

            foreach (var lessonNumber in _rows)
            {
                table.Cell().Element(HeaderStyle).Text(lessonNumber.Number.ToString());
                for (var i = 0; i < _flatColumns.Count;)
                {
                    var currentKey = (lessonNumber.Number, _flatColumns[i].GroupId, _flatColumns[i].SubgroupName);
                    _lessons.TryGetValue(currentKey, out var currentLesson);
                    uint span = 1;
                    while (i + (int)span < _flatColumns.Count)
                    {
                        var nextIndex = i + (int)span;
                        var nextCol = _flatColumns[nextIndex];
                        if (nextCol.GroupId != _flatColumns[i].GroupId)
                            break;
                        var nextKey = (lessonNumber.Number, nextCol.GroupId, nextCol.SubgroupName);
                        _lessons.TryGetValue(nextKey, out var nextLesson);
                        if (!IsSameLesson(currentLesson, nextLesson))
                            break;
                        span++;
                    }

                    table.Cell().ColumnSpan(span).Element(CellStyle).Text(
                        currentLesson != null
                            ? SchoolFormat(currentLesson.Classroom, currentLesson.SchoolSubject, currentLesson.Teacher)
                            : "-");
                    i += (int)span;
                }
            }
        });
    }

    public void CreateExcelReport(Stream fileStream)
    {
        using (var workbook = new XLWorkbook())
        {
            var ws = workbook.Worksheets.Add("Расписание");
            var currentExcelCol = 2;
            ws.Cell(1, 1).Value = "#";
            ws.Range(1, 1, 2, 1).Merge();
            foreach (var group in _cols)
            {
                if (group.StudySubgroups.Count != 0)
                {
                    var subCount = group.StudySubgroups.Count;
                    var groupHeaderRange = ws.Range(1, currentExcelCol, 1, currentExcelCol + subCount - 1);
                    groupHeaderRange.Merge().Value = group.Name;
                    groupHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    groupHeaderRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    foreach (var sub in group.StudySubgroups)
                    {
                        ws.Cell(2, currentExcelCol).Value = sub.Name;
                        ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        currentExcelCol++;
                    }
                }
                else
                {
                    var range = ws.Range(1, currentExcelCol, 2, currentExcelCol);
                    range.Merge().Value = group.Name;
                    range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    currentExcelCol++;
                }
            }

            var startRow = 3;
            foreach (var lessonNumber in _rows)
            {
                ws.Cell(startRow + lessonNumber.Number - 1, 1).Value = lessonNumber.Number;
                var current = 2;
                for (var i = 0; i < _flatColumns.Count;)
                {
                    var currentKey = (lessonNumber.Number, _flatColumns[i].GroupId, _flatColumns[i].SubgroupName);
                    _lessons.TryGetValue(currentKey, out var currentLesson);
                    var span = 1;
                    while (i + span < _flatColumns.Count)
                    {
                        var nextIndex = i + span;
                        if (_flatColumns[nextIndex].GroupId != _flatColumns[i].GroupId)
                            break;
                        var nextKey = (lessonNumber.Number, _flatColumns[nextIndex].GroupId,
                            _flatColumns[nextIndex].SubgroupName);
                        _lessons.TryGetValue(nextKey, out var nextLesson);
                        if (!IsSameLesson(currentLesson, nextLesson))
                            break;
                        span++;
                    }

                    var currentRow = startRow + (lessonNumber.Number - 1);
                    if (span > 1)
                    {
                        var range = ws.Range(currentRow, current, currentRow, current + span - 1);
                        range.Merge();
                        if (currentLesson != null)
                        {
                            range.Value = SchoolFormat(currentLesson.Classroom, currentLesson.SchoolSubject,
                                currentLesson.Teacher);
                            range.Style.Alignment.WrapText = true;
                            range.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            range.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        }
                    }
                    else
                    {
                        if (currentLesson != null)
                        {
                            ws.Cell(currentRow, current).Value = SchoolFormat(currentLesson.Classroom,
                                currentLesson.SchoolSubject, currentLesson.Teacher);
                            ws.Cell(currentRow, current).Style.Alignment.WrapText = true;
                            ws.Cell(currentRow, current).Style.Alignment.Horizontal =
                                XLAlignmentHorizontalValues.Center;
                            ws.Cell(currentRow, current).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        }
                    }

                    i += span;
                    current += span;
                }
            }

            ws.Columns(2, currentExcelCol).Width = 25; 
            for (var r = startRow; r <= startRow + _rows.Count; r++)
            {
                var row = ws.Row(r);
                double maxPixelHeight = 0;
                foreach (var cell in row.CellsUsed())
                {
                    var text = cell.GetString();
                    if (string.IsNullOrEmpty(text))
                        continue;
                    var linesCount = text.Split('\n').Length;
                    var extraLines = 0;
                    foreach (var line in text.Split('\n'))
                    {
                        if (line.Length > 25)
                            extraLines += line.Length / 25;
                    }
                    linesCount += extraLines;
                    double neededHeight = linesCount * 18 + 10;
                    if (neededHeight > maxPixelHeight)
                        maxPixelHeight = neededHeight;
                }
                if (maxPixelHeight > 0)
                    row.Height = Math.Max(maxPixelHeight, 15); 
            }
            var rng = ws.RangeUsed();
            rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.Style.Fill.BackgroundColor = XLColor.White;
            ws.Columns().AdjustToContents();
            workbook.SaveAs(fileStream);
        }
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter();
    }

    private static IContainer HeaderStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Medium)
            .Background(Colors.Grey.Lighten3)
            .Padding(5)
            .AlignMiddle()
            .AlignCenter()
            .DefaultTextStyle(x => x.Bold());
    }

    private string ToTeacherFormat(Teacher? teacher)
    {
        return teacher is null
            ? ""
            : $"{teacher.Surname} {teacher.Name.FirstOrDefault()}." 
              + (string.IsNullOrEmpty(teacher.Patronymic) ? "" : $"{teacher.Patronymic.First()}.");
    }

    private string SchoolFormat(Classroom? classroom, SchoolSubject subject, Teacher? teacher)
    {
        return $"{subject.Name}\n{classroom?.Name}\n{ToTeacherFormat(teacher)}";
    }

    private string UniversityFormat(Classroom? classroom, SchoolSubject subject, Teacher? teacher)
    {
        return $"{subject.Name}, {classroom?.Name}, {ToTeacherFormat(teacher)}";
    }

    bool IsSameLesson(Lesson? a, Lesson? b)
    {
        if (a is null && b is null) return true;
        if (a is null || b is null) return false;
        return a.SchoolSubject == b.SchoolSubject &&
               a.Classroom == b.Classroom &&
               a.Teacher == b.Teacher;
    }
}

public class ColumnMap
{
    public Guid GroupId { get; set; }
    public string? SubgroupName { get; set; }
}