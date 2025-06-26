using Mapster;
using jwtAuthService.Application.DTOs;
using jwtAuthService.Application.Interfaces;
using jwtAuthService.Domain.Model;

namespace jwtAuthService.Application.Implementations;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }
    public async Task<NoteResponse> CreateNoteAsync(CreateNoteRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var note = new Note
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = userId
        };
        await _noteRepository.AddAsync(note, cancellationToken);
        return note.Adapt<NoteResponse>();
    }
    public async Task<NoteResponse?> GetNoteByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId, cancellationToken);
        return note?.Adapt<NoteResponse>();
    }
    public async Task<List<NoteResponse>> GetAllNotesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notes = await _noteRepository.GetAllByUserAsync(userId, cancellationToken);
        return notes.Adapt<List<NoteResponse>>();
    }
    public async Task<bool> UpdateNoteAsync(Guid id, UpdateNoteRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId, cancellationToken);
        if (note == null) return false;
        note.Title = request.Title;
        note.Content = request.Content;
        note.UpdatedAt = DateTime.UtcNow;
        await _noteRepository.UpdateAsync(note, cancellationToken);
        return true;
    }
    public async Task<bool> DeleteNoteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId, cancellationToken);
        if (note == null) return false;
        await _noteRepository.DeleteAsync(id, userId, cancellationToken);
        return true;
    }
}
