namespace LostTech.IO.Links {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    class WindowsLinks: IFileSystemLinks {
        public void CreateFileSymlink(string symlink, string pointingTo)
            => CreateSymlinkInternal(symlink, pointingTo, LinkFlags.File);
        public void CreateDirectorySymlink(string symlink, string pointingTo)
            => CreateSymlinkInternal(symlink, pointingTo, LinkFlags.Directory);

        static LinkFlagsContainer? ExtraFlags;
        static void CreateSymlinkInternal(string symlink, string target, LinkFlags fileOrDirectory) {
            if (symlink == target) throw new ArgumentException("Source and Target are the same");

            var extraFlags = Volatile.Read(ref ExtraFlags);
            if (extraFlags == null) {
                var flags = fileOrDirectory | LinkFlags.AllowUnprivileged;
                if (CreateSymbolicLink(symlink, target, flags)) {
                    // remember we can use AllowUnprivileged
                    extraFlags = new LinkFlagsContainer {Value = LinkFlags.AllowUnprivileged};
                    Volatile.Write(ref ExtraFlags, extraFlags);
                    return;
                }

                int error = Marshal.GetLastWin32Error();

                if (CreateSymbolicLink(symlink, target, fileOrDirectory)) {
                    // remember we can't use AllowUnprivileged
                    extraFlags = new LinkFlagsContainer();
                    Volatile.Write(ref ExtraFlags, extraFlags);
                    return;
                }

                throw GetSymlinkError();
            }

            if (CreateSymbolicLink(symlink, target, fileOrDirectory | extraFlags.Value))
                return;

            throw GetSymlinkError();
        }

        static Exception GetSymlinkError() {
            int hResult = Marshal.GetHRForLastWin32Error();
            int error = Marshal.GetLastWin32Error();
            var inner = Marshal.GetExceptionForHR(hResult);
            return new IOException("Unable to create symbolic link", inner);
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool CreateSymbolicLink(string symlink, string target, LinkFlags flags);

        class LinkFlagsContainer { public LinkFlags Value; }
    }

    [Flags]
    enum LinkFlags: int
    {
        File = 0,
        Directory = 1,
        AllowUnprivileged = 2,
    }
}
