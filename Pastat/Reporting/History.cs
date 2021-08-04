using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PascalCodeStats.Reporting
{
	public class History : IDisposable
	{
		private static readonly string FileName = BuildFileName();
		private const string Header = "TimeStamp,Total lines,Code lines,Comments,Directives,Empty lines,Files";

		private string _fileName;
		private List<string> _lines;

		private History(string filename)
		{
			_fileName = filename;
			if (File.Exists(_fileName))
			{
				ReadContent();
			}
			else
			{
				CreateContent();
			}
		}

		private void ReadContent()
		{
			_lines = File.ReadAllLines(_fileName).ToList();
		}

		private void CreateContent()
		{
			_lines = new List<string> { Header };
		}

		public static History Load()
		{
			return new History(FileName);
		}

		public void Append(IStatsCollector s, int filesCount)
		{
			var timeStamp = DateTime.Now.ToString(CultureInfo.InvariantCulture);
			_lines.Add($"{timeStamp}" +
				$",{s.TotalLines}" +
				$",{s.CodeCount}" +
				$",{s.CommentsCount}" +
				$",{s.DirectivesCount}" +
				$",{s.EmptyLinesCount}" +
				$",{filesCount}");
		}

		private static string BuildFileName()
		{
			string projectName = Path.GetFileName(Environment.CurrentDirectory);
			byte[] hash = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(Environment.CurrentDirectory));

			string fileName = $"{projectName}_{ByteArrayToString(hash)}.csv";
			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string folder = Path.Combine(appDataPath, "Pastat");
			Directory.CreateDirectory(folder);
			return Path.Combine(folder, fileName);
		}

		private static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		public void Dispose()
		{
			File.WriteAllLines(_fileName, _lines);
		}
	}
}
