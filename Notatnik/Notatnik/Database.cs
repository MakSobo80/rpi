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
        // Maximum length for file/folder/organization names. Set to 255 to match typical DB varchar limits
        // and avoid unintended truncation. Names exceeding this length will be truncated by TruncateName.
        private const int MaxNameLength = 255;

        // Default organization ID for users not assigned to any organization.
        private const byte DefaultOrganizationId = 1;

        private static string TruncateName(string name) =>
            name.Length > MaxNameLength ? name.Substring(0, MaxNameLength) : name;

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
                    string safeName = TruncateName(name);
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

        /// <summary>
        /// Returns metadata-only file records (without blob data) for the given organization.
        /// Only includes actual files (excludes folder records with empty content).
        /// </summary>
        public static List<(int Id, string Name, byte AuthorId, int SizeBytes)> GetFileMetadataForOrganization(int organizationId)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    return context.Filezs
                        .Where(f => f.OrganizationId == organizationId && f.File != null && f.File.Length > 0)
                        .Select(f => new ValueTuple<int, string, byte, int>(f.Id, f.Name, f.AuthorId, f.File != null ? f.File.Length : 0))
                        .ToList();
                }
                catch
                {
                    return new List<(int, string, byte, int)>();
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

        /// <summary>
        /// Assigns an existing database user to the given organization.
        /// Returns 0 on success, 1 if the user is not found, 2 if the user already belongs to
        /// a real organization (OrganizationId != DefaultOrganizationId).
        /// </summary>
        public static int AddUserToOrganization(string username, int organizationId)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == username);
                    if (user == null)
                        return 1;
                    if (user.OrganizationId != DefaultOrganizationId)
                        return 2;
                    user.OrganizationId = checked((byte)organizationId);
                    context.SaveChanges();
                    return 0;
                }
                catch
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Renames the organization with the given ID. Returns true on success.
        /// </summary>
        public static bool RenameOrganization(int organizationId, string newName)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    var org = context.Organizations.FirstOrDefault(o => o.Id == organizationId);
                    if (org == null)
                        return false;
                    org.Name = TruncateName(newName);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool AddOrganization(string name)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    string safeName = TruncateName(name);
                    var org = new Models.Organization { Name = safeName };
                    context.Organizations.Add(org);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
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
                    string safeName = TruncateName(name);
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

        /// <summary>
        /// Inserts a new file record or updates the content of an existing one that has the
        /// same name, organization, and parent.
        /// </summary>
        public static void UpsertFile(string name, byte[] fileContent, int authorId, int organizationId, int? parentId = null)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    string safeName = TruncateName(name);
                    byte orgByte = checked((byte)organizationId);
                    byte? parentByte = parentId.HasValue ? checked((byte?)parentId.Value) : null;

                    var existing = context.Filezs.FirstOrDefault(f =>
                        f.Name == safeName &&
                        f.OrganizationId == orgByte &&
                        f.Parent == parentByte);

                    if (existing != null)
                    {
                        existing.File = fileContent;
                        existing.AuthorId = checked((byte)authorId);
                    }
                    else
                    {
                        context.Filezs.Add(new Models.Filez
                        {
                            Name = safeName,
                            File = fileContent,
                            AuthorId = checked((byte)authorId),
                            OrganizationId = orgByte,
                            Parent = parentByte
                        });
                    }
                    context.SaveChanges();
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Returns the ID of an existing folder record (name + org + parent) or inserts a new
        /// one and returns its ID. Returns -1 on failure.
        /// </summary>
        public static int UpsertFolderRecord(string name, int authorId, int organizationId, int? parentId = null)
        {
            using (var context = new Models.AppDbContext())
            {
                try
                {
                    string safeName = TruncateName(name);
                    byte orgByte = checked((byte)organizationId);
                    byte? parentByte = parentId.HasValue ? checked((byte?)parentId.Value) : null;

                    var existing = context.Filezs.FirstOrDefault(f =>
                        f.Name == safeName &&
                        f.OrganizationId == orgByte &&
                        f.Parent == parentByte &&
                        f.File != null &&
                        f.File.Length == 0);

                    if (existing != null)
                        return existing.Id;

                    var folder = new Models.Filez
                    {
                        Name = safeName,
                        File = Array.Empty<byte>(),
                        AuthorId = checked((byte)authorId),
                        OrganizationId = orgByte,
                        Parent = parentByte
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