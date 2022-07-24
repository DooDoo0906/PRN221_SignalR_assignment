using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PostDAO
    {
        private static PostDAO instance = null;
        private static readonly object instanceLock = new object();
        public static PostDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new PostDAO();
                }
                return instance;
            }
        }

        public IQueryable<Post> GetPosts()
        {
            using var context = new ApplicationDBContext();
            var items = from s in context.Posts.Include(m=>m.Author)
                        select s;
            return items;
        }
        public async Task<Post> GetPostById(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.Posts.FirstOrDefaultAsync(m => m.PostId == Id);
            return item;
        }
        public async Task AddNewAsync(Post createItem)
        {
            var item = await GetPostById(createItem.PostId);
            if (item == null)
            {
                using var context = new ApplicationDBContext();
                context.Posts.Add(createItem);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(Post updateItem)
        {
            var item = await GetPostById(updateItem.PostId);
            if (item != null)
            {
                using var context = new ApplicationDBContext();
                context.Posts.Update(updateItem);
                context.SaveChanges();
            }

        }

        public async Task RemoveAsync(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.Posts.FindAsync(Id);

            if (item != null)
            {
                context.Posts.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
