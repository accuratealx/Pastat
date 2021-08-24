using System.Drawing;

namespace Pastat.Reporting.Diagram
{
    public partial class CoordinatesTranslator : ITranslator
    {
        private PointF _shift;
        private PointF _scale;

        public CoordinatesTranslator(PointF shift, PointF scale)
        {
            _shift = shift;
            _scale = scale;
        }

        public PointL Translate(PointL p)
        {
            PointF pf = new PointF(p.X, p.Y);
            pf.X += _shift.X;
            pf.Y += _shift.Y;
            pf.X *= _scale.X;
            pf.Y *= _scale.Y;
            return new PointL((long)pf.X, (long)pf.Y);
        }

        public ITranslator Then(ITranslator t)
        {
            return new TranslationSequence(this).Then(t);
        }
    }
}
