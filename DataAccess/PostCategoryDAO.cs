using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class PostCategoryDAO
    {
        private static PostCategoryDAO instance = null;
        private static readonly object instanceLock = new object();
        public static PostCategoryDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new PostCategoryDAO();
                }
                return instance;
            }
        }

        public IQueryable<PostCategory> GetPostCategories()
        {
            using var context = new ApplicationDBContext();
            var items = from s in context.PostCategories.Include(p => p.Posts)
                        select s;
            return items;
        }
        public async Task<PostCategory> GetPostCategoryById(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.PostCategories.FirstOrDefaultAsync(m => m.CategoryId == Id);
            return item;
        }
        public async Task AddNewAsync(PostCategory createItem)
        {
            var item = await GetPostCategoryById(createItem.CategoryId);
            if (item == null)
            {
                using var context = new ApplicationDBContext();
                context.PostCategories.Add(createItem);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(PostCategory updateItem)
        {
            var item = await GetPostCategoryById(updateItem.CategoryId);
            if (item != null)
            {
                using var context = new ApplicationDBContext();
                context.PostCategories.Update(updateItem);
                context.SaveChanges();
            }

        }

        public async Task RemoveAsync(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.PostCategories.FindAsync(Id);

            if (item != null)
            {
                context.PostCategories.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
