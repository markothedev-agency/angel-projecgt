using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class ClassroomChannels
{
    public int Id { get; set; }
    public int ClassroomId { get; set; }
    public Classroom Classroom { get; set; }
    public int ChannelId { get; set; }
    public Channel Channel { get; set;}
}

public class ClassroomChannelsGetDto
{
    public int Id { get; set;  }
    public string Name { get; set;  }
    public string Description { get; set; }
}


public class ClassroomChannelsEntityTypeConfiguration : IEntityTypeConfiguration<ClassroomChannels>
{
    public void Configure(EntityTypeBuilder<ClassroomChannels> builder)
    {
        builder.ToTable("ClassroomChannels");
        
        builder.HasOne(x => x.Classroom)
            .WithMany(x => x.Channels);
        
        builder.HasOne(x => x.Channel)
            .WithMany();
    }
}


