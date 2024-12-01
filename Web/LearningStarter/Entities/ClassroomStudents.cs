using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class ClassroomStudents
{
    public int Id { get; set; }
    
    public int ClassroomId { get; set; }
    public Classroom Classroom { get; set; }
    
    public int StudentId { get; set; }
    public Student Student { get; set; }
}

public class ClassroomStudentsGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StudentEmail { get; set; }
}

public class ClassroomStudentsEntityTypeConfiguration : IEntityTypeConfiguration<ClassroomStudents>
{
    public void Configure(EntityTypeBuilder<ClassroomStudents> builder)
    {
        builder.ToTable("ClassroomStudents");

        builder.HasOne(x => x.Classroom)
            .WithMany(x => x.Students);
        
        builder.HasOne(x => x.Student)
            .WithMany();
    }
}