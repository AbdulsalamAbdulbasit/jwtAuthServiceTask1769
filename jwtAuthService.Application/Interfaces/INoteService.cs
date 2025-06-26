using jwtAuthService.Application.DTOs;

namespace jwtAuthService.Application.Interfaces
{
    public interface INoteService
    {
        Task<NoteResponse> CreateNoteAsync(CreateNoteRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<NoteResponse?> GetNoteByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<List<NoteResponse>> GetAllNotesAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> UpdateNoteAsync(Guid id, UpdateNoteRequest request, Guid userId, CancellationToken cancellationToken = default);
        Task<bool> DeleteNoteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
