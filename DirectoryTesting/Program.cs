using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DirectoryTesting
{
    class Program
    {
        private static readonly System.Collections.Specialized.StringCollection Log = new System.Collections.Specialized.StringCollection();

        static void Main()
        {          
            var startingDir = new DirectoryInfo(@"I:\mp3");
            var files = WalkDirectoryTree(startingDir);

            foreach (var fileInfo in files)
            {
                Console.WriteLine(fileInfo.FullName + " - " + fileInfo.DirectoryName + " = " + fileInfo.Extension);
            }

            Console.WriteLine("Files with restricted access:");
            foreach (var s in Log)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static List<FileInfo> WalkDirectoryTree(DirectoryInfo root, List<FileInfo> fileInfos = null)
        {
            FileInfo[] files = null;          
            if (fileInfos == null)
                fileInfos = new List<FileInfo>();
            var result = fileInfos;

            // First, process all the files directly under this folder
            try
            {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e)
            {
                // This is thrown if even one of the files requires permissions greater
                // than the application provides.
                Log.Add(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            if (files == null)
                return null;

            result.AddRange(files
                .Where(fi => fi.DirectoryName != null)
                .Where(fi => fi.DirectoryName.ToUpper()
                .StartsWith(@"I:\MP3\S") && 
                fi.Extension.ToUpper() == ".MP3"));

            // Now find all the subdirectories under this directory.
            var subDirs = root.GetDirectories();

            foreach (var dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                WalkDirectoryTree(dirInfo, result);
            }

            return result;
        }
    }
}
