using Pastat.Reporting.Diagram;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pastat.Reporting
{
	public class History : IDisposable
	{
		private static readonly string FileName = BuildFileName();
		private static readonly string[] Headers =
			{
				"TimeStamp",
				"Total lines",
				"Code lines",
				"Comments",
				"Directives",
				"Empty lines",
				"Files"
			};

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
			_lines = new List<string> { string.Join(",", Headers) };
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

		public GraphData ExtractData()
		{
			var data = new GraphData();
			long minDate = 0;
			for (int i = 1; i < _lines.Count; i++)
			{
				LineParsingResult parsingResult = ParseLine(_lines[i]);
				if (i == 1)
					minDate = parsingResult.XValue.X;

				XValue xValue;
				Dictionary<string, long> headerToValue = new Dictionary<string, long>();
				for (int j = 0; j < parsingResult.Values.Length; j++)
				{
					string header = Headers[j];
					long value = parsingResult.Values[j];
					headerToValue[header] = value;
				}
				xValue = new XValue { X = parsingResult.XValue.X - minDate, Label = parsingResult.XValue.Label };
				data.Append(headerToValue, xValue);
			}
			return data;
		}

		private LineParsingResult ParseLine(string line)
		{
			string[] parts = line.Split(',');
			long[] parsed = new long[parts.Length - 1];
			string label = DateTime.Parse(parts[0], CultureInfo.InvariantCulture).ToShortDateString();
			long x = DateTime.Parse(parts[0], CultureInfo.InvariantCulture).Ticks;
			for (int i = 1; i < parts.Length; i++)
			{
				parsed[i - 1] = long.Parse(parts[i]);
			}
			return new LineParsingResult
			{
				XValue = new XValue
				{
					X = x,
					Label = label
				},
				Values = parsed
			};
		}

		// TODO: Move to separate class
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

		struct LineParsingResult
		{
			public XValue XValue { get; set; }
			public long[] Values { get; set; }
		}
	}
}
