namespace LostTech.IO.Links {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    class UnixLinks: IFileSystemLinks {
        const int EINTR = 4;

        public void CreateFileSymlink(string symlink, string pointingTo) {
            if (symlink == null) throw new ArgumentNullException(nameof(symlink));
            if (pointingTo == null) throw new ArgumentNullException(nameof(pointingTo));

            if (!File.Exists(pointingTo))
                throw new FileNotFoundException(new FileNotFoundException().Message, pointingTo);

            CreateSymlinkInternal(symlink, target: pointingTo);
        }

        public void CreateDirectorySymlink(string symlink, string pointingTo) {
            if (symlink == null) throw new ArgumentNullException(nameof(symlink));
            if (pointingTo == null) throw new ArgumentNullException(nameof(pointingTo));

            if (!Directory.Exists(pointingTo))
                throw new DirectoryNotFoundException();

            CreateSymlinkInternal(symlink, target: pointingTo);
        }

        static void CreateSymlinkInternal(string symlink, string target) {
            int errno;
            do {
                if (UnixLinks.symlink(symlink: symlink, target: target))
                    return;

                errno = Marshal.GetLastWin32Error();
                if (errno == EINTR) continue;
                int hResult = Marshal.GetHRForLastWin32Error();
                Marshal.ThrowExceptionForHR(hResult);
            } while (errno == EINTR);
        }

        [DllImport("libc", SetLastError = true)]
        static extern bool symlink(string target, string symlink);
    }
}
