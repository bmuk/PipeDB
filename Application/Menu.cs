﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Application {

	static class Menu {
		public static void PresentMenu() {
			var options = new List<string>() {
				"1. Import Database",
				"2. Create New Database",
				"3. Exit The Program"
			};
			options.ForEach(option => Console.WriteLine(option));
			DisplayPrompt();
			var choice = Convert.ToInt32(Console.ReadLine());
			switch(choice) {
				case 1:
					ImportDatabase();
					break;
				case 2:
					//NewDatabase(); TODO
					break;
				case 3:
					Environment.Exit(0);
					break;
				default:
					PresentMenu();
					Console.WriteLine("Please enter a valid option.");
					break;
			}
		}

		private static void DisplayPrompt() {
			var prompt1 = "{ ~ }";
			var prompt2 = "  » ";
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(prompt1);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(prompt2);
			Console.ResetColor();
		}

		private static void ImportDatabase() {
			Console.Write("Please specify the location of your .psv file: ");
			var filename = Console.ReadLine();
			Table table;
			using(StreamReader psv = new StreamReader(filename)) {
				table = File.Exists(filename) ?
					ParsePSV.GetTable(psv , filename) :
					null;
			}
			if(table != null) {
				Shell.Repl(table);
			}
			else {
				ImportDatabase();
			}
		}
	}
}