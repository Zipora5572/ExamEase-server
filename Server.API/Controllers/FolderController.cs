using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.API.PostModel;
using Server.Core.DTOs;
using Server.Core.Entities;
using Server.Core.IServices;
using Server.Service;
using System.Collections.Generic;

namespace Server.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService _folderService;
        private readonly IMapper _mapper;

        public FolderController(
          
            IFolderService folderService,
            IMapper mapper)
        {

            _folderService = folderService;
            _mapper = mapper;
        }

        // GET: api/<FolderController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Folder>>> Get()
        {
            List<Folder> folders = await _folderService.GetAllFoldersAsync();
            if (folders == null)
            {
                return NotFound();
            }
            return Ok(folders);
        }

        // GET api/<FolderController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FolderDto>> Get(int id)
        {
            FolderDto folderDto = await _folderService.GetByIdAsync(id);
            if (folderDto == null)
            {
                return NotFound();
            }
            return Ok(folderDto);
        }

        // POST api/<FolderController>
        [HttpPost]
        public async Task<ActionResult<FolderDto>> Post([FromBody] FolderPostModel folderPostModel)
        {
            if (folderPostModel == null)
            {
                return BadRequest("folder is require");
            }
            var folderDto = _mapper.Map<FolderDto>(folderPostModel);
            if (folderDto.ParentFolderId != null)
            {
                int id=(int)folderDto.ParentFolderId;
                var parentFolder = await _folderService.GetByIdAsync(id);
                
                folderDto.NamePrefix = $"{parentFolder.NamePrefix}/{folderDto.Name}";
            }
            else
                folderDto.NamePrefix =folderDto.Name;

            try
            {
                await _folderService.AddFolderAsync(folderDto);
                return Ok(folderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding file: {ex.Message}");
            }         
        }

        // PUT api/<FolderController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Folder>> Put(int id, [FromBody] FolderPostModel folderPostModel)
        {
            if (folderPostModel == null)
            {
                return BadRequest("Exam cannot be null");
            }
            FolderDto folderDto = _mapper.Map<FolderDto>(folderPostModel);

            folderDto =await _folderService.UpdateFolderAsync(id, folderDto);
            if (folderDto == null)
            {
                return NotFound();
            }
            return Ok(folderDto);
        }
        
        // PATCH api/<FolderController>/rename/5
        [HttpPatch("rename/{id}")]
        public async Task<ActionResult<ExamDto>> UpdateName(int id, [FromBody] string newName)
        {
            if (string.IsNullOrEmpty(newName))
            {
                return BadRequest("New name cannot be null or empty.");
            }

            var examDto = await _folderService.GetByIdAsync(id);
            if (examDto == null)
            {
                return NotFound();
            }
            string oldName = examDto.Name;
            examDto.Name = newName;
            var updatedExam = await _folderService.UpdateFolderAsync(id, examDto, oldName);

            return Ok(updatedExam);
        }
        // DELETE api/<FolderController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<int>> Delete(int id)
        {
            var folder = await _folderService.GetByIdAsync(id);
            try
            {
                await _folderService.DeleteFolderAsync(folder);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting folder: {ex.Message}");
            }

            return Ok(id);
        }
        [HttpPatch("{id}/toggle-star")]
        public async Task<IActionResult> ToggleStar(int id)
        {
          
            var updatedFolder = await _folderService.ToggleStarAsync(id);
            if (updatedFolder == null) return NotFound();
            return Ok(updatedFolder);
        }



    }
}