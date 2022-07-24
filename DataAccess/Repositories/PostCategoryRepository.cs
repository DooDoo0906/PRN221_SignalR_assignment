using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PostCategoryRepository : IPostCategoryRepository
    {
        public async Task DeletePostCategoryAsync(int Id) => await  PostCategoryDAO.Instance.RemoveAsync(Id);

        public async Task<PostCategory> GetPostCategoryById(int id) => await  PostCategoryDAO.Instance.GetPostCategoryById(id);

        public IQueryable<PostCategory> GetPostCategories() =>  PostCategoryDAO.Instance.GetPostCategories();

        public async Task InsertPostCategoryAsync(PostCategory createItem) => await  PostCategoryDAO.Instance.AddNewAsync(createItem);

        public async Task UpdatePostCategoryAsync(PostCategory updateItem) => await  PostCategoryDAO.Instance.UpdateAsync(updateItem);
    }
}
