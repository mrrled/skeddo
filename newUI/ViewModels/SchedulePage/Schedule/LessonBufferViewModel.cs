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
    public event Action<LessonDto>? RequestEditLesson;

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
        if (drafts == null) return;

        foreach (var draft in drafts)
        {
            var card = new LessonCardViewModel(scopeFactory, windowManager, async () => await RefreshAsync())
            {
                Lesson = draft.ToLessonDto(),
                IsVisible = true
            };

            // 2. Подписываемся на клик по карточке в буфере
            // В LessonCardViewModel должен быть либо EditRequested, либо ClickCommand вызывающий событие
            card.EditRequested += (lesson) => 
            {
                RequestEditLesson?.Invoke(lesson);
            };

            // Оставляем ваши существующие подписки, если они используются для Drag-and-Drop или быстрых действий
            card.LessonUpdated += async _ =>
            {
                RequestTableRefresh?.Invoke();
                await RefreshAsync();
            };

            card.LessonDeleted += async _ =>
            {
                RequestTableRefresh?.Invoke();
                await RefreshAsync();
            };

            LessonCards.Add(card);
        }
    }

    public async Task RefreshAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();
        var drafts = await service.GetLessonDraftsByScheduleId(scheduleId);

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            AddMany(drafts);
            OnPropertyChanged(nameof(LessonCards));
        });
    }

    private async Task ClearBufferAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILessonDraftServices>();
        var result = await service.ClearDraftsByScheduleId(scheduleId);
        if (result.IsFailure)
        {
            await windowManager.ShowDialog<NotificationViewModel, object?>(
                new NotificationViewModel(result.Error));
            return;
        }
        await RefreshAsync();
    }
}