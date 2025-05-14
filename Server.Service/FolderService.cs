using AutoMapper;
using Google.Cloud.Storage.V1;
using Server.Core.DTOs;

using Server.Core.Entities;
using Server.Core.IRepositories;
using Server.Core.IServices;
using Server.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Service
{
    public class FolderService : IFolderService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly IExamService _examService;
        private readonly IStorageService _storageService;

        public FolderService(IRepositoryManager repositoryManager, IMapper mapper, IExamService examService, IStorageService storageService)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _examService = examService;
            _storageService = storageService;

        }

        public async Task<List<Folder>> GetAllFoldersAsync()
        {

            var Folders = await _repositoryManager.Folders.GetAllAsync();
            return Folders.ToList();
        }

        public async Task<FolderDto> GetByIdAsync(int id)
        {

            Folder Folder = await _repositoryManager.Folders.GetByIdAsync(id);
            FolderDto FolderDto = _mapper.Map<FolderDto>(Folder);
            return FolderDto;

        }

        public async Task<FolderDto> AddFolderAsync(FolderDto folderDto)
        {
            Folder folder = _mapper.Map<Folder>(folderDto);
            folder = await _repositoryManager.Folders.AddAsync(folder);
            await _repositoryManager.SaveAsync();
            folderDto = _mapper.Map<FolderDto>(folder);

            return folderDto;
        }


        public async Task DeleteFolderAsync(FolderDto folderDto)
        {
            var existingFolder = await _repositoryManager.Folders.GetByIdAsync(folderDto.Id);
            if (existingFolder != null)
            {
                var exams = _repositoryManager.Exams.GetAllExams().Where(e => e.FolderId == folderDto.Id).ToList();

                if (exams.Any())
                {
                    
                    foreach (var exam in exams)
                    {
                        await _examService.DeleteExamAsync(_mapper.Map<ExamDto>(exam));
                      
                    }
                }

                await _repositoryManager.Folders.DeleteAsync(existingFolder);
                await _repositoryManager.SaveAsync();
            }
            else
            {
                throw new InvalidOperationException("Folder not found.");
            }
        }



        public async Task<FolderDto> UpdateFolderAsync(int id, FolderDto folderDto, string oldName = "")
        {

            Folder folder = _mapper.Map<Folder>(folderDto);
            folder = await _repositoryManager.Folders.UpdateAsync(id, folder);
            await _repositoryManager.SaveAsync();

            if (oldName != "")
            {

                string oldPrefix = oldName + "/";
                string newPrefix = folderDto.Name + "/";
                await _storageService.RenameFolderAsync(oldPrefix, newPrefix);
                var examsToUpdate = _repositoryManager.Exams.GetAllExams().Where(e => e.FolderId == folderDto.Id).ToList();
                foreach (var exam in examsToUpdate)
                {
                    exam.NamePrefix = exam.NamePrefix.Replace(oldPrefix, newPrefix);
                    await _repositoryManager.Exams.UpdateAsync(exam.Id,exam);
                }
                await _repositoryManager.SaveAsync(); 
            }
            folderDto = _mapper.Map<FolderDto>(folder);
            return folderDto;
        }
        public async Task<FolderDto> ToggleStarAsync(int folderId)
        {
            var folder = await _repositoryManager.Folders.GetByIdAsync(folderId);
            if (folder == null) return null;

            folder.IsStarred = !folder.IsStarred;
            await _repositoryManager.SaveAsync();
             return _mapper.Map<FolderDto>(folder); 
        }
    }
}
