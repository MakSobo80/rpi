using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace Notatnik
{
    internal static class Database
    {

        public static void RegisterUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            using (var context = new Models.AppDbContext())
            {
                var user = new Models.User
                {
                    Username = username,
                    IsManager = false,
                    OrganizationId = 1
                };
                context.Users.Add(user);
                context.SaveChanges();
            }
        }

        public static bool UserExists(string username)
        {
            using (var context = new Models.AppDbContext())
            {
                return context.Users.Any(u => u.Username == username);
            }
        }
    }
}
