namespace LostTech.IO.Links {
    public interface IFileSystemLinks {
        void CreateFileSymlink(string symlink, string pointingTo);
        void CreateDirectorySymlink(string symlink, string pointingTo);
    }
}
