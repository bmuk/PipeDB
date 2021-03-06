﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Application {

	/// <summary>
	/// Generic data class for data entry.
	/// </summary>
	class Table {

		/// <summary>
		/// Session used to access the persistance file.
		/// </summary>
		private Session FileSession;

		/// <summary>
		/// Constructor; Takes a session used to manage file access. 
		/// </summary>
		/// <param name="fileSession"></param>
		/// <returns></returns>
		public Table(Session fileSession) {
			this.FileSession = fileSession;
		}

		/// <summary>
		/// Name of the database or file name.
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// Header or column names for the database.
		/// </summary>
		public List<string> Headers { get; private set; }

		/// <summary>
		/// Entries under the header.
		/// </summary>
		public List<List<string>> Entries { get; private set; }

		/// <summary>
		/// Sets database or file headers. 
		/// </summary>
		/// <param name="headers"></param>
		public void SetTableHeaders(List<string> headers) {
			// Just for my sanity atm...
			this.Headers = headers.Select(h => h.ToLower()).ToList();
		}

		/// <summary>
		/// Sets the table entries, essentially a lines and their entries.
		/// </summary>
		/// <param name="entries">The list of list or lines of entries.</param>
		public void SetTableEntries(List<List<string>> entries) {
			// again... making it easier... deadlines.
			this.Entries = entries.Select(e => e.Select(v => v.ToLower()).ToList()).ToList();
		}

		/// <summary>
		/// Adds a row to the table; must be the same length as the headers.
		/// Returns success status.
		/// </summary>
		/// <param name="newRow"></param>
		/// <returns></returns>
		public bool Create(List<string> newRow) {
			if (newRow.Count != this.Headers.Count) {
				return false;
			}
			this.Entries.Add(newRow);
			Sync();
			return true;
		}

		/// <summary>
		/// Returns all the rows which match the given query delegate.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public List<List<string>> Read(int column = -1 , Predicate<List<string>> query = null) {
			if (query == null) {
				query = _ => true;
			}
			var rowMatches = this.Entries.FindAll(query);
			if (column == -1) {
				return rowMatches.ToList();
			}
			else {
				// looks hairy but I need to wrap the inside values in a list.
				return rowMatches.Select(r => Enumerable.Repeat(r[column] , 1).ToList()).ToList();
			}
		}

		/// <summary>
		/// Modifies the rows which match the given query delegate.
		/// Unlike read, this is required because it's very rare to want to change
		/// every column to a single value globally.
		/// </summary>
		/// <param name="diff"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public void Update(Tuple<int , string> diff , Predicate<List<string>> query) {
			var matchingRows = Read(-1 , query);
			foreach (var row in matchingRows) {
				var index = this.Entries.IndexOf(row);
				row[diff.Item1] = diff.Item2;
				this.Entries[index] = row;
			}
			Sync();
		}

		/// <summary>
		/// Deletes rows which match a query.
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public void Delete(Predicate<List<string>> query) {
			this.Entries.RemoveAll(query);
			Sync();
		}

		/// <summary>
		/// Syncs the changes to the file system.
		/// </summary>
		private void Sync() {
			this.FileSession.WriteTable(this);
		}
	}
}
