using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int SourceUserID, int TargetUserId)
        {
            return await _context.likes.FindAsync(SourceUserID, TargetUserId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.likes.AsQueryable();

            if(likesParams.predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserID == likesParams.UserId);
                users = likes.Select(like => like.TargetUser);
            }

            if(likesParams.predicate == "likedBy")
            {
                likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }

            var likesdUsers = users.Select(user => new LikeDto {
                UserName = user.UserName,
                KnownAs = user.KnownAs, 
                Age = user.DateOfBirth.CalculateAge(),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                City = user.City, 
                Id = user.Id});
            
            return await PagedList<LikeDto>.CreateAsync(likesdUsers, likesParams.PageNumber, likesParams.pageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userID)
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userID);
        }
    }
}