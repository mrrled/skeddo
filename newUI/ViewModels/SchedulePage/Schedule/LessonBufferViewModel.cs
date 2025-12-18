using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Application.DtoModels;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using newUI.Services;
using newUI.ViewModels.SchedulePage.Lessons;

namespace newUI.ViewModels.SchedulePage.Schedule;

public class LessonBufferViewModel : ViewModelBase
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly IWindowManager windowManager;
    private readonly Guid scheduleId;

    // ObservableCollection обеспечивает мгновенное исчезновение карточки из UI
    public ObservableCollection<LessonCardViewModel> LessonCards { get; } = new();

    public event Action? RequestTableRefresh;

    public LessonBufferViewModel(IServiceScopeFactory scopeFactory, IWindowManager windowManager, Guid scheduleId)
    {
        this.scopeFactory = scopeFactory;
        this.windowManager = windowManager;
        this.scheduleId = scheduleId;
        ClearCommand = new RelayCommand(Clear);
    }

    public IRelayCommand ClearCommand { get; }

    public void Clear()
    {
        LessonCards.Clear();
        // В продакшене здесь также можно вызвать сервис для удаления черновиков из БД, если нужно
    }

    public void AddMany(IEnumerable<LessonDraftDto> drafts)
    {
        foreach (var draft in drafts)
        {
            if (LessonCards.Any(c => c.Lesson.Id == draft.Id)) continue;

            // Создаем карточку. refreshCallback здесь не нужен, так как мы подписываемся на LessonUpdated
            var card = new LessonCardViewModel(scopeFactory, windowManager);
            var lessonDto = draft.ToLessonDto();
            lessonDto.ScheduleId = scheduleId;
            card.Lesson = lessonDto;
            card.IsVisible = true; // Возвращаем свойство

            // Когда черновик в этой карточке заполнен и сохранен как полноценный урок
            card.LessonUpdated += (updatedLesson) =>
            {
                RemoveCard(card);
                // Сигнализируем ScheduleViewModel, что нужно обновить таблицу
                RequestTableRefresh?.Invoke();
            };

            LessonCards.Add(card);
        }
    }

    private void RemoveCard(LessonCardViewModel card)
    {
        // Выполняем в UI-потоке, чтобы избежать исключений при асинхронном сохранении
        Avalonia.Threading.Dispatcher.UIThread.Post(() => { LessonCards.Remove(card); });
    }
}