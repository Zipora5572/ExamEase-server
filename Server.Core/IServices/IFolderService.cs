using Server.Core.DTOs;
using Server.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Core.IServices
{
    public interface IFolderService
    {
        Task<List<Folder>> GetAllFoldersAsync();
        Task<FolderDto> GetByIdAsync(int id);
        Task<FolderDto> AddFolderAsync(FolderDto folder);
        Task DeleteFolderAsync(FolderDto folder);
        Task<FolderDto> UpdateFolderAsync(int id, FolderDto folder,string oldName="");
        Task<FolderDto> ToggleStarAsync(int folderId);

    }
}
