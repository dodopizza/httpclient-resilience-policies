using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.HttpClient.ResiliencePolicies.FallbackSettings
{
	public class FallbackSettings : IFallbackSettings
	{
		public Func<DelegateResult<HttpResponseMessage>, Task> OnFallbackAsync { get; }

		public Func<CancellationToken, Task<HttpResponseMessage>> FallbackActionAsync { get; }

		/// <summary>
		/// Creates a configuration of fallback.
		/// </summary>
		/// <param name="fallbackValue">The fallback <see cref="HttpResponseMessage"/> value to provide.</param>
		/// <exception cref="ArgumentNullException">fallbackValue</exception>
		public FallbackSettings(HttpResponseMessage fallbackValue)
			: this((_) => Task.FromResult(fallbackValue), _defaultOnFallback)
		{
			if (fallbackValue == null) throw new ArgumentNullException(nameof(fallbackValue));
		}

		/// <summary>
		/// Creates a configuration of fallback.
		/// </summary>
		/// <param name="fallbackActionAsync">The fallback delegate.</param>
		/// <exception cref="ArgumentNullException">onFallbackAsync</exception>
		public FallbackSettings(Func<CancellationToken, Task<HttpResponseMessage>> fallbackActionAsync)
			: this(fallbackActionAsync, _defaultOnFallback)
		{ }

		/// <summary>
		/// Creates a configuration of fallback.
		/// </summary>
		/// <param name="fallbackValue">The fallback <typeparamref name="HttpResponseMessage"/> value to provide.</param>
		/// <param name="onFallbackAsync">The action to call asynchronously before invoking the fallback delegate.</param>
		/// <exception cref="ArgumentNullException">fallbackValue</exception>
		/// <exception cref="ArgumentNullException">onFallbackAsync</exception>
		public FallbackSettings(HttpResponseMessage fallbackValue,
			Func<DelegateResult<HttpResponseMessage>, Task> onFallbackAsync)
			: this((_) => Task.FromResult(fallbackValue), onFallbackAsync)
		{
			if (fallbackValue == null) throw new ArgumentNullException(nameof(fallbackValue));
		}

		/// <summary>
		/// Creates a configuration of fallback.
		/// </summary>
		/// <param name="fallbackActionAsync">The fallback delegate.</param>
		/// <param name="onFallbackAsync">The action to call asynchronously before invoking the fallback delegate.</param>
		/// <exception cref="ArgumentNullException">fallbackActionAsync</exception>
		/// <exception cref="ArgumentNullException">onFallbackAsync</exception>
		public FallbackSettings(
			Func<CancellationToken, Task<HttpResponseMessage>> fallbackActionAsync,
			Func<DelegateResult<HttpResponseMessage>, Task> onFallbackAsync)
		{
			if (fallbackActionAsync == null) throw new ArgumentNullException(nameof(fallbackActionAsync));
			if (onFallbackAsync == null) throw new ArgumentNullException(nameof(onFallbackAsync));

			FallbackActionAsync = fallbackActionAsync;
			OnFallbackAsync = onFallbackAsync;
		}

		private static readonly Func<DelegateResult<HttpResponseMessage>, Task> _defaultOnFallback = (_) => Task.CompletedTask;
	}
}
