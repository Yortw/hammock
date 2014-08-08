using System;
using Hammock.Web;

namespace Hammock.Retries
{
#if !SILVERLIGHT && !WINRT
    [Serializable]
#endif
	public abstract class RetryResultCondition : IRetryCondition<WebQueryResult>
    {
        public virtual Predicate<WebQueryResult> RetryIf
        {
            get { return r => false; }
        }
    }
}