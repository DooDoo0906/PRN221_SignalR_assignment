using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        public async Task<AppUser> GetAccountUser(int userId, string password) => await AppUserDAO.Instance.GetAccountUserByIDAsync(userId, password);

        public async Task DeleteUserAsync(int Id) => await AppUserDAO.Instance.RemoveAsync(Id);

        public async Task<AppUser> GetUserById(int id) => await AppUserDAO.Instance.GetAppUserById(id);

        public IQueryable<AppUser> GetUsers() => AppUserDAO.Instance.GetAppUsers();

        public async Task InsertUserAsync(AppUser createItem) => await AppUserDAO.Instance.AddNewAsync(createItem);

        public async Task UpdateUserAsync(AppUser updateItem) => await AppUserDAO.Instance.UpdateAsync(updateItem);
    }
}
