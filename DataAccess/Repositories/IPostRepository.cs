using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPostRepository
    {
        IQueryable<Post> GetPosts();
        Task<Post> GetPostById(int id);
        Task InsertPostAsync(Post createItem);
        Task UpdatePostAsync(Post updateItem);
        Task DeletePostAsync(int Id);
    }
}
