﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Application {

	/// <summary>
	/// Parses a psv file with a header top row.
	/// </summary>
	class ParsePSV {

		/// <summary>
		/// Creates a table from a given file path and file stream.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		public static Table GetTable(string filePath) {
			var fileSession = new Session(filePath);
			var table = new Table(fileSession);
			table.TableName = filePath;
			table.SetTableHeaders(GetHeaders(fileSession));
			table.SetTableEntries(GetEntries(fileSession));
			return table;
		}

		/// <summary>
		/// Gets the header from the first row of the file stream.
		/// </summary>
		/// <param name="fileSession">The session which handles file access.</param>
		/// <returns>Returns a list of type string with the header entries.</returns>
		private static List<string> GetHeaders(Session fileSession) {
			return SplitOnPipe(fileSession.GetFirstLine());
		}

		/// <summary>
		/// Splits the given line through pipe delineation. 
		/// </summary>
		/// <param name="line">The line to parse.</param>
		/// <returns>Returns a parsed list of elements.</returns>
		private static List<string> SplitOnPipe(string line) {
			return line.Split('|')
				.Select(element => element.Trim())
				.ToList();
		}

		/// <summary>
		/// Gets the entries from a given stream starting at line 2 or position 1
		/// </summary>
		/// <param name="fileSession">The session manager which handles file access.</param>
		/// <returns>Returns a list of list, essentially the lines and their entries.</returns>
		private static List<List<string>> GetEntries(Session fileSession) {
			return fileSession.GetRestOfFile() 
				.Split(new string[] { Environment.NewLine } , StringSplitOptions.None)
				.Select(line => (List<string>) SplitOnPipe(line))
				.ToList();
		}
	}
}
