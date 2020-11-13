using System;
using System.Net.Http;

namespace Dodo.HttpClientResiliencePolicies.Core
{
	public sealed class PolicyResult
	{
		/// <summary>
		/// Create an instance of <see cref="DelegateResult{TResult}"/> representing an execution which returned <paramref name="result"/>
		/// </summary>
		/// <param name="result">The result.</param>
		public PolicyResult(HttpResponseMessage result) => Result = result;

		/// <summary>
		/// Create an instance of <see cref="DelegateResult{TResult}"/> representing an execution which threw <paramref name="exception"/>
		/// </summary>
		/// <param name="exception">The exception.</param>
		public PolicyResult(Exception exception) => Exception = exception;

		/// <summary>
		/// The result of executing the delegate. Will be default(TResult) if an exception was thrown.
		/// </summary>
		public HttpResponseMessage Result { get; }

		/// <summary>
		/// Any exception thrown while executing the delegate. Will be null if policy executed without exception.
		/// </summary>
		public Exception Exception { get; }
	}
}
