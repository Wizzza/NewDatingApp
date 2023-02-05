using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int SourceUserID, int TargetUserId);
        Task<AppUser> GetUserWithLikes(int userID);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}