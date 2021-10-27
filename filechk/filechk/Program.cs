using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace filechk
{
    class Program
    {
        static void Main(string[] args)
        {
            new app();
            Console.ReadLine();
        }
    }

    class app {
        public app() {
            //control();
            getDiffereces($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/testfolders/f1", 
                $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/testfolders/f2");
        }

        void control() {
            Console.WriteLine("[0] Scan doubles\n[1] View doubles");
            string ch = Console.ReadLine();

            if (ch == "0")
            {
                Console.Write("Directory 1: ");
                string dir1 = Console.ReadLine();
                Console.Write("Directory 2: ");
                string dir2 = Console.ReadLine();
                getDiffereces(dir1, dir2);
            }
            else if (ch == "1") { 
                
            }
        }

        rootData dira = new rootData();
        rootData dirb = new rootData();

        void getDiffereces(string roota, string rootb) {
            Console.WriteLine("Scanning dir 1");
            listDirs(roota, dira);
            Console.WriteLine("\nDone");
            Console.WriteLine("Scanning dir 2");
            listDirs(rootb, dirb);
            Console.WriteLine("\nDone");


        }

        List<DoubleFile> doubleFiles = new List<DoubleFile>(); 
        void getDoubles() {
            foreach (FileID f1 in dira.Files) {
                foreach (FileID f2 in dirb.Files) {
                    if (f1.Name == f2.Name && f1.Size == f2.Size) {  } // add double info !!!!!!!!!!!!!!!
                }
            }
        }

        #region Get files
        int tempCount = 0;
        void listDirs(string path, rootData stor) {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo f in di.GetFiles()) {
                tempCount++;
                Console.Write($"\rCounting files: {tempCount}");
                try { stor.Files.Add(new FileID(f.Name, f.FullName, f.Length)); } catch { stor.Errors.Add(f); }
            }
            foreach (DirectoryInfo d in di.GetDirectories()) {
                listDirs(d.FullName, stor);
            }
        }
        #endregion
    }

    public class rootData {
        public List<FileID> Files = new List<FileID>();
        public List<FileInfo> Errors = new List<FileInfo>();
    }

    public class FileID {
        public FileID(string name, string path, long size) { Size = size; Name = name; Path = path; }
        public long Size;
        public string Name;
        public string Path;
    }

    public class DoubleFile {
        List<string> DoublePaths = new List<string>();
    }
}
