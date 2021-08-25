using System.Collections.Generic;

namespace Pastat.Reporting.Diagram
{
	public partial class CoordinatesTranslator
	{
		private class TranslationSequence : ITranslator
		{
			private List<ITranslator> _translators { get; set; }

			public TranslationSequence(ITranslator t)
			{
				_translators = new List<ITranslator> { t };
			}

			public PointL Translate(PointL p)
			{
				foreach(ITranslator t in _translators)
					p = t.Translate(p);
				return p;
			}

			public ITranslator Then(ITranslator t)
			{
				_translators.Add(t);
				return this;
			}
		}
	}
}
