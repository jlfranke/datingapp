using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace API.Data;

public class LikesRepository(AppDbContext context) : ILikesRepository
{
    public async Task<MemberLike?> GetMemberLike(string SourceMemberId, string TargetMemberId)
    {
        return await context.Likes.FindAsync(SourceMemberId, TargetMemberId);
    }

    public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likesParams)
    {
        var query = context.Likes.AsQueryable();
        IQueryable<Member> result;

        switch(likesParams.Predicate)
        {
            case "liked":
                result = query.Where(like => like.SourceMemberId == likesParams.MemberId).Select(like => like.TargetMember);
                break;
            case "likedBy":
                result = query.Where(like => like.TargetMemberId == likesParams.MemberId).Select(like => like.SourceMember);
                break;
            default: //mutual
                var likeIds = await GetCurrentMemberLikeIds(likesParams.MemberId);
                result = query.Where(x => x.TargetMemberId == likesParams.MemberId && likeIds.Contains(x.SourceMemberId)).Select(x => x.SourceMember);
                break;
        }

        return await PaginationHelper.CreateAsync(result, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
    {
        return await context.Likes
            .Where(like => like.SourceMemberId == memberId)
            .Select(like => like.TargetMemberId)
            .ToListAsync();
    }

    public void DeleteLike(MemberLike like)
    {
        context.Likes.Remove(like);
    }

    public void AddLike(MemberLike like)
    {
        context.Likes.Add(like);
    }

    public async Task<bool> SaveAllChanges()
    {
        return await context.SaveChangesAsync() > 0;
    }
}