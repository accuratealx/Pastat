namespace Pastat.Reporting
{
    interface IStatsCollector
	{
		int TotalLines { get; }
		int CodeCount { get; }
		int CommentsCount { get; }
		int DirectivesCount { get; }
		int EmptyLinesCount { get; }
	}
}
