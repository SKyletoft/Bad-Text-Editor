using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Text_Editor {
	class Program {

		static bool run = true;
		static List<List<char>> file = new List<List<char>> { new List<char> { } };
		static int line = 0;
		static int character = 0;
		static int topRow = 0;

		static String language = "meh";

		//Possible optimisation:
		//	sort the keyword lists by how common the terms are

		static String[] Ckeywords = {
			"auto",
			"else",
			"long",
			"switch",
			"break",
			"enum",
			"register",
			"typedef",
			"case",
			"extern",
			"return",
			"union",
			"char",
			"float",
			"short",
			"unsigned",
			"const",
			"for",
			"signed",
			"void",
			"continue",
			"goto",
			"sizeof",
			"volatile",
			"default",
			"if",
			"static",
			"while",
			"do",
			"int",
			"struct",
			"_Packed",
			"double"
		};

		static String[] CSkeywords = {
			"abstract",
			"as",
			"base",
			"bool",
			"break",
			"byte",
			"case",
			"catch",
			"char",
			"checked",
			"class",
			"const",
			"continue",
			"decimal",
			"default",
			"delegate",
			"do",
			"double",
			"else",
			"enum",
			"event",
			"explicit",
			"extern",
			"false",
			"finally",
			"fixed",
			"float",
			"for",
			"foreach",
			"goto",
			"if",
			"implicit",
			"in",
			"int",
			"interface",
			"internal",
			"is",
			"lock",
			"long",
			"namespace",
			"new",
			"null",
			"object",
			"operator",
			"out",
			"override",
			"params",
			"private",
			"protected",
			"public",
			"readonly",
			"ref",
			"return",
			"sbyte",
			"sealed",
			"short",
			"sizeof",
			"stackalloc",
			"static",
			"string",
			"struct",
			"switch",
			"this",
			"throw",
			"true",
			"try",
			"typeof",
			"uint",
			"ulong",
			"unchecked",
			"unsafe",
			"ushort",
			"using",
			"virtual",
			"void",
			"volatile",
			"while",
			"add",
			"alias",
			"ascending",
			"async",
			"await",
			"by",
			"descending",
			"dynamic",
			"equals",
			"from",
			"get",
			"global",
			"group",
			"into",
			"join",
			"let",
			"nameof",
			"on",
			"orderby",
			"partial",
			"remove",
			"select",
			"set",
			"value",
			"var",
			"when",
			"where",
			"yield"
		};

		static char[] endStatement = { ' ', '.', ',', ';', ':', '(', '{', '[' };

		static void Main (string[] args) {
			Console.WriteLine("Welcome!\n\n\nLoad or new file?\n(L/N)");
			while (true) {
				var input = Console.ReadKey(true).Key;
				if (input == ConsoleKey.L) {
					//LOAD
					//TODO
					break;
				} else if (input == ConsoleKey.N) {
					Console.WriteLine("\n\nWhat language?\nCurrently installed languages are:\nEnglish\nC\nC#");
					while (language == "meh") {
						var input2 = Console.ReadLine().ToLower();
						switch (input2) {
							case "english":
							case "eng":
							case "en":
								language = "en";
								break;
							case "c":
								language = "c";
								break;
							case "cs":
							case "c#":
								language = "cs";
								break;
							case "":
								Console.WriteLine("Blank? Disabling syntax highlighting");
								language = "en";
								System.Threading.Thread.Sleep(500);
								break;
							default:
								Console.WriteLine("That is not a valid option\nPlease state your language of choice clearly as written in the list");
								break;
						}
					}
					break;
				} else if (input == ConsoleKey.Q) {
					Console.WriteLine("Goodbye!");
					System.Threading.Thread.Sleep(1000);
					run = false;
				}
			}
			Render();
			while (run) {
				NextInput();
				Render();
			}
		}

		static void NextInput () {
			Console.SetCursorPosition(0, 0);
			var input = Console.ReadKey(true);
			switch (input.Key) {
				case ConsoleKey.Backspace:
					//while (true) {
					if (file[line].Count > 0 && character > 0) {
						file[line].RemoveAt(character - 1);
						character--;
						//break;
					} else {
						if (line != 0) {
							if (file[line].Count > 0) {
								file[line - 1].AddRange(file[line]);
							}
							file.RemoveAt(line);
							line--;
							character = file[line].Count;
							//break;
						}
					}
					//}
					break;
				case ConsoleKey.Escape:
					Save();
					break;
				case ConsoleKey.LeftArrow:
					if (character > 0) {
						character--;
					}
					break;
				case ConsoleKey.RightArrow:
					if (character < file[line].Count) {
						character++;
					}
					break;
				case ConsoleKey.UpArrow:
					if (line > 0) {
						line--;
					}
					if (line <= topRow) {
						topRow--;
						if (topRow < 0) {
							topRow = 0;
						}
					}
					if (character > file[line].Count) {
						character = file[line].Count;
					}
					break;
				case ConsoleKey.DownArrow:
					if (line < file.Count - 1) {
						line++;
					}
					if (line > Console.WindowHeight + topRow) {
						topRow++;
					}
					if (character > file[line].Count) {
						character = file[line].Count;
					}
					break;
				case ConsoleKey.Tab:
					file[line].AddRange(new List<char> { ' ', ' ', ' ', ' ' });
					character += 4;
					break;
				case ConsoleKey.Enter:
					List<List<char>> newList = file.GetRange(0, line + 1);
					newList.Add(new List<char> { });
					newList.AddRange(file.GetRange(line + 1, file.Count - (line + 1)));
					file = newList;
					if (character != file[line].Count) {
						file[line + 1] = file[line].GetRange(character, file[line].Count - character);
						file[line] = file[line].GetRange(0, character);
					}
					character = 0;
					if (line >= 0 && file[line].Count > 0) {
						int tempChar = 0;
						List<char> tempList = new List<char> { };
						while (file[line][tempChar] == ' ') {
							tempList.Add(' ');
							tempChar++;
							if (tempChar >= file[line].Count) {
								break;
							}
						}
						character = tempList.Count;
						tempList.AddRange(file[line + 1]);
						file[line + 1] = tempList;
					}
					line++;
					while (line + 2 > topRow + Console.WindowHeight) {
						topRow++;
					}
					break;
				default:
					if (character == file[line].Count) {
						file[line].Add(input.KeyChar);
					} else {
						List<char> newList2 = file[line].GetRange(0, character);
						newList2.Add(input.KeyChar);
						newList2.AddRange(file[line].GetRange(character, file[line].Count - character));
						file[line] = newList2;
					}
					character++;
					break;
			}
		}
		static void Render () {
			Console.Clear();
			int temp;
			var keywords = CSkeywords;
			switch (language) {
				case "en":
				default:
					keywords = new String[] { };
					break;
				case "c":
					keywords = Ckeywords;
					break;
				case "cs":
					keywords = CSkeywords;
					break;
			}
			for (var i = topRow; i < file.Count && i < Console.WindowHeight + topRow + 2; i++) {
				var endColor = -1;
				var isComment = false;
				var isInclude = false;
				var isString = false;
				var isChar = false;
				if (file[i].Count >= 1) {
					if (file[i][0] == '#') {
						isInclude = true;
					}
				}
				if (file[i].Count == 0 && i == line) {
					Console.Write('█');
				}
				for (var j = 0; j < file[i].Count && j < Console.WindowWidth; j++) {
					if (file[i][j] == '/') {
						if (j + 1 < file[i].Count) {
							if (file[i][j + 1] == '/' || file[i][j + 1] == '*') {
								isComment = true;
							}
						}
					}
					if (file[i][j] == '*') {
						if (j + 1 < file[i].Count) {
							if (file[i][j + 1] == '/') {
								isComment = false;
							}
						}
					}
					if (file[i][j] == '"') {
						isString = !isString;
					}
					if (file[i][j] == '\'') {
						isChar = !isChar;
					}
					if (j >= endColor) {
						if (j > 0) {
							if (endStatement.Contains(file[i][j - 1])) {
								colorManagment1(ref i, ref j, keywords, ref endColor);
							}
						} else {
							colorManagment1(ref i, ref j, keywords, ref endColor);
						}
					}
					if (language != "en") {
						if (isComment) {
							Console.ForegroundColor = ConsoleColor.Green;
						} else if (isString || isChar) {
							Console.ForegroundColor = ConsoleColor.Red;
						} else if (isInclude) {
							Console.ForegroundColor = ConsoleColor.DarkMagenta;
						} else if (endColor > j) {
							Console.ForegroundColor = ConsoleColor.Cyan;
						} else if (int.TryParse(file[i][j].ToString(), out temp)) {
							Console.ForegroundColor = ConsoleColor.DarkYellow;
						} else if (file[i][j] == '.') {
							Console.ForegroundColor = ConsoleColor.Yellow;
						} else {
							Console.ResetColor();
						}
					}
					if (character - 1 == j && line == i) {
						Console.Write(file[i][j]);
						//Console.ForegroundColor = ConsoleColor.White;
						Console.Write("█");
						//Console.ResetColor();
					} else {
						Console.Write(file[i][j]);
					}

				}
				Console.Write("\n");
			}
			System.Console.Title = line + ":" + character + ", " + topRow;
		}

		static void colorManagment1 (ref int i, ref int j, String[] keywords, ref int endColor) {
			for (var k = 0; k < keywords.Length; k++) {
				if (file[i].Count >= keywords[k].Length + j) {
					var thisPasses = true;
					for (var l = 0; l < keywords[k].Length; l++) {
						if (file[i][j + l] != keywords[k][l]) {
							thisPasses = false;
							break;
						}
					}
					if (thisPasses) {
						if (file[i].Count > j + keywords[k].Length) {
							if (!endStatement.Contains(file[i][j + keywords[k].Length])) {
								thisPasses = false;
							}
						}
					}
					if (thisPasses) {
						endColor = keywords[k].Length + j;
						break;
					}
				}
			}
		}

		static void Save () {
			ConsoleKey input;
			Console.Clear();
			Console.WriteLine("\nSave?\n(Y/n)");
			while (true) {
				input = Console.ReadKey(true).Key;
				if (input == ConsoleKey.Y) {
					String[] stringFile = new String[file.Count];
					for (var i = 0; i < file.Count; i++) {
						for (var j = 0; j < file[i].Count; j++) {
							stringFile[i] += file[i][j];
						}
					}
					Console.WriteLine("File name?");
					String adress = Console.ReadLine();
					String format = ".txt";
					if (!adress.Contains('.')) {
						adress += format;
					}
					System.IO.File.WriteAllLines(@adress, stringFile);
					break;
				} else if (input == ConsoleKey.N) {
					break;
				} else {
					Console.WriteLine("Not a valid option, try again.\nSave?\n(Y/n)");
				}
			}
			Console.Clear();
			Console.WriteLine("\n\nExit?\n(Y/n)");
			while (true) {
				input = Console.ReadKey(true).Key;
				if (input == ConsoleKey.Y) {
					Console.Clear();
					Console.WriteLine("\nGoodbye!");
					System.Threading.Thread.Sleep(1000);
					run = false;
					Console.Clear();
					break;
				} else if (input == ConsoleKey.N) {
					break;
				} else {
					Console.WriteLine("Not a valid option, try again.\nExit?\n(Y/n)");
				}
			}
		}
	}
}
