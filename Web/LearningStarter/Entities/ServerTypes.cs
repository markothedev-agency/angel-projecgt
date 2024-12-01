using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningStarter.Entities;

public class ServerTypes
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }


}

public class ServerTypesGetDto
{
   public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ServerTypesCreateDto
{
    public string Name { get; set; }
    public string Description { get; set; } 
}

public class ServerTypesUpdateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    
}
public class ServerTypesEntityTypeConfiguration : IEntityTypeConfiguration<ServerTypes>
{
    public void Configure(EntityTypeBuilder<ServerTypes> builder)
    { 
        builder.ToTable("ServerTypes");
    }
}
