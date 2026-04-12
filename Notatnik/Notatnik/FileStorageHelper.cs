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
        /// Uploads the contents of the local org folder to the database using a BFS walk.
        /// For each directory level: files are upserted first, then subdirectories are upserted
        /// as folder records (with their parent set to the containing folder's database ID),
        /// and their contents are enqueued for the next level.
        /// Existing records (matched by name + org + parent) are updated; new ones are inserted.
        /// </summary>
        public static void UploadFolderToDatabase(int orgId, int authorId)
        {
            string rootFolder = GetOrgDataFolder(orgId);
            var queue = new Queue<(string dirPath, int? parentId)>();
            queue.Enqueue((rootFolder, null));

            while (queue.Count > 0)
            {
                var (dirPath, parentId) = queue.Dequeue();

                foreach (string filePath in Directory.GetFiles(dirPath))
                {
                    string name = Path.GetFileName(filePath);
                    byte[] content = File.ReadAllBytes(filePath);
                    Database.UpsertFile(name, content, authorId, orgId, parentId);
                }

                foreach (string subDir in Directory.GetDirectories(dirPath))
                {
                    string dirName = Path.GetFileName(subDir);
                    int folderId = Database.UpsertFolderRecord(dirName, authorId, orgId, parentId);
                    if (folderId >= 0)
                        queue.Enqueue((subDir, folderId));
                }
            }
        }

        /// <summary>
        /// Downloads all files for the organization from the database to the local AppData folder
        /// using a BFS walk of the parent hierarchy: root items (no parent) are written first,
        /// then their children, level by level, preserving the directory structure stored in the
        /// parent column.
        /// </summary>
        public static IReadOnlyList<string> DownloadDatabaseToFolder(int orgId)
        {
            string rootFolder = GetOrgDataFolder(orgId);
            var allRecords = Database.GetFilesForOrganization(orgId);

            // Group records by their parent ID. ILookup supports null keys (root records).
            var childrenOf = allRecords.ToLookup(f => f.Parent);

            // BFS queue: each entry is (record, local path of the parent directory).
            var queue = new Queue<(Models.Filez record, string parentPath)>();

            // Seed with root-level items — those that have no parent.
            foreach (var item in childrenOf[null])
                queue.Enqueue((item, rootFolder));

            var overwritten = new List<string>();

            while (queue.Count > 0)
            {
                var (record, parentPath) = queue.Dequeue();
                string localPath = Path.Combine(parentPath, record.Name.Trim());

                if (record.File == null || record.File.Length == 0)
                {
                    // Folder record — DB wins: if a file exists at this path, remove it first.
                    if (File.Exists(localPath))
                    {
                        overwritten.Add(localPath);
                        File.Delete(localPath);
                    }
                    Directory.CreateDirectory(localPath);
                    foreach (var child in childrenOf[(byte?)record.Id])
                        queue.Enqueue((child, localPath));
                }
                else
                {
                    // File record — track conflicts, then always overwrite with DB version.
                    // If a directory exists at this path, remove it first (DB wins).
                    if (Directory.Exists(localPath))
                    {
                        overwritten.Add(localPath);
                        Directory.Delete(localPath, recursive: true);
                    }
                    else if (File.Exists(localPath))
                    {
                        byte[] localBytes = File.ReadAllBytes(localPath);
                        if (!localBytes.SequenceEqual(record.File))
                            overwritten.Add(localPath);
                    }
                    Directory.CreateDirectory(parentPath);
                    File.WriteAllBytes(localPath, record.File);
                }
            }

            return overwritten;
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