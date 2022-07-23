using System.Collections.Generic;
using System.IO;

namespace Cogworks.UmbracoFlare.Core.Services
{
    public interface IUmbracoFlareFileService
    {
        IEnumerable<FileInfo> GetFiles(string folder);
        IEnumerable<FileInfo> GetFilesIncludingSubDirs(string path);
        IEnumerable<DirectoryInfo> GetFolders(string folder);
    }
}