using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class Server
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ServerClassroom> Classes { get; set; }
}
public class ServerPosts
{
    public int ServerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<PostDto> Posts { get; set; }
}
public class PostDto
{
    public int PostId { get; set; }
    public string Text { get; set; }
    public string SentBy { get; set; }

    public DateTimeOffset Time { get; set; }
}
public class ServerGetDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<ServerClassroomGetDto> Classes { get; set; }
}

public class ServerCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ServerUpdateDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ServerEntityTypeConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.ToTable("Server");
    }
}