using System;
using System.Linq;
using System.Web;

namespace Albayan.Helpers
{
    public static class YouTubeHelper
    {
        public static string GetEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            if (url.Contains("/embed/"))
            {
                return url.Replace("youtube.com", "youtube-nocookie.com") + "?rel=0";
            }

            string videoId = string.Empty;
            try
            {
                var uri = new Uri(url);
                if (uri.Host.Contains("youtube.com"))
                {
                    var query = HttpUtility.ParseQueryString(uri.Query);
                    videoId = query["v"];
                }
                else if (uri.Host.Contains("youtu.be"))
                {
                    videoId = uri.Segments.LastOrDefault();
                }
            }
            catch
            {
                return "";
            }

            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://www.youtube-nocookie.com/embed/{videoId}?rel=0";
            }

            return "";
        }
    }
}
