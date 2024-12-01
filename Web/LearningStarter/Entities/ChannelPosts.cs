using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class ChannelPosts
{
    public int Id { get; set; }
    public int ChannelId { get; set; }
    public Channel Channel { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}

public class ChannelPostsGetDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Time { get; set; }
}

public class ChannelPostsEntityTypeConfiguration : IEntityTypeConfiguration<ChannelPosts>
{
    public void Configure(EntityTypeBuilder<ChannelPosts> builder)
    {
        builder.ToTable("ChannelPosts");
        
        builder.HasOne(x => x.Channel)
            .WithMany(x => x.Posts);
        builder.HasOne(x => x.Post)
            .WithMany();
    }
}


