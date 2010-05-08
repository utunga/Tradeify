namespace Offr.Common
{
    public interface IWebRequestFactory
    {
        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <returns>The web server response.</returns>
        string RetrieveContent(string url);
    }
}