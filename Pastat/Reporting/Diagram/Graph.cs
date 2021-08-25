using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Pastat.Reporting.Diagram
{
	class Graph : IDisposable
	{
		private static readonly string FileName = BuildFileName();

		private readonly Font LabelFont;
		private readonly Pen GraphLinePen;
		private Bitmap _bitmap;
		private Graphics _graphics;
		private ITranslator _translator;
		private float _graphWidth;
		private float _graphHeight;
		private int _margin;
		private int PointRadius => _margin / 20;
		private int LineThikness => PointRadius;


		public Graph(int width, int height)
		{
			_margin = Math.Min(width, height) / 10;
			_graphWidth = width - 2 * _margin;
			_graphHeight = height - 2 * _margin;
			_bitmap = new Bitmap(width, height);
			_graphics = Graphics.FromImage(_bitmap);
			_graphics.SmoothingMode = SmoothingMode.HighQuality;
			_graphics.Clear(Color.White);
			LabelFont = new Font(FontFamily.GenericSansSerif, _margin / 10);
			GraphLinePen = new Pen(Brushes.Black, LineThikness);
		}

		public void SetupAndRenderAxes(long minX, long maxX, long minY, long maxY)
		{
			_translator = _translator.Shift(-minX, -minY)
				.Scale(_graphWidth / (maxX - minX), _graphHeight / (maxY - minY))
				.Shift(_margin, _margin)
				.Scale(1, -1)
				.Shift(0, _bitmap.Height);
			DrawLine(new PointL(minX, 0), new PointL(maxX, 0));
			DrawLine(new PointL(0, minY), new PointL(0, maxY));
		}

		private void DrawLine(PointL p1, PointL p2)
		{
			_graphics.DrawLine(GraphLinePen, (Point)_translator.Translate(p1), (Point)_translator.Translate(p2));
		}

		public void DrawSequence(Series series, XValue[] xValues)
		{
			var points = new PointL[xValues.Length];
			PointF? prevDrawPosition = null;
			for (int i = 0; i < xValues.Length; i++)
			{
				points[i] = new PointL(xValues[i].X, series.Values[i]);
				DrawLabel(xValues[i], prevDrawPosition, out PointF drawPosition);
				prevDrawPosition = drawPosition;
			}

			Point[] translated = points.Select(p => (Point)_translator.Translate(p)).ToArray();
			_graphics.DrawLines(GraphLinePen, translated);
			for (int i = 0; i < points.Length; i++)
			{
				_graphics.FillEllipse(Brushes.Red, GetRectFromCenter(translated[i], PointRadius));
			}
		}

		private void DrawLabel(XValue v, PointF? prevDrawPosition, out PointF drawPosition)
		{
			SizeF labelSize = _graphics.MeasureString(v.Label, LabelFont);
			PointL translatedPosition = _translator.Translate(new PointL(v.X, 0));
			drawPosition = new PointF(translatedPosition.X - labelSize.Width / 2, (int)(_graphHeight + 1.5 * _margin - 2 * labelSize.Height));
			if (prevDrawPosition.HasValue)
			{
				if(drawPosition.X - prevDrawPosition.Value.X < labelSize.Width)
				{
					return;
				}
			}
			_graphics.DrawString(v.Label, LabelFont, Brushes.Black, drawPosition, StringFormat.GenericTypographic);
		}

		private Rectangle GetRectFromCenter(Point p, int radius)
		{
			p.Offset(-radius, -radius);
			return new Rectangle(p, new Size(radius * 2, radius * 2));
		}

		public void WriteToFile()
		{
			_bitmap.Save(FileName);
		}

		// TODO: Move to separate class
		private static string BuildFileName()
		{
			string projectName = Path.GetFileName(Environment.CurrentDirectory);
			byte[] hash = MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(Environment.CurrentDirectory));

			string fileName = $"{projectName}_{ByteArrayToString(hash)}_diagram.bmp";
			string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string folder = Path.Combine(appDataPath, "Pastat");
			Directory.CreateDirectory(folder);
			return Path.Combine(folder, fileName);
		}

		private static string ByteArrayToString(byte[] ba)
		{
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
				hex.AppendFormat("{0:x2}", b);
			return hex.ToString();
		}

		public void Dispose()
		{
			_bitmap.Dispose();
			_graphics.Dispose();
		}
	}
}
