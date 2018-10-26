#pragma warning disable S3994 // URI Parameters should not be strings

namespace Fanex.NAP.FS.Library.Core.Helpers.Api
{
    using Fanex.Logging;
    using MailDelivery.Client;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public static class ApiHelper
    {
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param, int environment, int mailingListId)
        {
            try
            {
                using (var web = new HttpClient())
                {
                    var response = await web.PostAsJsonAsync(url, param).ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                    {
                        var err = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        throw new HttpRequestException(err);
                    }

                    return await response.Content.ReadAsAsync<TResult>().ConfigureAwait(false);
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
    }
}

#pragma warning restore S3994 // URI Parameters should not be strings