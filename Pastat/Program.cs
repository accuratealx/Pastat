using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PascalCodeStats
{
	class Program
	{
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
                string tableline =      $"------------+---------+--------------+-----------+--------+--------------------------------------------------";
                string tablehead =      $"Строки      | Код     | Комментарии  | Директивы | Пустые | Имя файла";
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
                Console.WriteLine($"{statsCollector.TotalLines, -12}" +
                                  $"| {statsCollector.CodeCount, -9}" +
                                  $"| {statsCollector.CommentsCount, -12}" +
                                  $"| {statsCollector.DirectivesCount, -10}" +
                                  $"| {statsCollector.EmptyLinesCount, -7}" +
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
			return $"{stats.TotalLines, -12}" +
				$"| {stats.CodeCount, -9}" +
				$"| {stats.CommentsCount, -12}" +
				$"| {stats.DirectivesCount, -10}" +
                $"| {stats.EmptyLinesCount, -7}" +
                $"| {Path.GetFileName(stats.FullPath)}";
		}

		static IEnumerable<string> EnumerateFilesRecursively(string path)
		{
			foreach (string filePath in EnumerateFiles(path))
                yield return filePath;



			foreach(string dirPath in Directory.EnumerateDirectories(path))
			{
				foreach (string nestedFile in EnumerateFilesRecursively(dirPath))
					yield return nestedFile;
			}
		}

        static IEnumerable<string> EnumerateFiles(string path)
        {
            return Directory.EnumerateFiles(path, "*.pas")
                .Concat(Directory.EnumerateFiles(path, "*.inc"))
                .Concat(Directory.EnumerateFiles(path, "*.pp"))
                .Concat(Directory.EnumerateFiles(path, "*.lpr"))
                .Concat(Directory.EnumerateFiles(path, "*.dpr"));
        }

    }
}

