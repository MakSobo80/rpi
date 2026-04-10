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
        private const int MaxNameLength = 10;
        private const byte DefaultOrganizationId = 1;

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
                    string safeName = name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;
                    var file = new Models.Filez
                    {
                        Name = safeName,
                        File = fileContent,
                        AuthorId = checked((byte)authorId),
                        OrganizationId = checked((byte)organizationId),
                        Parent = parentId.HasValue ? checked((byte?)parentId.Value) : null
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

        public static List<Models.Filez> GetFilesForOrganization(int organizationId)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Filezs
                        .Where(f => f.OrganizationId == organizationId)
                        .ToList();
                }
                catch
                {
                    return new List<Models.Filez>();
                }
            }
        }

        public static Models.Filez? GetFileById(int id)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Filezs.FirstOrDefault(f => f.Id == id);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static void DeleteFile(int id)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    var file = context.Filezs.FirstOrDefault(f => f.Id == id);
                    if (file != null)
                    {
                        context.Filezs.Remove(file);
                        context.SaveChanges();
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public static List<Models.User> GetUsersInOrganization(int organizationId)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Users
                        .Where(u => u.OrganizationId == organizationId)
                        .ToList();
                }
                catch
                {
                    return new List<Models.User>();
                }
            }
        }

        public static void RemoveUserFromOrganization(int userId)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        user.OrganizationId = DefaultOrganizationId;
                        context.SaveChanges();
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public static void SetUserManager(int userId, bool isManager)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    var user = context.Users.FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        user.IsManager = isManager;
                        context.SaveChanges();
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        public static List<Models.Organization> GetAllOrganizations()
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Organizations.ToList();
                }
                catch
                {
                    return new List<Models.Organization>();
                }
            }
        }

        public static Models.User? GetUserById(int id)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Users.FirstOrDefault(u => u.Id == id);
                }
                catch
                {
                    return null;
                }
            }
        }

        public static void AddOrganization(string name)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    string safeName = name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;
                    var org = new Models.Organization { Name = safeName };
                    context.Organizations.Add(org);
                    context.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a folder-marker record (empty file content) and returns the new record's ID,
        /// or -1 on failure.
        /// </summary>
        public static int AddFolderRecord(string name, int authorId, int organizationId, int? parentId = null)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    string safeName = name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;
                    var folder = new Models.Filez
                    {
                        Name = safeName,
                        File = Array.Empty<byte>(),
                        AuthorId = checked((byte)authorId),
                        OrganizationId = checked((byte)organizationId),
                        Parent = parentId.HasValue ? checked((byte?)parentId.Value) : null
                    };
                    context.Filezs.Add(folder);
                    context.SaveChanges();
                    return folder.Id;
                }
                catch
                {
                    return -1;
                }
            }
        }
    }
}
