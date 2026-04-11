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
        public static void DownloadDatabaseToFolder(int orgId)
        {
            string rootFolder = GetOrgDataFolder(orgId);
            var allRecords = Database.GetFilesForOrganization(orgId);

            // Group records by their parent ID so we can efficiently look up children.
            var childrenOf = allRecords
                .GroupBy(f => f.Parent)
                .ToDictionary(g => g.Key, g => g.ToList());

            // BFS queue: each entry is (record, local path of the parent directory).
            var queue = new Queue<(Models.Filez record, string parentPath)>();

            // Seed with root-level items — those that have no parent.
            if (childrenOf.TryGetValue(null, out var rootItems))
            {
                foreach (var item in rootItems)
                    queue.Enqueue((item, rootFolder));
            }

            while (queue.Count > 0)
            {
                var (record, parentPath) = queue.Dequeue();
                string localPath = Path.Combine(parentPath, record.Name.Trim());

                if (record.File == null || record.File.Length == 0)
                {
                    // Folder record — create the directory and enqueue its children.
                    Directory.CreateDirectory(localPath);
                    if (childrenOf.TryGetValue(record.Id, out var children))
                    {
                        foreach (var child in children)
                            queue.Enqueue((child, localPath));
                    }
                }
                else
                {
                    // File record — ensure parent directory exists and write the file.
                    Directory.CreateDirectory(parentPath);
                    File.WriteAllBytes(localPath, record.File);
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