namespace Fanex.NAP.FS.Library.Services.Implement
{
    using Fanex.NAP.FS.Library.Services.Interface;
    using System;
    using System.Net.Http;
    using System.Text;

    public class HttpWebService: IHttpWebService
    {
        public string GetJson(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var message = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = client.SendAsync(message).Result;

                    using (HttpContent content = response.Content)
                    {
                        var result = content.ReadAsStringAsync().Result;

                        return result;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        public string PostJson(string url, string serializedDataString)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var message = new HttpRequestMessage(HttpMethod.Post, new Uri(url));
                    message.Content = new StringContent(serializedDataString, Encoding.Unicode, "application/json");
                    var response = client.SendAsync(message).Result;

                    using (HttpContent content = response.Content)
                    {
                        var result = content.ReadAsStringAsync().Result;

                        return result;
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
