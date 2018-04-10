using System.Net.Http;
using System.Threading.Tasks;

namespace Fanex.NAP.FS.Library.Core.Helpers.Api
{
    public static class ApiHelper
    {
        public static async Task<TResult> Post<TParam, TResult>(string url, TParam param)
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
    }
}
