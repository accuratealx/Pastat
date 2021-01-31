using System.IO;

namespace PascalCodeStats
{
	partial class FileScanner
	{
		public FileStats Scan(string path)
		{
			FileStats stats = new FileStats(path);

			ScanState scanState = ScanState.None;
			var lines = File.ReadAllLines(path);
			stats.TotalLines = lines.Length;
			for (int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];

				if (string.IsNullOrWhiteSpace(line))
				{
					stats.Add(i, TokenType.EmptyLines);
					continue;
				}

				for(int j = 0; j < line.Length; j++)
				{
					char symbol = line[j];
					if (char.IsWhiteSpace(symbol))
						continue;

					switch (scanState)
					{
						case ScanState.None:
							if (DirectiveIsStarted(lines, i, j, out ScanState state))
							{
								scanState = state;
								stats.Add(i, TokenType.CompilerDirective);
								if (state == ScanState.CompilerDirectiveNonCurly)
									j++;
								continue;
							}
							if (CommentIsStarted(lines, i, j, out state))
							{
								scanState = state;
								stats.Add(i, TokenType.Comment);
								if (state == ScanState.CommentNonCurly)
									j++;
								continue;
							}
							if (SingleLineCommentStarted(lines, i, j))
							{
								stats.Add(i, TokenType.Comment);
								j = line.Length;
								continue;
							}
							if (symbol == '\'')
							{
								stats.Add(i, TokenType.Code);
								while (++j < line.Length && line[j] != '\'')
									;
								continue;
							}
							stats.Add(i, TokenType.Code);
							break;
						case ScanState.Code:
							if (DirectiveIsStarted(lines, i, j, out ScanState codeState))
							{
								scanState = codeState;
								stats.Add(i, TokenType.CompilerDirective);
								if (codeState == ScanState.CompilerDirectiveNonCurly)
									j++;
								continue;
							}
							if (CommentIsStarted(lines, i, j, out codeState))
							{
								scanState = codeState;
								stats.Add(i, TokenType.Comment);
								if (codeState == ScanState.CommentNonCurly)
									j++;
								continue;
							}
							if (SingleLineCommentStarted(lines, i, j))
							{
								stats.Add(i, TokenType.Comment);
								j = line.Length;
								continue;
							}
							if (symbol == '\'')
							{
								stats.Add(i, TokenType.Code);
								while (++j < line.Length && line[j] != '\'')
									;
								continue;
							}
							stats.Add(i, TokenType.Code);
							break;
						case ScanState.CompilerDirectiveCurly:
							stats.Add(i, TokenType.CompilerDirective);
							if (symbol == '}')
								scanState = ScanState.None;
							break;
						case ScanState.CompilerDirectiveNonCurly:
							stats.Add(i, TokenType.CompilerDirective);
							if (symbol == '*')
								if (j + 1 < line.Length && line[j+1] == ')')
								{
									j++;
									scanState = ScanState.None;
								}
							break;
						case ScanState.CommentCurly:
							stats.Add(i, TokenType.Comment);
							if (symbol == '}')
								scanState = ScanState.None;
							break;
						case ScanState.CommentNonCurly:
							stats.Add(i, TokenType.Comment);
							if (symbol == '*')
								if (j + 1 < line.Length && line[j + 1] == ')')
								{
									j++;
									scanState = ScanState.None;
								}
							break;
					}
				}
			}

			return stats;
		}

		private bool SingleLineCommentStarted(string[] lines, int lineNumber, int symbolNumber)
		{
			string line = lines[lineNumber];

			if (line[symbolNumber] == '/')
				if (symbolNumber + 1 < line.Length && line[symbolNumber + 1] == '/')
				{
					return true;
				}

			return false;
		}

		private bool DirectiveIsStarted(string[] lines, int lineNumber, int symbolNumber, out ScanState scanState)
		{
			scanState = ScanState.None;
			string line = lines[lineNumber];

			if (line[symbolNumber] == '{')
				if (symbolNumber + 1 < line.Length && line[symbolNumber + 1] == '$')
				{
					scanState = ScanState.CompilerDirectiveCurly;
					return true;
				}

			if (line[symbolNumber] == '(')
				if (symbolNumber + 1 < line.Length && line[symbolNumber + 1] == '*')
					if (symbolNumber + 2 < line.Length && line[symbolNumber + 2] == '$')
					{
						scanState = ScanState.CompilerDirectiveNonCurly;
						return true;
					}

			return false;
		}

		private bool CommentIsStarted(string[] lines, int lineNumber, int symbolNumber, out ScanState scanState)
		{
			scanState = ScanState.None;
			string line = lines[lineNumber];

			if (line[symbolNumber] == '{')
			{
				scanState = ScanState.CommentCurly;
				return true;
			}

			if (line[symbolNumber] == '(')
				if (symbolNumber + 1 < line.Length && line[symbolNumber + 1] == '*')
				{
					scanState = ScanState.CommentNonCurly;
					return true;
				}

			return false;
		}
	}
}
