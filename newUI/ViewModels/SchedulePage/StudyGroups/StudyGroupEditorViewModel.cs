using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoModels;
using Application.IServices;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.StudyGroups;

public class StudyGroupEditorViewModel : ViewModelBase
{
    public string HeaderText { get; }
    public event Action<StudyGroupDto>? StudyGroupSaved;
    public event Action<StudyGroupDto>? StudyGroupDeleted;

    private string studyGroupName = string.Empty;
    private AvaloniaList<StudySubgroupDto> subgroups = new();
    
    private List<StudySubgroupDto> subgroupsAdded = new();
    private List<StudySubgroupDto> subgroupsRemoved = new();
    
    public string StudyGroupName
    {
        get => studyGroupName;
        set => SetProperty(ref studyGroupName, value);
    }

    public AvaloniaList<StudySubgroupDto> Subgroups
    {
        get => subgroups;
        set => SetProperty(ref subgroups, value);
    }
    public bool IsEditMode => editingStudyGroup != null;

    private readonly IWindowManager windowManager;
    private readonly IServiceScopeFactory scopeFactory;
    private readonly StudyGroupDto? editingStudyGroup;
    private readonly Guid scheduleId;

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand AddSubgroupCommand { get; }
    public ICommand DeleteSubgroupCommand { get; }


    // ================== СОЗДАНИЕ ==================
    public StudyGroupEditorViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory, Guid scheduleId)
    {
        this.windowManager = windowManager;
        this.scopeFactory = scopeFactory;
        this.scheduleId = scheduleId;

        SaveCommand = new RelayCommandAsync(SaveAsync);
        HeaderText = "Добавление учебной группы";
        AddSubgroupCommand = new RelayCommandAsync(AddSubgroupAsync);
        DeleteSubgroupCommand = new AsyncRelayCommand<StudySubgroupDto>(DeleteSubgroup);
    }

    // ================== РЕДАКТИРОВАНИЕ ==================
    public StudyGroupEditorViewModel(IWindowManager windowManager, IServiceScopeFactory scopeFactory, StudyGroupDto studyGroupToEdit, Guid scheduleId)
        : this(windowManager, scopeFactory, scheduleId)
    {
        editingStudyGroup = studyGroupToEdit;
        StudyGroupName = studyGroupToEdit.Name;
        subgroups = new AvaloniaList<StudySubgroupDto>(editingStudyGroup.StudySubgroups);

        DeleteCommand = new RelayCommandAsync(DeleteAsync);
        HeaderText = "Редактирование учебной группы";
    }
    
    // ================== ДОБАВЛЕНИЕ ПОДГРУППЫ ==================
    private async Task AddSubgroupAsync()
    {
        var inputVm = new TextInputViewModel("Добавление подгруппы", 
            "Введите название подгруппы:", "А");
        
        var result = await windowManager.ShowDialog<TextInputViewModel, string?>(inputVm);
        
        if (!string.IsNullOrWhiteSpace(result))
        {
            if (Subgroups.Any(s => s.Name.Equals(result, StringComparison.OrdinalIgnoreCase)))
            {
                windowManager.ShowWindow(new NotificationViewModel($"Ошибка. Подгруппа с именем '{result}' уже существует."));
                return;
            }
            
            if(editingStudyGroup is null)
                return;
            
            var newSubgroup = new StudySubgroupDto
            {
                Name = result.Trim(),
                StudyGroup = editingStudyGroup
            };
            
            subgroupsAdded.Add(newSubgroup);
            Subgroups.Add(newSubgroup);
            OnPropertyChanged(nameof(Subgroups));
        }
    }
    
    // ================== УДАЛЕНИЕ ПОДГРУППЫ ==================
    private async Task DeleteSubgroup(StudySubgroupDto? subgroup)
    {
        editingStudyGroup.UpdateSubgroups();
        if (subgroup == null) return;
        
        var confirmVm = new ConfirmDeleteViewModel(
            $"Удалить подгруппу \"{subgroup.Name}\"?");
        var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

        if (result != true)
            return;
        
        subgroupsRemoved.Add(subgroup);
        Subgroups.Remove(subgroup);
        OnPropertyChanged(nameof(Subgroups));
    }


    // ================== SAVE ==================
    private async Task SaveAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();

        StudyGroupDto studyGroup;

        if (editingStudyGroup == null)
        {
            var createStudyGroup = new CreateStudyGroupDto { Name = StudyGroupName, ScheduleId = scheduleId };
            var studyGroupResult = await service.AddStudyGroup(createStudyGroup);
            if (studyGroupResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(studyGroupResult.Error));
                return;
            }
            studyGroup = studyGroupResult.Value;
        }
        else
        {
            studyGroup = new StudyGroupDto
            {
                Id = editingStudyGroup.Id,
                Name = StudyGroupName
            };

            var studyGroupEditResult = await service.EditStudyGroup(studyGroup);
            if (studyGroupEditResult.IsFailure)
            {
                await windowManager.ShowDialog<NotificationViewModel, object?>(
                    new NotificationViewModel(studyGroupEditResult.Error));
                return;
            }
            
            var subgroupService = scope.ServiceProvider.GetRequiredService<IStudySubgroupService>();
            foreach (var subgroup in subgroupsAdded)
            {
                await subgroupService.AddStudySubgroup(subgroup);
            }
            foreach (var subgroup in subgroupsRemoved)
            {
                await subgroupService.DeleteStudySubgroup(subgroup);
            }
        }

        StudyGroupSaved?.Invoke(studyGroup);
        Window?.Close();
    }

    // ================== DELETE ==================
    private async Task DeleteAsync()
    {
        
        if (editingStudyGroup == null)
            return;

        var confirmVm = new ConfirmDeleteViewModel(
            $"Вы уверены, что хотите удалить учебную группу \"{editingStudyGroup.Name}\"?"
        );

        var result = await windowManager.ShowDialog<ConfirmDeleteViewModel, bool?>(confirmVm);

        if (result != true)
            return;

        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IStudyGroupServices>();

        await service.DeleteStudyGroup(editingStudyGroup);

        StudyGroupDeleted?.Invoke(editingStudyGroup);
        Window?.Close();
    }
}
