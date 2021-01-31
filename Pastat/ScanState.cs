namespace PascalCodeStats
{
	partial class FileScanner
	{
		private enum ScanState
		{
			None,
			Code,
			CompilerDirectiveCurly,
			CompilerDirectiveNonCurly,
			CommentCurly,
			CommentNonCurly,
		}
	}
}
