namespace LostTech.IO.Links {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    class UnixLinks: IFileSystemLinks {
        const int EINTR = 4;
        const int EEXIST = 17;

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
            if (symlink == target) throw new ArgumentException("Source and Target are the same");
            while (true) {
                if (UnixLinks.symlink(symlink: symlink, target: target) == 0)
                    return;

                int errno = Marshal.GetLastWin32Error();
                switch (errno) {
                case EINTR: continue;
                case EEXIST: throw new IOException("File already exists");
                }
                int hResult = HResultFromErrno(errno);
                Marshal.ThrowExceptionForHR(hResult);
            }
        }

        [DllImport("libc", SetLastError = true)]
        static extern int symlink(string target, string symlink);

        static int HResultFromErrno(int errno)
            => (errno & 0x80000000) == 0x80000000
                ? errno
                : (errno & 0x0000FFFF) | unchecked((int)0x80070000);
    }
}
