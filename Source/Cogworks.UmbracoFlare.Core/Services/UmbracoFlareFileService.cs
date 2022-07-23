using Cogworks.UmbracoFlare.Core.Extensions;
using Cogworks.UmbracoFlare.Core.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Umbraco.Cms.Core.Hosting;

namespace Cogworks.UmbracoFlare.Core.Helpers
{
    public class UmbracoFlareFileService : IUmbracoFlareFileService
    {
        private readonly IEnumerable<string> ExcludedPaths = new List<string> { "app_data", "app_browsers", "app_data", "app_code", "app_plugins", "properties", "bin", "config", "media", "obj", "umbraco", "views" };
        private readonly IEnumerable<string> ExcludedExtensions = new List<string> { ".config", ".asax", ".user", ".nuspec", ".dll", ".pdb", ".lic", ".csproj" };
        private readonly IHostingEnvironment hostingEnvironment;

        public UmbracoFlareFileService(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        public IEnumerable<DirectoryInfo> GetFolders(string folder)
        {
            var path = hostingEnvironment.MapPathWebRoot("~/" + folder.TrimStart('~', '/'));
            var directory = new DirectoryInfo(path);

            if (!directory.Exists)
            {
                return Enumerable.Empty<DirectoryInfo>();
            }

            var directories = directory.EnumerateDirectories().Where(x => !ExcludedPaths.Contains(x.Name.ToLowerInvariant())).ToList();
            var allowedDirectories = directories.Where(x => x.EnumerateFiles().Any(f => !ExcludedExtensions.Contains(f.Extension)));

            return allowedDirectories;
        }

        public IEnumerable<FileInfo> GetFiles(string folder)
        {
            var path = hostingEnvironment.MapPathWebRoot("~/" + folder.TrimStart('~', '/'));
            var directory = new DirectoryInfo(path);

            if (!directory.Exists)
            {
                return Enumerable.Empty<FileInfo>();
            }

            var files = directory.EnumerateFiles().Where(f => !ExcludedExtensions.Contains(f.Extension));

            return files;
        }

        public IEnumerable<FileInfo> GetFilesIncludingSubDirs(string path)
        {
            var queue = new Queue<string>();
            queue.Enqueue(path);

            while (queue.Count > 0)
            {
                path = queue.Dequeue();

                foreach (var subDir in Directory.GetDirectories(path))
                {
                    queue.Enqueue(subDir);
                }

                var files = new DirectoryInfo(path).GetFiles();
                if (!files.HasValue()) { continue; }

                foreach (var file in files)
                {
                    yield return file;
                }
            }
        }
    }
}