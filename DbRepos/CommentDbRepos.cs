using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class CommentDbRepos
{
    private ILogger<CommentDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CommentDbRepos(ILogger<CommentDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponsePageDto<IComment>> ReadCommentsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        IQueryable<DbComment> query;

        if (flat)
        {
            query = _dbContext.Comment.AsNoTracking();
        }
        else
        {
            query = _dbContext.Comment.AsNoTracking()
                .Include(i => i.DbReview)
                .ThenInclude(r => r.DbAttraction);
        }

        var ret = new ResponsePageDto<IComment>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query.CountAsync(),
            PageItems = await query.ToListAsync<IComment>(),
        };
        return ret;
    }

    public async Task<ResponseItemDto<IComment>> ReadCommentAsync(Guid id, bool flat)
    {
        IQueryable<DbComment> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Comment.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Comment.AsNoTracking()
                .Include(i => i.DbReview)
                .ThenInclude(r => r.DbAttraction);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.CommentId == id);

        var ret = new ResponseItemDto<IComment>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }

    public async Task<ResponseItemDto<IComment>> DeleteCommentAsync(Guid id)
    {
        var item = await _dbContext.Comment
            .Include(c => c.DbReview)
            .FirstOrDefaultAsync(c => c.CommentId == id);

        if (item == null)
            throw new ArgumentException($"Comment id {id} not existing");

        if (item.DbReview != null)
            item.DbReview.DbComment = null;

        _dbContext.Comment.Remove(item);

        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IComment> { Item = item };
    }

    public async Task<ResponseItemDto<IComment>> CreateCommentAsync(CommentCuDto itemDto)
    {
        if (itemDto.CommentId != null)
            throw new ArgumentException($"{nameof(itemDto.CommentId)} must be null when creating a new object");

        if (itemDto.ReviewId == null)
            throw new ArgumentException($"{nameof(itemDto.ReviewId)} is required when creating a comment");

        var item = new DbComment(itemDto);

        await navProp_CommentCUdto_to_CommentDbM(itemDto, item);

        _dbContext.Comment.Add(item);
        await _dbContext.SaveChangesAsync();

        return await ReadCommentAsync(item.CommentId, false);
    }

    private async Task navProp_CommentCUdto_to_CommentDbM(CommentCuDto itemDtoSrc, DbComment itemDst)
    {
        // Behåll nuvarande review om ReviewId inte skickas med (samma skydd som för AddressId)
        if (itemDtoSrc.ReviewId == null) return;

        // Samma review som idag, inget att göra
        if (itemDst.DbReview?.ReviewId == itemDtoSrc.ReviewId) return;

        var review = await _dbContext.Review
            .Include(r => r.DbComment)
            .FirstOrDefaultAsync(r => r.ReviewId == itemDtoSrc.ReviewId);

        if (review == null)
            throw new ArgumentException($"Review id {itemDtoSrc.ReviewId} not existing");

        // En review kan bara ha en kommentar
        if (review.DbComment != null && review.DbComment.CommentId != itemDst.CommentId)
            throw new ArgumentException($"Review id {itemDtoSrc.ReviewId} already has a comment");

        // Vid flytt vid Update: släpp den gamla kopplingen först
        if (itemDst.DbReview != null)
            itemDst.DbReview.DbComment = null;

        // FK:n ligger på Review, så sätt den sidan
        review.DbComment = itemDst;
    }


    public async Task<ResponseItemDto<IComment>> UpdateCommentAsync(CommentCuDto itemDto)
    {
        var item = await _dbContext.Comment
            .Include(c => c.DbReview)
            .FirstOrDefaultAsync(c => c.CommentId == itemDto.CommentId);

        if (item == null)
            throw new ArgumentException($"Comment id {itemDto.CommentId} not existing");

        item.UpdateFromDTO(itemDto);

        await navProp_CommentCUdto_to_CommentDbM(itemDto, item);

        _dbContext.Comment.Update(item);
        await _dbContext.SaveChangesAsync();

        return await ReadCommentAsync(item.CommentId, false);
    }

}
