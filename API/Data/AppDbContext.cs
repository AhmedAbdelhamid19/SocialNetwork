using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, IdentityRole<int>, int>(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberFollow> Follows { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = "Member", NormalizedName = "MEMBER" },
            new IdentityRole<int> { Id = 2, Name = "Moderator", NormalizedName = "MODERATOR" },
            new IdentityRole<int> { Id = 3, Name = "Admin", NormalizedName = "ADMIN" }
        );

        builder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.MessagesRecieved)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<MemberFollow>()
            .HasKey(k => new { k.SourceMemberId, k.TargetMemberId });

        builder.Entity<MemberFollow>()
            .HasOne(s => s.SourceMember)
            .WithMany(m => m.Following)
            .HasForeignKey(s => s.SourceMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<MemberFollow>()
            .HasOne(s => s.TargetMember)
            .WithMany(m => m.Followers)
            .HasForeignKey(s => s.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction );
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();

        configurationBuilder.Properties<DateTime?>()
            .HaveConversion<NullableUtcDateTimeConverter>();
    }
    private sealed class UtcDateTimeConverter()
        : ValueConverter<DateTime, DateTime>(v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    private sealed class NullableUtcDateTimeConverter()
        : ValueConverter<DateTime?, DateTime?>(v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
}