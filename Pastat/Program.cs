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

            Console.WriteLine("Pastat v1.0 (23.01.2021)  <Cherry Programmer. medulla_261@mail.ru>");
            Console.WriteLine("Special for Simple Game Engine");

            string currentDir = Directory.GetCurrentDirectory();
			string[] filePaths = EnumerateFilesRecursively(currentDir).ToArray();

            if (filePaths.Length > 0)
            {
                Console.WriteLine();
                Console.WriteLine($"Всего строк | Код     | Комментарии | Директивы | Пустых | Имя файла");
                Console.WriteLine($"------------+---------+-------------+-----------+--------+---------------------------------------------------");
                FileScanner scanner = new FileScanner();
                TotalStatsCollector statsCollector = new TotalStatsCollector();
                foreach (string filePath in filePaths)
                {
                    var stats = scanner.Scan(filePath);
                    statsCollector.Add(stats);
                    Console.WriteLine(ReadStatsLine(stats));
                }

                Console.WriteLine($"------------+---------+-------------+-----------+--------+---------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine($"Всего в ({filePaths.Length}) файлах:");
                Console.WriteLine($"  Строк      : {statsCollector.TotalLines}");
                Console.WriteLine($"  Код        : {statsCollector.CodeCount}");
                Console.WriteLine($"  Комментарии: {statsCollector.CommentsCount}");
                Console.WriteLine($"  Директивы  : {statsCollector.DirectivesCount}");
                Console.WriteLine($"  Пустых     : {statsCollector.EmptyLinesCount}");
            }
            else
            {
                Console.WriteLine("файлов не найдено");
            }

            Console.ReadKey();
        }

		private static string ReadStatsLine(FileStats stats)
		{
			return $"{stats.TotalLines, -12}" +
				$"| {stats.CodeCount, -8}" +
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
