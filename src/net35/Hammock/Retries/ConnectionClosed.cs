using System;
using System.Net;

namespace Hammock.Retries
{
#if !SILVERLIGHT && !WINRT
    [Serializable]
#endif
	public class ConnectionClosed : RetryErrorCondition
    {
        public override Predicate<Exception> RetryIf
        {
            get
            {
                return e =>
                           {
                               var we = e as WebException;
#if !WINRT
                               return we != null && (we.Status == WebExceptionStatus.ConnectionClosed || we.Status == WebExceptionStatus.KeepAliveFailure);
#else
															 return we != null && (we.Status == WebExceptionStatus.ConnectFailure || we.Status == WebExceptionStatus.UnknownError);
#endif
                           };
            }
        }
    }
}