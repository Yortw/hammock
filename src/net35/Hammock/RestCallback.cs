﻿using System;

namespace Hammock
{
#if !SILVERLIGHT && !WINRT
    [Serializable]
#endif
    public delegate void RestCallback(RestRequest request, RestResponse response, object userState);

#if !SILVERLIGHT && !WINRT
    [Serializable]
#endif
		public delegate void RestCallback<T>(RestRequest request, RestResponse<T> response, object userState);
}