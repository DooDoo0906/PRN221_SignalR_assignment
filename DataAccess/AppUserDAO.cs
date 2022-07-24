using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AppUserDAO
    {
        private static AppUserDAO instance = null;
        private static readonly object instanceLock = new object();
        public static AppUserDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                        instance = new AppUserDAO();
                }
                return instance;
            }
        }
        public async Task<AppUser> GetAccountUserByIDAsync(int userId, string password)
        {
            using var context = new ApplicationDBContext();
            var item = await context.AppUsers.FirstOrDefaultAsync(m => m.UserId == userId && m.Password == password);
            return item;
        }
        public IQueryable<AppUser> GetAppUsers()
        {
            using var context = new ApplicationDBContext();
            var items = from s in context.AppUsers.Include(p => p.Posts)
                        select s;
            return items;
        }
        public async Task<AppUser> GetAppUserById(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.AppUsers.FirstOrDefaultAsync(m => m.UserId == Id);
            return item;
        }
        public async Task AddNewAsync(AppUser createItem)
        {
            var item = await GetAppUserById(createItem.UserId);
            if (item == null)
            {
                using var context = new ApplicationDBContext();
                context.AppUsers.Add(createItem);
                await context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(AppUser updateItem)
        {
            var item = await GetAppUserById(updateItem.UserId);
            if (item != null)
            {
                using var context = new ApplicationDBContext();
                context.AppUsers.Update(updateItem);
                context.SaveChanges();
            }

        }

        public async Task RemoveAsync(int Id)
        {
            using var context = new ApplicationDBContext();
            var item = await context.AppUsers.FindAsync(Id);

            if (item != null)
            {
                context.AppUsers.Remove(item);
                await context.SaveChangesAsync();
            }
        }
    }
}
