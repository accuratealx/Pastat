using System.Drawing;

namespace Pastat.Reporting.Diagram
{
	public interface ITranslator
	{
		PointL Translate(PointL p);
		ITranslator Then(ITranslator t);
	}

	public static class TranslatorExtensions
	{
		public static ITranslator Shift(this ITranslator t, float x, float y)
		{
			ITranslator shiftTranslator = new CoordinatesTranslator(new PointF(x, y), new PointF(1, 1));
			return t?.Then(shiftTranslator) ?? shiftTranslator;
		}

		public static ITranslator Scale(this ITranslator t, float x, float y)
		{
			ITranslator shiftTranslator = new CoordinatesTranslator(new PointF(0, 0), new PointF(x, y));
			return t?.Then(shiftTranslator) ?? shiftTranslator;
		}
	}
}
