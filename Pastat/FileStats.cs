using System.Collections.Generic;

namespace Pastat
{
	class FileStats
	{
		private Dictionary<TokenType, HashSet<int>> _tokenTypeToLineNumbers;
		public string FullPath { get; private set; }
		public int TotalLines { get; internal set; }
		public int CodeCount => _tokenTypeToLineNumbers.ContainsKey(TokenType.Code) ? _tokenTypeToLineNumbers[TokenType.Code].Count : 0;
		public int CommentsCount => _tokenTypeToLineNumbers.ContainsKey(TokenType.Comment) ? _tokenTypeToLineNumbers[TokenType.Comment].Count : 0;
		public int DirectivesCount => _tokenTypeToLineNumbers.ContainsKey(TokenType.CompilerDirective) ? _tokenTypeToLineNumbers[TokenType.CompilerDirective].Count : 0;
		public int EmptyLinesCount => _tokenTypeToLineNumbers.ContainsKey(TokenType.EmptyLines) ? _tokenTypeToLineNumbers[TokenType.EmptyLines].Count : 0;

		public FileStats(string path)
		{
			FullPath = path;
			_tokenTypeToLineNumbers = new Dictionary<TokenType, HashSet<int>>();
		}

		internal void Add(int lineNumber, TokenType tokenType)
		{
			if(_tokenTypeToLineNumbers.TryGetValue(tokenType, out HashSet<int> lineNumbers))
			{
				lineNumbers.Add(lineNumber + 1);
			}
			else
			{
				_tokenTypeToLineNumbers[tokenType] = new HashSet<int> { lineNumber + 1 };
			}
		}
	}
}
