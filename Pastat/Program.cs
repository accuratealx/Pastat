using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PascalCodeStats
{
	class Program
	{
		private static readonly HashSet<string> FilesToSearch = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			".pas", ".inc", ".pp", ".lpr", ".dpr"
		};

		static void Main(string[] args)
		{
			Console.WindowWidth = 110;
			Console.BufferWidth = 110;
			Console.WindowHeight = 50;

			Console.WriteLine("Pastat v1.1 23.01.2021  [Cheery Programmer   medulla_261@mail.ru]");
			Console.WriteLine("Special for Simple Game Engine");

			string currentDir = Directory.GetCurrentDirectory();
			string[] filePaths = EnumerateFilesRecursively(currentDir).ToArray();

			if (filePaths.Length > 0)
			{
				string tableline = $"------------+---------+--------------+-----------+--------+--------------------------------------------------";
				string tablehead = $"Строки      | Код     | Комментарии  | Директивы | Пустые | Имя файла";
				string tableheadtotal = $"Cтрок       | Кода    | Комментариев | Директив  | Пустых | Файлов";

				//Таблица
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine(tableline);
				Console.WriteLine(tablehead);
				Console.WriteLine(tableline);
				FileScanner scanner = new FileScanner();
				TotalStatsCollector statsCollector = new TotalStatsCollector();
				foreach (string filePath in filePaths)
				{
					var stats = scanner.Scan(filePath);
					statsCollector.Add(stats);
					Console.WriteLine(ReadStatsLine(stats));
				}
				Console.WriteLine(tableline);

				//Итого
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine("                                                    Всего:");
				Console.WriteLine(tableline);
				Console.WriteLine(tableheadtotal);
				Console.WriteLine(tableline);
				Console.WriteLine($"{statsCollector.TotalLines,-12}" +
								  $"| {statsCollector.CodeCount,-8}" +
								  $"| {statsCollector.CommentsCount,-13}" +
								  $"| {statsCollector.DirectivesCount,-10}" +
								  $"| {statsCollector.EmptyLinesCount,-7}" +
								  $"| {filePaths.Length}");
				Console.WriteLine(tableline);
			}
			else
			{
				Console.WriteLine();
				Console.WriteLine("файлов не найдено");
			}

			Console.ReadKey();
		}

		private static string ReadStatsLine(FileStats stats)
		{
			return $"{stats.TotalLines,-12}" +
				$"| {stats.CodeCount,-8}" +
				$"| {stats.CommentsCount,-13}" +
				$"| {stats.DirectivesCount,-10}" +
				$"| {stats.EmptyLinesCount,-7}" +
				$"| {Path.GetFileName(stats.FullPath)}";
		}

		static IEnumerable<string> EnumerateFilesRecursively(string path)
		{
			return FilesToSearch
				.Select(ext => $"*{ext}")
				.SelectMany(pattern => Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories))
				.Where(filePath => FilesToSearch.Contains(Path.GetExtension(filePath)));
		}
	}
}

