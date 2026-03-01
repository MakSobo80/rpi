using Notatnik.Models;
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

        public static Organization GetOrganization(int? organizationId)
        {
            if (organizationId == null)
            {
                return null;
            }
            using (var context = new Models.AppDbContext())
            {
                return context.Organizations.FirstOrDefault(o => o.Id == organizationId);
            }
        }

        public static void AddFile(string name, byte[] fileContent, int authorId, int organizationId, int? parentId = null)
        {
            using (var context = new Models.AppDbContext())
            {
                var file = new Models.Filez
                {
                    Name = name,
                    File = fileContent,
                    AuthorId = (byte)authorId,
                    OrganizationId = (byte)organizationId,
                    Parent = (byte)parentId
                };
                context.Filezs.Add(file);
                context.SaveChanges();
            }
        }
    }
}
