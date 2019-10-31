using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Filemanager {
    class FM {
        string currentDirectory;
        int printHeight;
        int selectedItemIndex;
        int filesStartAt;
        string[] items;

        public FM (string cd) {
            currentDirectory = cd;
            printHeight = Console.WindowHeight - 4;
            selectedItemIndex = 0;
            UpdateItems();
            PrintFolder();
        }

        public void Run () {
            while (true) {
                TakeAction();
            }
        }

        void UpdateItems () {
            var localItems = new List<string> { ".." };
            localItems.AddRange(Directory.GetDirectories(currentDirectory));
            filesStartAt = localItems.Count;
            localItems.AddRange(Directory.GetFiles(currentDirectory));
            items = localItems.ToArray();
        }

        void Select () {
            if (selectedItemIndex == 0) {
                //UP A LEVEL
            } else if (selectedItemIndex < filesStartAt) {
                //OPEN DIRECTORY
                currentDirectory = items[selectedItemIndex];
                UpdateItems();
            } else {
                //Open file?
            }
        }

        void TakeAction () {
            switch (Console.ReadKey().Key) {
                case ConsoleKey.UpArrow:
                    selectedItemIndex--;
                    selectedItemIndex %= items.Length;
                    if (selectedItemIndex < 0) {
                        selectedItemIndex += items.Length;
                    }
                    PrintFolder();
                    break;
                case ConsoleKey.DownArrow:
                    selectedItemIndex++;
                    selectedItemIndex %= items.Length;
                    PrintFolder();
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.Enter:
                    Select();
                    //SELECTION
                    break;
                case ConsoleKey.LeftArrow:
                    selectedItemIndex = 0;
                    Select();
                    //MOVE UP IN FOLDER PATH
                    break;
                default:
                    break;
            }
        }
        void PrintFolder() {
            Console.Clear();
            Console.WriteLine("{0}\n", currentDirectory);
            var preCursor = new StringBuilder();
            var postCursor = new StringBuilder();
            var cursor = String.Empty;
            for (var i = 0; i < items.Length && i < printHeight; i++) {
                if (i < selectedItemIndex) {
                    preCursor.Append("\n");
                    preCursor.Append(items[i]);
                } else if (i > selectedItemIndex) {
                    postCursor.Append("\n");
                    postCursor.Append(items[i]);
                } else {
                    cursor = items[i];
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
        }
    }
}
