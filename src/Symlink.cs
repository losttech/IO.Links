namespace LostTech.IO.Links {
    using System.Runtime.InteropServices;

    public static class Symlink {
        static readonly IFileSystemLinks Impl =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? (IFileSystemLinks)new WindowsLinks()
                : new UnixLinks();

        /// <summary>
        /// Create a symbolic link, pointing to the specified directory
        /// </summary>
        /// <param name="directoryPath">Which directory the symlink will point to</param>
        /// <param name="symlink">Where the symlink will be created</param>
        public static void CreateForDirectory(string directoryPath, string symlink)
            => Impl.CreateDirectorySymlink(symlink: symlink, pointingTo: directoryPath);

        /// <summary>
        /// Create a symbolic link, pointing to the specified file
        /// </summary>
        /// <param name="filePath">Which file the symlink will point to</param>
        /// <param name="symlink">Where the symlink will be created</param>
        public static void CreateForFile(string filePath, string symlink)
            => Impl.CreateFileSymlink(symlink: symlink, pointingTo: filePath);
    }
}
