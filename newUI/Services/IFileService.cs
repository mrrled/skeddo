using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace newUI.Services;

public interface IFileService
{
    Task<IStorageFile?> SaveFileAsync(string title, string defaultExtension, string suggestedName);
}