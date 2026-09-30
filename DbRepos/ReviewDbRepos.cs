using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using DbModels;
using DbContext;
using Models.DTO;

namespace DbRepos;

public class ReviewDbRepos
{
    private ILogger<ReviewDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public ReviewDbRepos(ILogger<ReviewDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    #region Read all items
    public async Task<ResponsePageDto<IReview>> ReadReviewsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<DbReview> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Review.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Review.AsNoTracking()
                .Include(i => i.DbAttraction)
                .Include(i => i.DbUser)
                .Include(i => i.DbComment);
        }
        var ret = new ResponsePageDto<IReview>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.DbAttraction.Name.ToLower().Contains(filter) ||
                            i.DbUser.UserName.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) &&
                        (i.DbAttraction.Name.ToLower().Contains(filter) ||
                            i.DbUser.UserName.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IReview>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }
    #endregion

    #region Read item
    public async Task<ResponseItemDto<IReview>> ReadReviewAsync(Guid id, bool flat)
    {
        IQueryable<DbReview> query;

        if (flat)
        {
            // Create query without navigation properties
            query = _dbContext.Review.AsNoTracking();
        }
        else
        {
            // Create query with all navigation properties included
            query = _dbContext.Review.AsNoTracking()
                .Include(i => i.DbAttraction)
                .Include(i => i.DbUser)
                .Include(i => i.DbComment);
        }

        // Find the C by ID and return
        var C = await query.FirstOrDefaultAsync(f => f.ReviewId == id);

        var ret = new ResponseItemDto<IReview>()
        {
#if DEBUG
            ConnectionString = _dbContext.dbConnection,
#endif
            Item = C
        };
        return ret;
    }
    #endregion

    #region Delete item
    public async Task<ResponseItemDto<IReview>> DeleteReviewAsync(Guid id)
    {

        var item = await _dbContext.Review
            .Include(r => r.DbComment)
            .FirstOrDefaultAsync(r => r.ReviewId == id);

        if (item == null)
            throw new ArgumentException($"Review id {id} not existing");


        if (item.DbComment != null)
            _dbContext.Comment.Remove(item.DbComment);

        _dbContext.Review.Remove(item);


        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IReview> { Item = item };
    }
    #endregion

    #region Create item
    public async Task<ResponseItemDto<IReview>> CreateReviewAsync(ReviewCuDto itemDto)
    {
        // 1. Validate that ReviewId is null
        if (itemDto.ReviewId != null)
            throw new ArgumentException($"{nameof(itemDto.ReviewId)} must be null when creating a new object");

        // 2. Create new database entity from DTO
        var item = new DbReview(itemDto);

        // 3. Update navigation properties
        await navProp_ReviewCUdto_to_ReviewDbM(itemDto, item);

        // 4. Add to context and save
        _dbContext.Review.Add(item);
        await _dbContext.SaveChangesAsync();

        // 5. Return fully populated item
        return await ReadReviewAsync(item.ReviewId, false);
    }
    #endregion

    #region Navigation property CuDto
    private async Task navProp_ReviewCUdto_to_ReviewDbM(ReviewCuDto itemDtoSrc, DbReview itemDst)
    {
        // Attraction not nullable
        var attraction = await _dbContext.Attraction
            .FirstOrDefaultAsync(a => a.AttractionId == itemDtoSrc.AttractionId);
        if (attraction == null)
            throw new ArgumentException($"Attraction id {itemDtoSrc.AttractionId} not existing");
        itemDst.DbAttraction = attraction;

        // User not nullable
        var user = await _dbContext.User
            .FirstOrDefaultAsync(u => u.UserId == itemDtoSrc.UserId);
        if (user == null)
            throw new ArgumentException($"User id {itemDtoSrc.UserId} not existing");
        itemDst.DbUser = user;

        // Comment is optional, new id is generated 
        if (!string.IsNullOrWhiteSpace(itemDtoSrc.CommentText))
        {
            if (itemDst.DbComment != null)
            {
                // If comments exist, update
                itemDst.DbComment.CommentText = itemDtoSrc.CommentText;
            }
            else
            {
                // Create new comment if none exist
                var comment = new DbComment
                {
                    CommentId = Guid.NewGuid(),
                    CommentText = itemDtoSrc.CommentText
                };

                _dbContext.Comment.Add(comment);
                itemDst.DbComment = comment;
            }
        }
        else
        {
            // If no input, set null
            itemDst.DbComment = null;
        }
    }
    #endregion

    #region Update item
    public async Task<ResponseItemDto<IReview>> UpdateReviewAsync(ReviewCuDto itemDto)
    {
        //Find the instance with matching id and read the navigation properties.
        var query1 = _dbContext.Review
            .Where(i => i.ReviewId == itemDto.ReviewId);
        var item = await query1
            .Include(i => i.DbAttraction)
            .Include(i => i.DbUser)
            .Include(i => i.DbComment)
            .FirstOrDefaultAsync<DbReview>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.ReviewId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_ReviewCUdto_to_ReviewDbM(itemDto, item);

        //write to database model
        _dbContext.Review.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadReviewAsync(item.ReviewId, false);
    }
    #endregion
}
