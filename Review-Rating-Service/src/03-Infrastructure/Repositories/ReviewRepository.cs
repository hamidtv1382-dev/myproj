using Microsoft.EntityFrameworkCore;
using Review_Rating_Service.src._01_Domain.Core.Aggregates.Review;
using Review_Rating_Service.src._01_Domain.Core.Interfaces.Repositories;
using Review_Rating_Service.src._03_Infrastructure.Data;

namespace Review_Rating_Service.src._03_Infrastructure.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review?> GetByIdAsync(Guid id)
        {
            return await _context.Reviews
                .Include(r => r.Ratings)
                .Include(r => r.Comments)
                .Include(r => r.Attachments)
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(Guid productId)
        {
            return await _context.Reviews
                .Include(r => r.Ratings)
                .Where(r => r.ProductId == productId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Reviews
                .Include(r => r.Ratings)
                .Where(r => r.UserId == userId && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<Review?> GetByUserAndProductAsync(Guid userId, Guid productId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId && !r.IsDeleted);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Review review)
        {
            review.MarkAsDeleted();
            _context.Reviews.Update(review);
            await Task.CompletedTask;
        }
    }
}
