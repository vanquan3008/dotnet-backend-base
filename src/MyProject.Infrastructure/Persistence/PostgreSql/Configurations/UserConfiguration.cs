using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;
using MyProject.Domain.Enums;
using MyProject.Domain.ValueObjects.Users;
using UserEmail = MyProject.Domain.ValueObjects.Users.Email;

namespace MyProject.Infrastructure.Persistence.PostgreSql.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", table =>
            table.HasCheckConstraint("ck_users_status", "status IN (1, 2, 3, 4)"));

        builder.HasKey(user => user.Id).HasName("pk_users");
        builder.Property(user => user.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(user => user.Code)
            .HasConversion(code => code.Value, value => Code.Create(value))
            .HasColumnName("code").HasColumnType("varchar(50)").IsRequired();
        builder.Property(user => user.Name)
            .HasConversion(name => name.Value, value => Name.Create(value))
            .HasColumnName("name").HasColumnType("varchar(200)").IsRequired();
        builder.Property(user => user.Email)
            .HasConversion(email => email.Value, value => UserEmail.Create(value))
            .HasColumnName("email").HasColumnType("varchar(320)").IsRequired();
        builder.Property(user => user.Status).HasColumnName("status")
            .HasConversion<int>().HasColumnType("integer").IsRequired();
        builder.Property(user => user.AvatarUrl).HasColumnName("avatar_url")
            .HasColumnType("varchar(2048)");
        builder.Property(user => user.LastLoginAt).HasColumnName("last_login_at")
            .HasColumnType("timestamp with time zone");
        builder.Property(user => user.CreatedAt).HasColumnName("created_at")
            .HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(user => user.ModifiedAt).HasColumnName("modified_at")
            .HasColumnType("timestamp with time zone");
        builder.HasIndex(user => user.Code).IsUnique().HasDatabaseName("ux_users_code");
        builder.HasIndex(user => user.Email).IsUnique().HasDatabaseName("ux_users_email");
    }
}
