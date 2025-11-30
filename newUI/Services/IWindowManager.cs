using newUI.ViewModels;
using System.Threading.Tasks;

namespace newUI.Services;

public interface IWindowManager
{
    void Show<TViewModel>() where TViewModel : ViewModelBase;
    
    Task<TResult?> ShowDialog<TViewModel, TResult>(TViewModel viewModel)
        where TViewModel : ViewModelBase;
}