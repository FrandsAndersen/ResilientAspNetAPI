using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjectA.Services
{
    public class RequestStringService : IRequestStringService
    {
        private readonly HttpClient _httpClient;

        public RequestStringService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetStringsFromProjectB()
        {
            var response = await _httpClient.GetStringAsync("");
            return response;
        }


    }
}
