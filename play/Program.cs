namespace LostTech.IO.Links {
    using System.IO;

    static class Program {
        static void Main() {
            Directory.CreateDirectory("linkSource");
            if (Directory.Exists("link"))
                Directory.Delete("link");

            Symlink.CreateForDirectory(directoryPath: "linkSource", symlink: "link");
        }
    }
}
