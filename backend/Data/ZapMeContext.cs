using Microsoft.EntityFrameworkCore;
using ZapMe.Data.Models;

namespace ZapMe.Data;

public sealed class ZapMeContext : DbContext
{
    public ZapMeContext(DbContextOptions<ZapMeContext> options) : base(options)
    {
    }

    public required DbSet<UserEntity> Users { get; set; }
    public required DbSet<ImageEntity> Images { get; set; }
    public required DbSet<SignInEntity> SignIns { get; set; }
    public required DbSet<LockOutEntity> LockOuts { get; set; }
    public required DbSet<UserRoleEntity> UserRoles { get; set; }
    public required DbSet<UserRelationEntity> UserRelations { get; set; }
    public required DbSet<FriendRequestEntity> FriendRequests { get; set; }
    public required DbSet<OAuthConnectionEntity> OAuthConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseHiLo();

        new UserEntityConfiguration().Configure(modelBuilder.Entity<UserEntity>());
        new ImageEntityConfiguration().Configure(modelBuilder.Entity<ImageEntity>());
        new SignInEntityConfiguration().Configure(modelBuilder.Entity<SignInEntity>());
        new LockOutEntityConfiguration().Configure(modelBuilder.Entity<LockOutEntity>());
        new UserRoleEntityConfiguration().Configure(modelBuilder.Entity<UserRoleEntity>());
        new UserRelationEntityConfiguration().Configure(modelBuilder.Entity<UserRelationEntity>());
        new FriendRequestEntityConfiguration().Configure(modelBuilder.Entity<FriendRequestEntity>());
        new OAuthConnectionEntityConfiguration().Configure(modelBuilder.Entity<OAuthConnectionEntity>());
    }
}