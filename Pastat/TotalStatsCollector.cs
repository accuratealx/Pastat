namespace PascalCodeStats
{
	internal class TotalStatsCollector
	{
		public int TotalLines { get; private set; }
		public int CodeCount { get; private set; }
		public int CommentsCount { get; private set; }
		public int DirectivesCount { get; private set; }
		public int EmptyLinesCount { get; private set; }

		internal void Add(FileStats stats)
		{
			TotalLines += stats.TotalLines;
			CodeCount += stats.CodeCount;
			CommentsCount += stats.CommentsCount;
			DirectivesCount += stats.DirectivesCount;
			EmptyLinesCount += stats.EmptyLinesCount;
		}
	}
}