using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PZOAutoUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PZOAutoUpdater";
            if (!Directory.Exists("Patches"))
            {
                Console.WriteLine("Patches directory not found. Please create a patches directory and extract all patches to that directory.");
                Console.ReadLine();
                return;
            }

            string[] patches = Directory.GetDirectories("Patches");

            string bestPatch = string.Empty;
            int patchNum = 0;

            foreach (string patch in patches)
            {
                int value = extractNum(patch);

                if (value > patchNum)
                {
                    patchNum = value;
                    bestPatch = patch;
                }
                
            }

            if (patchNum <= 0)
            {
                Console.WriteLine("No valid patches found.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(string.Format("Newest patch found is {0}. Install? y/n", patchNum));
            if (Console.ReadLine() != "y")
            {
                return;
            }

            Console.WriteLine("Copying from : " + bestPatch);

            StringBuilder errors = new StringBuilder();

            if (File.Exists(bestPatch + "\\Game.rgssad"))
            {
                if (File.Exists("Game.rgssad"))
                    File.Delete("Game.rgssad");
                File.Copy(bestPatch + "\\Game.rgssad", "Game.rgssad");
            }
            else
                errors.AppendLine("Game.rgssad not found : patch may not be downloaded properly");

            string[] directoriesToCopy = Directory.GetDirectories(bestPatch);

            foreach (string directory in directoriesToCopy)
            {                
                string dir = directory.Remove(0, Path.GetDirectoryName(directory).Length + 1);
                
                if (dir.StartsWith("Copy into "))
                {
                    string str = dir.Remove(0, 10);
                    str = str.Replace('_', '\\');
                    str = str.Replace(':', '\\');

                    string copyPath = str + "\\";

                    if (!Directory.Exists(str))
                    {
                        errors.AppendLine("Couldn't find directory : \"" + str + "\". Files not copied");
                        continue;
                    }

                    Console.WriteLine("Copying to : " + copyPath);


                    string[] files = Directory.GetFiles(directory);

                    foreach (string file in files)
                    {
                        string copyTo = copyPath + Path.GetFileName(file);
                        if (File.Exists(copyTo))
                            File.Delete(copyTo);
                        File.Copy(file, copyTo);
                    }
                }
            }



            Console.WriteLine("Completed! Any errors encountered will be listed below.");
            Console.WriteLine(errors);
            Console.ReadLine();


           
        }

        static int extractNum(string s)
        {
            string num = string.Empty;
            int val = 0;

            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsDigit(s[i]))
                    num += s[i];
            }

            if (num.Length > 0)
                val = int.Parse(num);
            return val;
        }
    }
}
