namespace Fanex.NAP.FS.Library.Services.Interface
{
    public interface IHttpWebService
    {
        string GetJson(string url);

        string PostJson(string url, string serializedDataString);
    }
}
