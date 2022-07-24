using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IAppUserRepository
    {
        IQueryable<AppUser> GetUsers();
        Task<AppUser> GetAccountUser(int userId, string password);
        Task<AppUser> GetUserById(int id);
        Task InsertUserAsync(AppUser createItem);
        Task UpdateUserAsync(AppUser updateItem);
        Task DeleteUserAsync(int Id);
    }
}
