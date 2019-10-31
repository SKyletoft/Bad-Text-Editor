using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace Filemanager {
    class Program {
        static void Main (string[] args) {
            Console.CursorVisible = false;
            var thisFM = new FM(@"C:\Users\skyle\");
            thisFM.Run();
        }
    }
}
