using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Dodo.HttpClient.ResiliencePolicies.FallbackSettings
{
	public class FallbackSettings : IFallbackSettings
	{
		public Action<DelegateResult<HttpResponseMessage>, TimeSpan> OnFallback { get; }

		public Action FallbackAction { get; }

		public FallbackSettings() : this(_defaultFallbackAction, _defaultOnFallback)
		{ }

		public FallbackSettings(Action fallbackAction) : this(fallbackAction, _defaultOnFallback)
		{ }

		public FallbackSettings(Action fallbackAction, Action<DelegateResult<HttpResponseMessage>, TimeSpan> onFallback)
		{
			FallbackAction = fallbackAction;
			OnFallback = onFallback;
		}

		public static IFallbackSettings Default() => new FallbackSettings();

		private static readonly Action _defaultFallbackAction = () => { };

		private static readonly Action<DelegateResult<HttpResponseMessage>, TimeSpan> _defaultOnFallback = (_, __) => { };
	}
}
