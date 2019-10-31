using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Filemanager {
    class FM {
        string currentDirectory;
        int selectedItemIndex = 0;
        int filesStartAt;
        string[] items;
        int scroll = 0;
        string bufferPath;
        bool cut = false;

        const string controls = "Arrows: Move    Enter: Select    C: Copy    X: Move    V: Paste    D: Delete    R: Rename                                                                                                                                                                                   ";

        public FM (string cd) {
            currentDirectory = cd;
            UpdateItems();
        }

        public void Run () {
            Console.Clear();
            PrintFolder();
            while (true) {
                TakeAction();
            }
        }

        void UpdateItems () {
            var localItems = new List<string> { currentDirectory + "\\.." };
            localItems.AddRange(Directory.GetDirectories(currentDirectory));
            filesStartAt = localItems.Count;
            localItems.AddRange(Directory.GetFiles(currentDirectory));
            items = localItems.ToArray();
        }

        void Select () {
            if (selectedItemIndex == 0) {
                //UP A LEVEL
                if (currentDirectory.Length > 2) {
                    var lastIndex = currentDirectory.LastIndexOf('\\', currentDirectory.Length - 2);
                    if (lastIndex != -1) {
                        currentDirectory = currentDirectory.Substring(0, lastIndex);
                    }
                }
            } else if (selectedItemIndex < filesStartAt) {
                //OPEN DIRECTORY
                currentDirectory = items[selectedItemIndex];
            } else {
                //Open file?
            }
            if (!currentDirectory.EndsWith('\\')) {
                currentDirectory += '\\';
            }
            UpdateItems();
            selectedItemIndex = 0;
            scroll = 0;
        }

        void TakeAction () {

            var lowerTrigger = selectedItemIndex - Console.WindowHeight + 8;
            var lowerMax = Math.Min(items.Length - Console.WindowHeight + 3, items.Length);
            var upperMax = 0;
            var upperTrigger = selectedItemIndex - 1;

            switch (Console.ReadKey(true).Key) {
                case ConsoleKey.UpArrow:
                    selectedItemIndex--;
                    selectedItemIndex %= items.Length;
                    if (selectedItemIndex < 0) {
                        selectedItemIndex += items.Length;
                        scroll = lowerMax;
                        Console.Clear();
                    } else if (upperTrigger <= scroll) {
                        scroll = Math.Max(scroll - 10, upperMax);
                        Console.Clear();
                    }
                    PrintFolder();
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex++;
                    selectedItemIndex %= items.Length;
                    if (selectedItemIndex == 0) {
                        scroll = upperMax;
                        Console.Clear();
                    } else if (lowerTrigger > scroll) {
                        scroll = Math.Min(scroll + 10, lowerMax);
                        Console.Clear();
                    }
                    PrintFolder();
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.Enter:
                    Select();
                    Console.Clear();
                    PrintFolder();
                    //SELECTION
                    break;
                case ConsoleKey.LeftArrow:
                    selectedItemIndex = 0;
                    Select();
                    Console.Clear();
                    PrintFolder();
                    //MOVE UP IN FOLDER PATH
                    break;
                case ConsoleKey.X:
                    cut = true;
                    bufferPath = items[selectedItemIndex];
                    break;
                case ConsoleKey.C:
                    cut = false;
                    bufferPath = items[selectedItemIndex];
                    break;
                case ConsoleKey.V:
                    var index = bufferPath.LastIndexOf('\\');
                    var name = bufferPath.Substring(index);
                    var final = String.Concat(currentDirectory, name);
                    if (cut) {
                        File.Move(bufferPath, final);
                        bufferPath = String.Empty;
                        cut = false;
                    } else {
                        File.Copy(bufferPath, final);
                    }
                    UpdateItems();
                    PrintFolder();
                    break;
                case ConsoleKey.D:
                    if (selectedItemIndex != 0) {
                        if (selectedItemIndex < filesStartAt) {
                            var deleteFolderOk = Confirm("Are you sure you want to delete an entire folder?");
                            if (deleteFolderOk == 0) {
                                File.Delete(items[selectedItemIndex]);
                                selectedItemIndex--;
                            }
                        } else {
                            File.Delete(items[selectedItemIndex]);
                            selectedItemIndex--;
                        }
                    }
                    UpdateItems();
                    Console.Clear();
                    PrintFolder();
                    break;
                case ConsoleKey.R:
                    RenameFile();
                    PrintFolder();
                    break;
                default:
                    break;
            }
        }
        int Confirm (string reason) {
            Console.Clear();
            Console.WriteLine(reason);
            var answer = Console.ReadKey().Key;
            switch (answer) {
                case ConsoleKey.Y:
                    //REPLACE
                    return 0;
                case ConsoleKey.N:
                    return 1;
                default:
                    Console.WriteLine("Invalid answer, aborting");
                    return 2;
            }
        }
        void RenameFile () {
            Console.Clear();
            Console.Write("Rename file: {0}\n\nNew name:", items[selectedItemIndex]);
            var newName = Console.ReadLine();
            var newPath = String.Concat(currentDirectory, newName);
            var alreadyExists = File.Exists(newPath);
            if (alreadyExists) {
                switch (Confirm("\nDuplicate file name\nReplace? (Y/n)")) {
                    case 0:
                        File.Copy(items[selectedItemIndex], newPath);
                        File.Delete(items[selectedItemIndex]);
                        break;
                    case 1:
                        RenameFile();
                        break;
                    case 2:
                        return;
                }
            } else {
                File.Copy(items[selectedItemIndex], newPath);
                File.Delete(items[selectedItemIndex]);

            }
            UpdateItems();
        }
        void PrintFolder () {
            var ignoreScroll = Console.WindowHeight - 5 >= items.Length;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("\n{0}", currentDirectory);
            var preCursor = new StringBuilder();
            var postCursor = new StringBuilder();
            var cursor = String.Empty;
            for (var i = ignoreScroll ? 0 : scroll; i < items.Length && (ignoreScroll || i < (Console.WindowHeight - 3 + scroll)); i++) {
                if (i < selectedItemIndex) {
                    preCursor.Append("\n");
                    preCursor.Append(items[i].Substring(currentDirectory.Length));
                } else if (i > selectedItemIndex) {
                    postCursor.Append("\n");
                    postCursor.Append(items[i].Substring(currentDirectory.Length));
                } else {
                    cursor = items[i].Substring(currentDirectory.Length);
                }
            }
            Console.WriteLine(preCursor.ToString());
            var tmp = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = tmp;
            Console.Write(cursor);
            tmp = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = tmp;
            Console.WriteLine(postCursor.ToString());
            tmp = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = tmp;

            Console.SetCursorPosition(0, Console.WindowHeight);
            Console.Write(controls.Substring(0, Math.Min(controls.Length, Console.WindowWidth)));

            tmp = Console.ForegroundColor;
            Console.ForegroundColor = Console.BackgroundColor;
            Console.BackgroundColor = tmp;
        }
    }
}
