using System.IO;

namespace Notatnik
{
    internal static class FileStorageHelper
    {
        /// <summary>
        /// Returns (and creates if needed) the local AppData folder for the given organization.
        /// Path: %AppData%\Notatnik\files\{orgId}
        /// </summary>
        public static string GetOrgDataFolder(int orgId)
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notatnik", "files", orgId.ToString());
            Directory.CreateDirectory(folder);
            return folder;
        }

        /// <summary>
        /// Recursively uploads all files from the local org folder to the database,
        /// preserving the directory structure via the parent column.
        /// </summary>
        public static void UploadFolderToDatabase(int orgId, int authorId)
        {
            string rootFolder = GetOrgDataFolder(orgId);
            UploadDirectory(rootFolder, orgId, authorId, null);
        }

        private static void UploadDirectory(string dirPath, int orgId, int authorId, int? parentId)
        {
            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                string name = Path.GetFileName(filePath);
                byte[] content = File.ReadAllBytes(filePath);
                Database.AddFile(name, content, authorId, orgId, parentId);
            }

            foreach (string subDir in Directory.GetDirectories(dirPath))
            {
                string dirName = Path.GetFileName(subDir);
                int folderId = Database.AddFolderRecord(dirName, authorId, orgId, parentId);
                if (folderId >= 0)
                    UploadDirectory(subDir, orgId, authorId, folderId);
            }
        }

        /// <summary>
        /// Downloads all files for the organization from the database to the local AppData folder,
        /// reconstructing the directory hierarchy from the parent column.
        /// </summary>
        public static void DownloadDatabaseToFolder(int orgId)
        {
            string rootFolder = GetOrgDataFolder(orgId);
            var allFiles = Database.GetFilesForOrganization(orgId);
            var fileDict = allFiles.ToDictionary(f => f.Id);

            foreach (var file in allFiles)
            {
                string localPath = ResolveLocalPath(file, fileDict, rootFolder);

                if (file.File == null || file.File.Length == 0)
                {
                    // Folder record — ensure the directory exists
                    Directory.CreateDirectory(localPath);
                }
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);
                    File.WriteAllBytes(localPath, file.File);
                }
            }
        }

        /// <summary>
        /// Resolves the full local filesystem path for a single database file record,
        /// walking the parent chain to reconstruct the relative directory structure.
        /// </summary>
        public static string ResolveLocalPath(
            Models.Filez file,
            Dictionary<byte, Models.Filez> fileDict,
            string rootFolder)
        {
            return ResolveLocalPathInternal(file, fileDict, rootFolder, new HashSet<byte>());
        }

        private static string ResolveLocalPathInternal(
            Models.Filez file,
            Dictionary<byte, Models.Filez> fileDict,
            string rootFolder,
            HashSet<byte> visited)
        {
            if (file.Parent == null || visited.Contains(file.Id))
                return Path.Combine(rootFolder, file.Name.Trim());

            visited.Add(file.Id);

            if (fileDict.TryGetValue(file.Parent.Value, out var parent))
                return Path.Combine(ResolveLocalPathInternal(parent, fileDict, rootFolder, visited), file.Name.Trim());

            // Parent not found — fall back to root
            return Path.Combine(rootFolder, file.Name.Trim());
        }
    }
}
