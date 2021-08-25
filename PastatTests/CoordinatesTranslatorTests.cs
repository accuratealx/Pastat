using NUnit.Framework;
using Pastat.Reporting.Diagram;
using System.Drawing;

namespace PastatTests
{
	[TestFixture]
	public class CoordinatesTranslatorTests
	{
		[Test]
		public static void TranslatorShouldTranslateCoordinates()
		{
			CoordinatesTranslator translator = new CoordinatesTranslator(new Point(5, 5), new PointF(2,3));
			Point expected = new Point(10, 15);
			PointL actual = translator.Translate(new PointL(0, 0));
			Assert.AreEqual(expected, (Point)actual);
		}

		[Test]
		public static void TranslatorShouldInvertYCoordinate()
		{
			CoordinatesTranslator translator = new CoordinatesTranslator(new Point(0, 0), new PointF(1, -1));
			Point expected = new Point(5, 5);
			PointL actual = translator.Translate(new PointL(5, -5));
			Assert.AreEqual(expected, (Point)actual);
		}
	}
}
