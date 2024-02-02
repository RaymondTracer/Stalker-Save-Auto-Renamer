using System.Diagnostics;
using System.Xml;

namespace Stalker_Save_Auto_Renamer
{
    internal class Program
    {
        private static string SavePath = @"";

        private static bool Renaming = false;
        private static string StrDateTime = "ERROR";
        private static bool IsAutosave = false;

        static void Main(string[] args)
        {
            if (File.Exists("SavePath.txt"))
            {
                SavePath = File.ReadAllLines("SavePath.txt")[0];

                if (!Directory.Exists(SavePath))
                {
                    Console.WriteLine($"The directory \"{SavePath}\" doesn't exist.");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine($"Path: {SavePath}");
            }
            else
            {
                Console.WriteLine("Create a text document called \"SavePath.txt\" and paste your STALKER save folder location into it.");
                Console.ReadKey();
                return;
            }

            FileSystemWatcher watcher = new(SavePath, "*.*")
            {
                EnableRaisingEvents = true
            };

            watcher.Changed += (_, args) =>
            {
                if (!Renaming)
                {
                    string save = string.Empty;
                    if (args.Name.Length > 9 && (IsAutosave = (save = string.Concat(args.Name[^13..^4])).Equals("_autosave")) || (save = string.Concat(args.Name[^14..^4])).Equals("_quicksave"))
                    {
                        Task.Run(() => RenameFiles(save, args));
                    }
                }
            };

            Console.WriteLine("Ready");
            Task.Delay(-1).Wait();
        }

        private static void RenameFiles(string save, FileSystemEventArgs args)
        {
            if (Renaming)
            {
                return;
            }

            Renaming = true;

            Task.Delay(100).Wait();

            StrDateTime = $"{DateTime.Now:dd-MM-yy HH\\hmm\\mss\\s}";
            Console.WriteLine($"{StrDateTime} -> {char.ToUpper(save[1])}{string.Concat(save.Skip(2))} detected.");

            try
            {
                File.Copy(string.Concat([string.Concat(args.FullPath.Split(".")[^2]), $".sav"]), string.Concat([string.Concat(args.FullPath.Split(".")[^2]), $" {StrDateTime}.sav"]), true);

                if (!IsAutosave)
                {
                    File.Copy(string.Concat([string.Concat(args.FullPath.Split(".")[^2]), $".dds"]), string.Concat([string.Concat(args.FullPath.Split(".")[^2]), $" {StrDateTime}.dds"]), true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            StrDateTime = "ERROR";
            Renaming = false;
        }
    }
}
