using System;
using System.Collections;
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
            var rootDir = @"I:\mp3";
            var directoryInfo = new DirectoryInfo(rootDir);

            var di2 = new DirectoryInfo($@"{rootDir}\Kiss\Dynasty");
            var albumFiles = GetFiles(di2, $@"{rootDir}\Kiss\Dynasty");
            foreach (var album in albumFiles)
            {
                Console.WriteLine(album.Name + "-" + album.FullName);
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();

            var files = WalkDirectoryTree(directoryInfo, rootDir, "c");
            
            var i = 0;
            foreach (var fileInfo in files)
            {
                //Console.WriteLine(fileInfo.FullName + " - " + fileInfo.DirectoryName + " = " + fileInfo.Extension);
                Console.WriteLine(i++ + " - " + fileInfo.Name);
            }

            Console.WriteLine("Files with restricted access:");
            foreach (var s in Log)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static List<FileInfo> GetFiles(DirectoryInfo directoryInfo, string rootDir)
        {
            FileInfo[] files = null;
            var result = new List<FileInfo>(); ;

            // First, process all the files directly under this folder
            try
            {
                files = directoryInfo.GetFiles("*.*");
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
                .Where(fi => fi.Extension.ToUpper() == ".MP3"));

            return result;
        }

        private static IEnumerable<DirectoryInfo> GetDirectories(DirectoryInfo root)
        {

            // Now find all the subdirectories under this directory.
            var subDirs = root.GetDirectories().ToList();

            return subDirs;
        }

        private static List<FileInfo> WalkDirectoryTree(DirectoryInfo directoryInfo, string rootDir, string startsWith, List<FileInfo> fileInfos = null)
        {
            FileInfo[] files = null;          
            if (fileInfos == null)
                fileInfos = new List<FileInfo>();
            var result = fileInfos;

            // First, process all the files directly under this folder
            try
            {
                files = directoryInfo.GetFiles("*.*");
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
                .StartsWith(rootDir.ToUpper() + @"\" + startsWith.ToUpper()) && 
                fi.Extension.ToUpper() == ".MP3"));

            // Now find all the subdirectories under this directory.
            var subDirs = directoryInfo.GetDirectories();

            foreach (var dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                WalkDirectoryTree(dirInfo, rootDir, startsWith, result);
            }

            return result;
        }
    }
}
