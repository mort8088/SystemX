// -----------------------------------------------------------------------
// <copyright file="CrashReport.cs" company="Mort8088 Games">
// Copyright (c) 2012-22 Dave Henry for Mort8088 Games.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Specialized;
using System.Net;

namespace SystemX.CrashReport
{
    /// <summary>
    /// Usage:- CrashReport.Post("http://crash.mort8088.com/", new NameValueCollection() { { "GameName", "GameTest" }, { "ReportData", "What went wrong" } });
    /// </summary>
	public static class CrashReport
	{
		static CrashReport () {}
		
        /// <summary>
        /// Sends an async post without doing anything with the response
        /// </summary>
        /// <param name="uri">Where to post</param>
        /// <param name="pairs">what to post</param>
		public static void Post(string uri, NameValueCollection pairs)
		{
			try {
				using (WebClient client = new WebClient())
				{
					client.UploadValues(uri, pairs);
				}
			}
			catch {
                // I don't care.
			} 
		}
	}
}