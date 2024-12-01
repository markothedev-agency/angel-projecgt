using System.Linq;
using LearningStarter.Common;
using LearningStarter.Data;
using LearningStarter.Entities;
using Microsoft.AspNetCore.Mvc;
namespace LearningStarter.Controllers;

[ApiController]
[Route("api/server-types")]

public class ServerTypesController : ControllerBase
{
    private readonly DataContext _dataContext;

    public ServerTypesController(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response();
        
        var data = _dataContext
            .Set<ServerTypes>()
            .Select(ServerTypes => new ServerTypesGetDto
            {
                Id = ServerTypes.Id,
                Name = ServerTypes.Name,
                Description = ServerTypes.Description,
            })
            .ToList();
        response.Data = data;

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ServerTypesCreateDto createDto)
    {
        var response = new Response();

        var ServerTypesToCreate = new ServerTypes
        {
            Name = createDto.Name,
            Description = createDto.Description,
        };

        _dataContext.Set<ServerTypes>().Add(ServerTypesToCreate);
        _dataContext.SaveChanges();

        var ServerTypesToReturn = new ServerTypesGetDto()
        {
            Id = ServerTypesToCreate.Id,
            Name = ServerTypesToCreate.Name,
            Description = ServerTypesToCreate.Description,
        };
        response.Data = ServerTypesToReturn;

        return Created("", response);
        
    }
}