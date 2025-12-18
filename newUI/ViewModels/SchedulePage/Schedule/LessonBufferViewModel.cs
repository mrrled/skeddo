using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Application.DtoExtensions;
using Application.DtoModels;
using Application.IServices;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.Shared;

namespace newUI.ViewModels.SchedulePage.Lessons;

public class LessonBufferViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly Guid scheduleId;

    public ObservableCollection<LessonCardViewModel> LessonCards { get; } = new();
    public event Action? RequestTableRefresh;
    public ICommand ClearCommand { get; }

    public LessonBufferViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.scheduleId = scheduleId;
        ClearCommand = new AsyncRelayCommand(ClearBufferAsync);
    }

    public void AddMany(IEnumerable<LessonDraftDto> drafts)
    {
        LessonCards.Clear();
        foreach (var draft in drafts)
        {
            var card = new LessonCardViewModel(scopeFactory, windowManager, async () => await RefreshAsync())
            {
                Lesson = draft.ToLessonDto(),
                IsVisible = true
            };
            LessonCards.Add(card);
        }

        RequestTableRefresh?.Invoke();
    }

    public async Task RefreshAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();
        var drafts = await service.GetLessonDraftsByScheduleId(scheduleId);
        AddMany(drafts);
    }

    private async Task ClearBufferAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();
        await service.ClearDraftsByScheduleId(scheduleId);
        await RefreshAsync();
    }
}