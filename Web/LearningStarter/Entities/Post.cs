using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LearningStarter.Entities;
public class Post
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int ServerId { get; set; }
    public string? UserName { get; set; }
    // public int ServerChannelId { get; set; }


    public DateTimeOffset Time { get; set; }
   
}
public class PostGetDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset Time { get; set; }
}
public class PostCreateDto
{
    [Required]
    [MinLength(1)]
    public string UserName { get; set; }
    [Required]
    public int ServerId { get; set; }
    [Required]
    public string Text { get; set; }
    public DateTimeOffset Time { get; set; } = DateTimeOffset.Now;
}
public class PostUpdateDto
{
    [Required]
    [MinLength(1)]
    public string UserName { get; set; }
    [Required]
    public int ServerId { get; set; }
    [Required]
    public string Text { get; set; }
    public DateTimeOffset Time { get; set; } = DateTimeOffset.Now;
}
public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");
    }
}