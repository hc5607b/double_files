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
            control();
            //getDiffereces($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/testfolders/f2", $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/testfolders/f1");
            //getDiffereces($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/testfolders/f1"); 
            //getDiffereces(@"F:\Images");
        }

        void control() {
            while (true)
            {
                Console.WriteLine("[0] Scan one dir\n[1] Scan 2 dirs\n[2] View doubles\n[3] View stats\n[4] Save results\n[5] Load results");
                string ch = Console.ReadLine();

                if (ch == "0")
                {
                    Console.Write("Directory to scan: ");
                    string dir1 = Console.ReadLine();
                    if (Directory.Exists(dir1)) { getDiffereces(dir1); }
                }
                else if (ch == "1")
                {
                    Console.Write("Directory 1: ");
                    string dir1 = Console.ReadLine();
                    Console.Write("Directory 2: ");
                    string dir2 = Console.ReadLine();
                    if (Directory.Exists(dir1) && Directory.Exists(dir2)) { getDiffereces(dir1, dir2); }
                }
                else if (ch == "2")
                {
                    if (doubleFiles.Count != 0)
                    {
                        printDoubles();
                    }
                }
                else if (ch == "3")
                {
                    printStats();
                }
                else if (ch == "4")
                {
                    Console.Write("Dir to save (blank is default): ");
                    saveRes(Console.ReadLine());
                }
                else if (ch == "5")
                {
                    loadRes();
                }
            }
        }

        void saveRes(string dir = "") {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (dir != "" && Directory.Exists(dir)) { path = dir; }
            string fileName = $"scanresult_{DateTime.Now}";
        }
        void loadRes() { 
            
        }

        void printStats() {
            string dirSacnTemp = dirsScanned[0];
            if (dirsScanned[1] != null) { dirSacnTemp += $", {dirsScanned[1]}"; }
            Console.WriteLine($"Dir(s) scanned: {dirSacnTemp}\nFiles scanned: {tempCount}\nDoubles: {doubleFiles.Count}\nScan time: {scanTime}");
        }

        void printDoubles()
        {
            foreach (DoubleFile df in doubleFiles)
            {
                Console.WriteLine($"----------------------------------------------------------------------------------------\n{df.File}\n");
                foreach (string s in df.DoublePaths) { Console.WriteLine($"\t{s}"); }
            }
            Console.WriteLine($"----------------------------------------------------------------------------------------");
            Console.WriteLine($"{doubleFiles.Count} doubles found");
        }

        TimeSpan scanTime = new TimeSpan();
        rootData dira = new rootData();
        rootData dirb = new rootData();
        string[] dirsScanned = new string[2];

        void initScan()
        {
            tempCount = 0;
            scanTime = new TimeSpan();
            dira = new rootData();
            dirb = new rootData();
            doubleFiles.Clear();
            dirsScanned = new string[2];
        }

        void getDiffereces(string roota)
        {
            initScan();
            dirsScanned[0] = roota;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Scanning dir 1");
            listDirs(roota, dira);
            Console.WriteLine("\nDone");

            getDoubles();
            sw.Stop();
            scanTime = sw.Elapsed;
        }
        void getDiffereces(string roota, string rootb)
        {
            initScan();
            dirsScanned[0] = roota;
            dirsScanned[1] = rootb;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Scanning dir 1");
            listDirs(roota, dira);
            Console.WriteLine("\nDone");
            Console.WriteLine("Scanning dir 2");
            listDirs(rootb, dirb);
            Console.WriteLine("\nDone");

            getDoubles(false);
            sw.Stop();
            scanTime = sw.Elapsed;
        }

        List<DoubleFile> doubleFiles = new List<DoubleFile>(); 
        void getDoubles(bool onefolder = true) {
            foreach (FileID f1 in dira.Files) {
                DoubleFile df = new DoubleFile();
                df.File = f1.Path;
                if (onefolder)
                {
                    foreach (FileID f2 in dira.Files)
                    {
                        if (f1.Name == f2.Name && f1.Size == f2.Size && f1.Path != f2.Path && !isMarked(f2.Path)) { df.DoublePaths.Add(f2.Path); }
                    }
                }
                else
                {
                    foreach (FileID f2 in dirb.Files)
                    {
                        if (f1.Name == f2.Name && f1.Size == f2.Size) { df.DoublePaths.Add(f2.Path); }
                    }
                }
                if (df.DoublePaths.Count != 0) { doubleFiles.Add(df); }
            }
        }

        bool isMarked(string path) {
            foreach (DoubleFile df in doubleFiles) {
                if (df.File == path) { return true; }
                foreach (string s in df.DoublePaths) {
                    if (s == path) { return true; }
                }
            }
            return false;
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
        public string File;
        public List<string> DoublePaths = new List<string>();
    }
}
