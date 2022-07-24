using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PostRepository : IPostRepository
    {
        public async Task DeletePostAsync(int Id) => await PostDAO.Instance.RemoveAsync(Id);

        public async Task<Post> GetPostById(int id) => await PostDAO.Instance.GetPostById(id);

        public IQueryable<Post> GetPosts() => PostDAO.Instance.GetPosts();

        public async Task InsertPostAsync(Post createItem) => await PostDAO.Instance.AddNewAsync(createItem);

        public async Task UpdatePostAsync(Post updateItem) => await PostDAO.Instance.UpdateAsync(updateItem);
    }
}
