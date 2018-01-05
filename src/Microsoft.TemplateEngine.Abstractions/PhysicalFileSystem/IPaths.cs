using System.Collections.Generic;
using System.IO;

namespace Microsoft.TemplateEngine.Abstractions.PhysicalFileSystem
{
    public interface IPaths
    {
        IGlobalPaths Global { get; }

        IUserPaths User { get; }

        void Copy(string path, string targetPath);

        void CreateDirectory(string path);

        void CreateDirectory(string path, string parent);

        void Delete(string path);

        void Delete(string path, params string[] patterns);

        void Delete(string path, SearchOption searchOption, params string[] patterns);

        void DeleteDirectory(string path);

        void DeleteFile(string path);

        bool DirectoryExists(string path);

        IEnumerable<string> EnumerateDirectories(string path, string pattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        IEnumerable<string> EnumerateFiles(string path, string pattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        IEnumerable<string> EnumerateFileSystemEntries(string path, string pattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly);

        bool Exists(string path);

        bool FileExists(string path);

        string Name(string path);

        Stream OpenRead(string path);

        string ProcessPath(string path);

        byte[] ReadAllBytes(string path);

        string ReadAllText(string path, string defaultValue = "");

        string ToPath(string codebase);

        void WriteAllText(string path, string value);
    }
}
