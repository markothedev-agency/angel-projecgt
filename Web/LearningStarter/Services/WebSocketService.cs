
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;
using System;
using LearningStarter.Data;
using LearningStarter.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace LearningStarter.Services
{

 
    public class WebSocketService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ConcurrentDictionary<string, (WebSocket Socket, int ServerId)> _sockets = new ConcurrentDictionary<string, (WebSocket, int)>();

        public WebSocketService(IServiceScopeFactory serviceScopeFactory)
        { 
            _serviceScopeFactory = serviceScopeFactory;
        }

      
        public async Task AddSocket(WebSocket socket, int ServerId, string userName)
        {
            var socketId = Guid.NewGuid().ToString();
            _sockets.TryAdd(socketId, (socket, ServerId));

            using var scope = _serviceScopeFactory.CreateScope();
            var _postRepository = scope.ServiceProvider.GetRequiredService<IPostRepository>();

            await Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var chatMessage = new Post
                    {
                        UserName = userName, // Replace with actual user info
                        Text = message,
                        Time = DateTime.UtcNow,
                        ServerId = ServerId,
                    };
                    var post = await _postRepository.SaveMessageAsync(chatMessage);
                    var sendData = new { message=message, postId= post.Id, sentBy=userName };

                    var stringifyData = JsonConvert.SerializeObject(sendData);
                    await SendMessageToAllAsync(stringifyData, ServerId);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _sockets.TryRemove(socketId, out _);
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });

        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        private async Task SendMessageToAllAsync(string message, int chatRoomId)
        {
            foreach (var (socket, roomId) in _sockets.Values)
            {
                if (socket.State == WebSocketState.Open && roomId == chatRoomId)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), 
                        WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}

