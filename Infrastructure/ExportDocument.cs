using ClosedXML.Excel;
using Domain.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public class ExportDocument : IDocument
{
    private readonly Dictionary<LessonNumber, Dictionary<StudyGroup, List<Lesson>>> _lessons;
    private readonly List<StudyGroup> _cols;
    private readonly List<LessonNumber> _rows;

    public ExportDocument(
        Dictionary<LessonNumber, Dictionary<StudyGroup, List<Lesson>>> lessons,
        List<StudyGroup> studyGroups,
        List<LessonNumber> lessonNumbers
    )
    {
        _lessons = lessons;
        _rows = lessonNumbers.OrderBy(x => x.Number).ToList();
        _cols = studyGroups.OrderBy(x => x.Name).ToList();
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
                for (var i = 0; i <= _cols.Count; i++)
                    columns.RelativeColumn();
            });
            
            table.Cell().Row(1).Column(1).Element(HeaderStyle).Text("");
            for (var k = 2; k <= _cols.Count + 1; k++)
            {
                table.Cell().Row(1).Column((uint)k).Element(HeaderStyle).Text($"{_cols[k - 2].Name}");
            }

            for (var i = 2; i <= _rows.Count + 1; i++)
            {
                table.Cell().Row((uint)i).Column(1).Element(HeaderStyle).Text($"{_rows[i - 2].Number}");
                for (var j = 2; j <= _cols.Count + 1; j++)
                {
                    if (_lessons.TryGetValue(_rows[i - 2], out var row) && row.TryGetValue(_cols[j - 2], out var lesson))
                    {
                        table.Cell()
                            .Row((uint)i)
                            .Column((uint)j)
                            .Element(CellStyle)
                            .Text(SchoolFormat(lesson.First().Classroom, lesson.First().SchoolSubject, lesson.First().Teacher));
                        continue;
                    }

                    table.Cell()
                        .Row((uint)i)
                        .Column((uint)j)
                        .Element(CellStyle)
                        .Text("");
                }
            }
        });
    }

    public void CreateExcelReport(string path)
    {
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Расписание");
            for (var k = 2; k <= _cols.Count + 1; k++)
            {
                worksheet.Cell(1, k).Value = _cols[k - 2].Name;
                worksheet.Cell(1, k).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(1, k).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            for (var i = 2; i <= _rows.Count + 1; i++)
            {
                worksheet.Cell(i, 1).Value = _rows[i - 2].Number;
                worksheet.Cell(i, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Cell(i, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                for (var j = 2; j <= _cols.Count + 1; j++)
                {
                    if (_lessons.TryGetValue(_rows[i - 2], out var row) && row.TryGetValue(_cols[j - 2], out var lesson))
                    {
                        worksheet.Cell(i, j).Value = SchoolFormat(lesson.First().Classroom, lesson.First().SchoolSubject, lesson.First().Teacher);
                        worksheet.Cell(i, j).Style.Alignment.WrapText = true;
                        worksheet.Cell(i, j).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        worksheet.Cell(i, j).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    }
                }
            }

            var rng = worksheet.RangeUsed();
            rng.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rng.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rng.Style.Fill.BackgroundColor = XLColor.White;
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(path);
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

    public DocumentMetadata GetMetadata() =>
        DocumentMetadata.Default; //еще нужно сделать несколько уроков в одной ячейке

    private string ToTeacherFormat(Teacher? teacher)
    {
        return teacher is null
            ? ""
            : $"{teacher.Surname} {teacher.Name.FirstOrDefault()}.{teacher.Patronymic.FirstOrDefault()}.";
    }

    private string SchoolFormat(Classroom? classroom, SchoolSubject subject, Teacher? teacher)
    {
        return $"{subject.Name}\n{classroom?.Name}\n{ToTeacherFormat(teacher)}";
    }

    private string UniversityFormat(Classroom? classroom, SchoolSubject subject, Teacher? teacher)
    {
        return $"{subject.Name}, {classroom?.Name}, {ToTeacherFormat(teacher)}";
    }
}