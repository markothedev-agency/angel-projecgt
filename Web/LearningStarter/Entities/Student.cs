using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StudentEmail { get; set; }
    public List<string> Classrooms { get; set; }
}

public class StudentGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string StudentEmail { get; set; }
    public List<string> Classrooms { get; set; }
}

public class StudentCreateDto
{
    public string Name { get; set; }
    public string StudentEmail { get; set; }
}

public class StudentUpdateDto
{
    public string Name { get; set; }
    public string StudentEmail { get; set; }
}

public class StudentEntityTypeConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Student");
    }
}