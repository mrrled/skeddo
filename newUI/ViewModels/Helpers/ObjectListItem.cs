using Application.Services;
using Avalonia.Collections;

namespace newUI.ViewModels.Helpers;

public interface IObjectListItem<TSelf> where TSelf : IObjectListItem<TSelf>
{
    public static abstract AvaloniaList<TSelf> FetchFromBackend(IService service);
}