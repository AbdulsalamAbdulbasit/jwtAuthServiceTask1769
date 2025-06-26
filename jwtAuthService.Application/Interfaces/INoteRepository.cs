using jwtAuthService.Domain.Model;

namespace jwtAuthService.Application.Interfaces
{
    public interface INoteRepository
    {
        Task<Note> AddAsync(Note note, CancellationToken cancellationToken = default);
        Task<Note?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<List<Note>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Note note, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
