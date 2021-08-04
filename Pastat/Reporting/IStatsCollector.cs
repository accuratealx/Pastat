namespace PascalCodeStats.Reporting
{
	public interface IStatsCollector
	{
		int TotalLines { get; }
		int CodeCount { get; }
		int CommentsCount { get; }
		int DirectivesCount { get; }
		int EmptyLinesCount { get; }
	}
}
