using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class ServerClassroom
{
    public int Id { get; set; }
    
    public int ServerId { get; set; }
    public Server Server { get; set; }
    
    public int ClassroomId { get; set; }
    public Classroom Classroom { get; set; }
}

public class ServerClassroomGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ServerClassroomEntityTypeConfiguration : IEntityTypeConfiguration<ServerClassroom>
{
    public void Configure(EntityTypeBuilder<ServerClassroom> builder)
    {
        builder.ToTable("ServerClassroom");

        builder.HasOne(x => x.Server)
            .WithMany(x => x.Classes);
        
        builder.HasOne(y => y.Classroom)
            .WithMany();
    }
}