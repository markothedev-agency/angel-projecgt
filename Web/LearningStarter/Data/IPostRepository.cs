using LearningStarter.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningStarter.Data
{
   

    public interface IPostRepository
    {
        Task<Post> SaveMessageAsync(Post message);
        Task<List<Post>> GetMessagesAsync(int chatRoomId);
        Task<ServerPosts> GetServerAndCorrespondingMessages(int chatRoomId);
        Task<List<Server>> GetChatRoomsAsync();
    }

    public class PostRepository : IPostRepository
    {
        private readonly DataContext _context;

        public PostRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Post> SaveMessageAsync(Post message)
        {
             var save = _context.Set<Post>().Add(message);
            await _context.SaveChangesAsync();
            return save.Entity;
        }

        public async Task<List<Post>> GetMessagesAsync(int serverId)
        {
            return await _context.Set<Post>()
                .Where(m => m.ServerId == serverId)
                .ToListAsync();
        }

        public async Task<ServerPosts> GetServerAndCorrespondingMessages(int chatRoomId)
        {
            try
            {
                var query = await _context.Set<Server>()
                        .Where(t => t.Id.Equals(chatRoomId)).AsNoTracking()
                            .GroupJoin(
                            _context.Set<Post>().AsNoTracking(),
                            server => server.Id,
                            post => post.ServerId,
                            (server,post )=> new {server,post}
                            ).Select(r => new ServerPosts
                            {
                                ServerId = r.server.Id,
                                Description = r.server.Description,
                                Name = r.server.Name,   

                                Posts = r.post.Select(t => new PostDto
                                {
                                    PostId = t.Id,
                                    Text =t.Text,
                                    Time = t.Time,
                                    SentBy=t.UserName,
                                }).ToList(),
                            }).ToListAsync();

                var result = query[0];
                return result;

            }catch(Exception e)
            {
                return new();
            }

            /*
             return await _context.ChatRooms .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == chatRoomId);
             */
        }

        public async Task<List<Server>> GetChatRoomsAsync()
        {
            return await _context.Set<Server>().ToListAsync();
        }
    }

}
