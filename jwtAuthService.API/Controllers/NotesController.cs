using System.Security.Claims;
using jwtAuthService.Application.DTOs;
using jwtAuthService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace jwtAuthService.API.Controllers
{
    [ApiController]
    [Route("api/notes")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        public NotesController(INoteService noteService)
        {
            _noteService = noteService;
        }
        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        [SwaggerOperation(Summary = "Create a note", Description = "Create a new note for the authenticated user.")]
        [ProducesResponseType(typeof(NoteResponse), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateNoteRequest request)
        {
            var note = await _noteService.CreateNoteAsync(request, GetUserId());
            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all notes", Description = "Get all notes for the authenticated user.")]
        [ProducesResponseType(typeof(List<NoteResponse>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var notes = await _noteService.GetAllNotesAsync(GetUserId());
            return Ok(notes);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get note by ID", Description = "Get a single note by ID for the authenticated user.")]
        [ProducesResponseType(typeof(NoteResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var note = await _noteService.GetNoteByIdAsync(id, GetUserId());
            if (note == null) return NotFound();
            return Ok(note);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a note", Description = "Update a note for the authenticated user.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateNoteRequest request)
        {
            var updated = await _noteService.UpdateNoteAsync(id, request, GetUserId());
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a note", Description = "Delete a note for the authenticated user.")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _noteService.DeleteNoteAsync(id, GetUserId());
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
