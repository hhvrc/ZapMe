using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using ZapMe.Data;
using ZapMe.Data.Models;
using ZapMe.Services.Interfaces;

namespace ZapMe.Services;

public sealed class UserStore : IUserStore
{
    private readonly ZapMeContext _dbContext;
    private readonly IHybridCache _cache;
    private readonly ILogger<UserStore> _logger;

    public UserStore(ZapMeContext dbContext, IHybridCache cacheProviderService, ILogger<UserStore> logger)
    {
        _dbContext = dbContext;
        _cache = cacheProviderService;
        _logger = logger;
    }

    public async Task<UserEntity?> TryCreateAsync(string username, string email, string passwordHash, CancellationToken cancellationToken)
    {
        var user = new UserEntity
        {
            UserName = username,
            Email = email,
            PasswordHash = passwordHash,
            OnlineStatus = UserOnlineStatus.Online,
            StatusText = String.Empty,
            UpdatedAt = DateTime.UtcNow
        };

        try
        {
            await _dbContext.Users.AddAsync(user, cancellationToken);
            int nAdded = await _dbContext.SaveChangesAsync(cancellationToken);

            if (nAdded > 0)
            {
                return user;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to create user {Username}", username);
        }

        return null;
    }

    public async Task<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _cache.GetOrAddAsync("user:id:" + userId, async (_, ct) =>
            new DTOs.HybridCacheEntry<UserEntity?>
            {
                Value = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct),
                ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            }
        , cancellationToken);
    }

    public async Task<UserEntity?> GetByNameAsync(string userName, CancellationToken cancellationToken)
    {
        return await _cache.GetOrAddAsync("user:name:" + userName, async (_, ct) =>
            new DTOs.HybridCacheEntry<UserEntity?>
            {
                Value = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct),
                ExpiresAtUtc = DateTime.UtcNow + TimeSpan.FromMinutes(5)
            }
        , cancellationToken);
    }

    public async Task<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<UserEntity?> GetByPasswordResetTokenAsync(string passwordResetToken, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == passwordResetToken, cancellationToken);
    }

    private async Task<bool> UpdateAsync(Expression<Func<UserEntity, bool>> whereSelector, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken)
    {
        try
        {
            return (await _dbContext.Users.Where(whereSelector).ExecuteUpdateAsync(setPropertyCalls, cancellationToken)) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to update user");
        }

        return false;
    }
    private Task<bool> UpdateAsync(Guid userId, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.Id == userId, setPropertyCalls, cancellationToken);
    private Task<bool> UpdateAsync(string userName, Expression<Func<SetPropertyCalls<UserEntity>, SetPropertyCalls<UserEntity>>> setPropertyCalls, CancellationToken cancellationToken) =>
        UpdateAsync(u => u.UserName == userName, setPropertyCalls, cancellationToken);

    public Task<bool> SetUserNameAsync(Guid userId, string userName, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.UserName, _ => userName).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailAsync(Guid userId, string email, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.Email, _ => email).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetEmailVerifiedAsync(Guid userId, bool emailVerified, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.EmailVerified, _ => emailVerified).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetPasswordHashAsync(Guid userId, string passwordHash, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.PasswordHash, _ => passwordHash).SetProperty(static u => u.UpdatedAt, _ => DateTime.UtcNow), cancellationToken);
    public Task<bool> SetLastOnlineAsync(Guid userId, DateTime lastOnline, CancellationToken cancellationToken) =>
        UpdateAsync(userId, s => s.SetProperty(static u => u.LastOnline, _ => lastOnline), cancellationToken);

    public Task<bool> SetPasswordResetTokenAsync(Guid userId, string? token, CancellationToken cancellationToken)
    {
        DateTime? createdAt = String.IsNullOrEmpty(token) ? null : DateTime.UtcNow;

        return UpdateAsync(userId, s => s.
                        SetProperty(static u => u.PasswordResetToken, _ => token).
                        SetProperty(static u => u.PasswordResetRequestedAt, _ => createdAt),
                        cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await _dbContext.Users.Where(u => u.Id == userId).ExecuteDeleteAsync(cancellationToken) > 0;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to delete user {UserId}", userId);
        }

        return false;
    }
}