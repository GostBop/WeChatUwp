using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WeChat
{
    class Get
    {
        public static async Task<HttpResponseMessage> Get_Response_Str(String uri, String cookie_str)
        {
            HttpClientHandler myHandler = new HttpClientHandler();
            myHandler.AllowAutoRedirect = false;
            HttpClient myClient = new HttpClient(myHandler);
            var myRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            myClient.DefaultRequestHeaders.Add("Cookie", cookie_str);
            var response = await myClient.SendAsync(myRequest);

            //string result = await response.Content.ReadAsStringAsync();

            return response;
        }
    }
}
