[1mdiff --git a/src/net35/Hammock/RestClient.cs b/src/net35/Hammock/RestClient.cs[m
[1mindex b06dfdf..89f1a27 100644[m
[1m--- a/src/net35/Hammock/RestClient.cs[m
[1m+++ b/src/net35/Hammock/RestClient.cs[m
[36m@@ -209,7 +209,7 @@[m [mprivate RestRequest PrepareRequest(RestRequest request, out string uri, out WebQ[m
         private bool RequestMultiPart(RestBase request, WebQuery query, string url, out WebException exception)[m
         {[m
             var parameters = GetPostParameters(request);[m
[31m-            if (parameters == null || parameters.Count() == 0)[m
[32m+[m[32m            if (parameters == null || !parameters.Any())[m[41m[m
             {[m
                 exception = null;[m
                 return false;[m
[36m@@ -2614,9 +2614,20 @@[m [mprivate void SetQueryMeta(RestRequest request, WebQuery query)[m
             CoalesceWebPairsIntoCollection(query.Cookies, Cookies, request.Cookies);[m
 #pragma warning restore 618[m
 [m
[32m+[m[41m[m
[32m+[m						[32m//TFW - Probably a bad idea[m[41m[m
[32m+[m						[32m//if ((request.Method.Value == WebMethod.Post || request.Method.Value == WebMethod.Put) && request.QueryHandling == Hammock.QueryHandling.AppendToParameters)[m[41m[m
[32m+[m						[32m//{[m[41m[m
[32m+[m						[32m//	foreach (var p in request.Parameters)[m[41m[m
[32m+[m						[32m//	{[m[41m[m
[32m+[m						[32m//		request.PostParameters.Add(new HttpPostParameter(p.Name, p.Value));[m[41m[m
[32m+[m						[32m//	}[m[41m[m
[32m+[m						[32m//	request.Parameters.Clear();[m[41m[m
[32m+[m						[32m//}[m[41m[m
[32m+[m[41m[m
 #if !NETCF[m
[31m-            // If CookieContainer is set on request object then use that, else use the CookieContainer set on the Client.[m
[31m-            if (request.CookieContainer == null)[m
[32m+[m						[32m// If CookieContainer is set on request object then use that, else use the CookieContainer set on the Client.[m[41m[m
[32m+[m						[32mif (request.CookieContainer == null)[m[41m[m
                 request.CookieContainer = this.CookieContainer; [m
 [m
             query.CookieContainer = request.CookieContainer;[m
[1mdiff --git a/src/net35/Hammock/Web/WebQuery.cs b/src/net35/Hammock/Web/WebQuery.cs[m
[1mindex 6b9a4c3..0c31078 100644[m
[1m--- a/src/net35/Hammock/Web/WebQuery.cs[m
[1m+++ b/src/net35/Hammock/Web/WebQuery.cs[m
[36m@@ -1241,20 +1241,38 @@[m [mprivate static int Write(bool write, Encoding encoding, Stream requestStream, st[m
             return dataBytes.Length;[m
         }[m
 [m
[31m-        private static int WriteLine(bool write, Encoding encoding, Stream requestStream, string input)[m
[31m-        {[m
[31m-            var sb = new StringBuilder();[m
[31m-            sb.AppendLine(input);[m
[32m+[m		[32m//private static int WriteLine(bool write, Encoding encoding, Stream requestStream, string input)[m[41m[m
[32m+[m		[32m//{[m[41m[m
[32m+[m		[32m//	var sb = new StringBuilder();[m[41m[m
[32m+[m		[32m//	sb.AppendLine(input);[m[41m[m
 [m
[31m-            var dataBytes = encoding.GetBytes(sb.ToString());[m
[31m-            if (write)[m
[31m-            {[m
[31m-                requestStream.Write(dataBytes, 0, dataBytes.Length);[m
[31m-            }[m
[31m-            return dataBytes.Length;[m
[31m-        }[m
[32m+[m		[32m//	var dataBytes = encoding.GetBytes(sb.ToString());[m[41m[m
[32m+[m		[32m//	if (write)[m[41m[m
[32m+[m		[32m//	{[m[41m[m
[32m+[m		[32m//		requestStream.Write(dataBytes, 0, dataBytes.Length);[m[41m[m
[32m+[m		[32m//	}[m[41m[m
 [m
[31m-        private long WriteMultiPartImpl(bool write, IEnumerable<HttpPostParameter> parameters, string boundary, Encoding encoding, Stream requestStream)[m
[32m+[m		[32m//	if (dataBytes.Length != encoding.GetByteCount(input))[m[41m[m
[32m+[m		[32m//		System.Diagnostics.Debugger.Break();[m[41m[m
[32m+[m[41m[m
[32m+[m		[32m//	return dataBytes.Length;[m[41m[m
[32m+[m		[32m//}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mprivate static readonly byte[] NewLineBytes = new byte[] { 13, 10 };[m[41m[m
[32m+[m		[32mprivate static int WriteLine(bool write, Encoding encoding, Stream requestStream, string input)[m[41m[m
[32m+[m		[32m{[m[41m[m
[32m+[m			[32mif (write)[m[41m[m
[32m+[m			[32m{[m[41m[m
[32m+[m				[32mvar dataBytes = encoding.GetBytes(input);[m[41m[m
[32m+[m				[32mrequestStream.Write(dataBytes, 0, dataBytes.Length);[m[41m[m
[32m+[m				[32mrequestStream.Write(NewLineBytes, 0, NewLineBytes.Length);[m[41m[m
[32m+[m				[32mreturn dataBytes.Length + NewLineBytes.Length;[m[41m[m
[32m+[m			[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m			[32mreturn encoding.GetByteCount(input) + NewLineBytes.Length;[m[41m[m
[32m+[m		[32m}[m[41m[m
[32m+[m[41m[m
[32m+[m		[32mprivate long WriteMultiPartImpl(bool write, IEnumerable<HttpPostParameter> parameters, string boundary, Encoding encoding, Stream requestStream)[m[41m[m
         {[m
             Stream fs = null;[m
             var header = string.Format("--{0}", boundary);[m
