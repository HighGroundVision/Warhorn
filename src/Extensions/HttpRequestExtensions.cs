using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace HGV.Warhorn.Api
{
    public static class HttpRequestExtensions
    {
        public static async Task<T> GetBodyAsync<T>(this HttpRequest req)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }
    }
}
