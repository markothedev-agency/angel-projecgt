using System.Linq;
using LearningStarter.Common;
using LearningStarter.Data;
using LearningStarter.Entities;
using LearningStarter.Services;
using Microsoft.AspNetCore.Mvc;
namespace LearningStarter.Controllers;
[Route("api/channels")]
public class ChannelController: ControllerBase
{
    private readonly DataContext _dataContext;
    private readonly IAuthenticationService _authenticationService;
    public ChannelController(DataContext dataContext, IAuthenticationService authenticationService)
    {
        _dataContext = dataContext;
        _authenticationService = authenticationService;
    }
    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new Response();
        
        var data = _dataContext
            .Set<Channel>()
            .Select(channel=> new ChannelGetDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
            })
            .ToList();
        response.Data = data;
        return Ok(response);
    }
    [HttpGet("{id}")]
    public IActionResult GetbyId(int id)
    {
        var response = new Response();
        
        var data = _dataContext
            .Set<Channel>()
            .Select(channel=> new ChannelGetDto
            {
                Id = channel.Id,
                Name = channel.Name,
                Description = channel.Description,
            })
            .FirstOrDefault(channel => channel.Id == id );
        response.Data = data;
        return Ok(response);
    }
    [HttpPost]
    public IActionResult Create([FromBody] ChannelCreateDto createDto)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(createDto.Description))
        {
            response.AddError("Description", "Description must not be empty.");
        }
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        var channelToCreate = new Channel 
        {
            Name = createDto.Name,
            Description = createDto.Description
        };
        _dataContext.Set<Channel>().Add(channelToCreate);
        _dataContext.SaveChanges();
        var channelToReturn = new ChannelGetDto
        {
            Id = channelToCreate.Id,
            Name = channelToCreate.Name,
            Description = channelToCreate.Description
        };
        response.Data = channelToReturn;
        return Created("", response);
    }
    
    [HttpPost("{channelId}/post/{postId}")]
    public IActionResult AddPostToChannel(int channelId, int postId)
    {
        var response = new Response();
        
        var channel = _dataContext.Set<Channel>()
            .FirstOrDefault(x => x.Id == channelId);
        
        var post = _dataContext.Set<Post>()
            .FirstOrDefault(x => x.Id == postId);
        
        if(channel == null || post == null)
        {
            response.AddError("id", "Channel or Post not found.");
        }
        
        var channelPosts = new ChannelPosts
        {
            Channel = channel,
            Post = post
        };
        
        _dataContext.Set<ChannelPosts>().Add(channelPosts);
        _dataContext.SaveChanges();
        
        response.Data = new ChannelGetDto
        {
            Id = channel.Id,
            Name = channel.Name,
            Description = channel.Description,
            Posts = channel.Posts.Select(x => new ChannelPostsGetDto
            {
                Id = x.Post.Id,
                Text = x.Post.Text,
                Time = x.Post.Time
            }).ToList()
        };
        
        return Ok(response);
    }
    
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] ChannelUpdateDto updateDto, int id)
    {
        var response = new Response();
        
        var channelToUpdate = _dataContext
            .Set<Channel>()
            .FirstOrDefault(channel => channel.Id == id);
        if(channelToUpdate == null)
        {
            response.AddError("id", "Post not found." );
        }
        if(string.IsNullOrEmpty(updateDto.Description))
        {
            response.AddError("Description", "Description must not be empty.");
        }
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        channelToUpdate.Name = updateDto.Name;
        channelToUpdate.Description = updateDto.Description;
        _dataContext.SaveChanges();
        var channelToReturn = new ChannelGetDto
        {
            Id = channelToUpdate.Id,
            Name = channelToUpdate.Name,
            Description = channelToUpdate.Description,
        };
        response.Data = channelToReturn;
        return Ok(response);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();
        var channelToDelete = _dataContext
            .Set<Channel>()
            .FirstOrDefault(channel => channel.Id == id);
        if(channelToDelete == null)
        {
            response.AddError("id", "Channel not found." );
        }
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        _dataContext.Set<Channel>().Remove(channelToDelete);
        _dataContext.SaveChanges();
        response.Data = true;
        return Ok(response);
    }
}