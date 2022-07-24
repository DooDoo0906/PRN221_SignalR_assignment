using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public  interface IPostCategoryRepository
    {
        IQueryable<PostCategory> GetPostCategories();
        Task<PostCategory> GetPostCategoryById(int id);
        Task InsertPostCategoryAsync(PostCategory createItem);
        Task UpdatePostCategoryAsync(PostCategory updateItem);
        Task DeletePostCategoryAsync(int Id);
    }
}
