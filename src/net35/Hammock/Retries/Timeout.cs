using System;
using System.Net;

namespace Hammock.Retries
{
#if !SILVERLIGHT && !WINRT
    [Serializable]
#endif
	public class Timeout : RetryErrorCondition
    {
        public override Predicate<Exception> RetryIf
        {
            get
            {
                return e =>
                           {
                               var we = (e as WebException);
#if !WINRT
                               return we != null && (we.Status == WebExceptionStatus.RequestCanceled || we.Status == WebExceptionStatus.Timeout);
#else
															 return we != null && (we.Status == WebExceptionStatus.RequestCanceled || we.Status == WebExceptionStatus.UnknownError);
#endif
                           };
            }
        }
    }
}