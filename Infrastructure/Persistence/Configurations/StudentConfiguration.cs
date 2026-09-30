using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students", t =>
        t.HasCheckConstraint("CK_Students_Grade", "[Grade] BETWEEN 7 AND 9"));

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ParentEmail)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(s => s.Grade)
            .IsRequired();

        builder.HasMany(s => s.Interests)
            .WithMany()
            .UsingEntity(
               "StudentInterests",
                r => r.HasOne(typeof(Interest)).WithMany().HasForeignKey("InterestId"),
                l => l.HasOne(typeof(Student)).WithMany().HasForeignKey("StudentId"),
                j => j.HasKey("StudentId", "InterestId"));

        builder.Navigation(s => s.Interests)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}