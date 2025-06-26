using jwtAuthService.Application.Interfaces;
using jwtAuthService.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace jwtAuthService.Infrastructure.Persistence
{
    public class NoteRepository : INoteRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public NoteRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Note> AddAsync(Note note, CancellationToken cancellationToken = default)
        {
            _dbContext.Notes.Add(note);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return note;
        }
        public async Task<Note?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);
        }
        public async Task<List<Note>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Notes.Where(n => n.UserId == userId).ToListAsync(cancellationToken);
        }
        public async Task UpdateAsync(Note note, CancellationToken cancellationToken = default)
        {
            _dbContext.Notes.Update(note);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, cancellationToken);
            if (note != null)
            {
                _dbContext.Notes.Remove(note);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
