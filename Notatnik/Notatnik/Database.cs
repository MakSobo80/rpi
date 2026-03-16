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
                try
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
                catch
                {
                    return;
                }
            }
        }

        public static bool UserExists(string username)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Users.Any(u => u.Username == username);
                } catch {
                    return false;
                }
            }
        }

        public static User GetUser(string username)
        {
            using (var context = new Models.AppDbContext())
            {
                User user;
                try
                {
                    user = context.Users.FirstOrDefault(u => u.Username == username);
                } catch
                {
                    user = new Models.User
                    {
                        Username = username,
                        IsManager = false,
                        OrganizationId = 0
                    };
                }
                return user;
            }
        }

        public static Organization? GetOrganization(int? organizationId)
        {
            if (organizationId == null)
            {
                return null;
            }
            using (var context = new Models.AppDbContext())
            {
                Organization? organization;
                try
                {
                    organization = context.Organizations.FirstOrDefault(o => o.Id == organizationId);
                } catch
                {
                    if (organizationId < byte.MinValue || organizationId > byte.MaxValue)
                        return null;
                    organization = new Models.Organization
                    {
                        Id = checked((byte)organizationId.Value),
                        Name = "Unknown"
                    };
                }
                return organization;
            }
        }

        public static void AddFile(string name, byte[] fileContent, int authorId, int organizationId, int? parentId = null)
        {
            using (var context = new Models.AppDbContext())
            {
                try
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
                catch
                {
                    return;
                }
            }
        }
    }
}
