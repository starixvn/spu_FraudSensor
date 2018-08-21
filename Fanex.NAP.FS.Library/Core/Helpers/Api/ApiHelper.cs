#pragma warning disable S3994 // URI Parameters should not be strings

namespace Fanex.NAP.FS.Library.Core.Helpers.Api
{
    using Fanex.Logging;
    using MailDelivery.Client;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public static class ApiHelper
    {
        [Obsolete("Too many generic and parameter")]
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param, int environment, int mailingListId)
        {
            try
            {
                using (var web = new HttpClient())
                {
                    var myContent = JsonConvert.SerializeObject(param);

                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);

                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    var response = await web.PostAsync(url, byteContent).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        Logging.LogService.Error(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                        return default(TResult);
                    }

                    return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                }
            }
            catch (Exception exception)
            {
                var newMail = new MailDelivery.Core.Models.Mail
                {
                    Subject = string.Format("[({0}) SPU FS] ", environment == 1 ? "LOCAL" : environment == 2 ? "UAT" : "PRO") + " Error Calling FPS Middle Service's API",
                    Title = "SPU Fraud Sensor",
                    Body = "Error when calling API from FPS Middle Service \n API url: " + url + " \n Params: " + param + " \n Error: " + exception,
                    MailingListId = mailingListId
                };

                MailClientHandler.SendFunctionMail(newMail);
                LogService.Error(exception.Message, exception);

                return default(TResult);
            }
        }

        public static async Task<TResult> Post<TResult>(string url, object param)
        {
            using (var web = new HttpClient())
            {
                var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return default(TResult);
                }

                return await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
            }
        }

        public static async Task Post(string url, object param)
        {
            using (var web = new HttpClient())
            {
                await web.PostAsJsonAsync(url, param).ConfigureAwait(false);
            }
        }
    }
}

#pragma warning restore S3994 // URI Parameters should not be strings