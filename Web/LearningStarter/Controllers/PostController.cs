using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using LearningStarter.Common;
using LearningStarter.Data;
using LearningStarter.Entities;
using LearningStarter.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace LearningStarter.Controllers;
[Route("api/posts")]
public class PostController: ControllerBase
{   
    private readonly DataContext _dataContext;
    private readonly IAuthenticationService _authenticationService;
    private readonly WebSocketService _webSocketManager;
    private readonly IPostRepository _postRepository;

    public PostController(DataContext dataContext, IAuthenticationService authenticationService, WebSocketService webSocketManager, IPostRepository postRepository)
    {
        _dataContext = dataContext;
        _authenticationService = authenticationService;
        _webSocketManager = webSocketManager;
        _postRepository = postRepository;
    }


    [HttpGet("/ws/{serverId}/{userName}")] 
    public async Task Get(int serverId, string userName)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest) 
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(); 
            await _webSocketManager.AddSocket(webSocket, serverId, userName);
        } else
        {
            HttpContext.Response.StatusCode = 400;
            
        }
    }


    [HttpGet("/{serverId}")]
    public async Task<IActionResult> GetAll(int serverId)
    {
        var response = new Response();

        var data = await _postRepository.GetMessagesAsync(serverId);
        if (data.Count < 1) {
            return NotFound(data);
        }
        response.Data = data;
        return Ok(response);
    }
    [HttpGet("get-server/{serverId}")]
    public async Task<IActionResult> GetServerAndCorrespondingMessages(int serverId)
    {
        var response = new Response();

        var data = await _postRepository.GetServerAndCorrespondingMessages(serverId);
        if (data is null)
        {
            return NotFound(data);
        }
        response.Data = data;
        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetbyId(int id)
    {
        var response = new Response();
        
        var data = _dataContext
              .Set<Post>()
              .Select(post=> new PostGetDto
              {
                   Id = post.Id,
                   Text = post.Text,
                   Time = post.Time
              })
              .FirstOrDefault(post => post.Id == id );
        response.Data = data;
        return Ok(response);
    }
    [HttpPost]
    public IActionResult Create([FromBody] PostCreateDto createDto)
    {
        var response = new Response();
        
        if(string.IsNullOrEmpty(createDto.Text))
        {
            response.AddError("Text", "Text must not be empty.");
        }
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        var postToCreate = new Post 
        {
            
            ServerId = createDto.ServerId,
            Text = createDto.Text,
            Time = createDto.Time,
            UserName = createDto.UserName,
        };
        _dataContext.Set<Post>().Add(postToCreate);
        _dataContext.SaveChanges();
        var postToReturn = new PostGetDto
        {
            Id = postToCreate.Id,
            Text = postToCreate.Text,
            Time = postToCreate.Time
        };
        response.Data = postToReturn;
        return Created("", response);
    }
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] PostUpdateDto updateDto, int id)
    {
        var response = new Response();
        
        var postToUpdate = _dataContext
            .Set<Post>()
            .FirstOrDefault(post => post.Id == id && 
                    post.ServerId.Equals(updateDto.ServerId) && 
                    post.UserName.Equals(updateDto.UserName, System.StringComparison.Ordinal)
             );
        if(postToUpdate == null)
        {
             response.AddError("id", "Post not found." );
        }
        if(string.IsNullOrEmpty(updateDto.Text))
        {
             response.AddError("Text", "Text must not be empty.");
        }
        if(response.HasErrors)
        {
             return BadRequest(response);
        }
        postToUpdate.Text = updateDto.Text;
        //postToUpdate.UserName = updateDto.UserName;
        //postToUpdate.ServerId = updateDto.ServerId;

        postToUpdate.Time = updateDto.Time;
        _dataContext.SaveChanges();
        var postToReturn = new PostGetDto
        {
            Id = postToUpdate.Id,
            Text = postToUpdate.Text,
            Time = postToUpdate.Time
        };
        response.Data = postToReturn;
        return Ok(response);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new Response();
        var postToDelete = _dataContext
            .Set<Post>()
            .FirstOrDefault(post => post.Id == id);
        if(postToDelete == null)
        {
            response.AddError("id", "Post not found." );
        }
        if(response.HasErrors)
        {
            return BadRequest(response);
        }
        _dataContext.Set<Post>().Remove(postToDelete);
        _dataContext.SaveChanges();
        response.Data = true;
        return Ok(response);
    }
}