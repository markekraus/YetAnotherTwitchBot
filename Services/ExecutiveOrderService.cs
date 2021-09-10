using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YetAnotherTwitchBot.Interfaces;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Services
{
    public class ExecutiveOrderService : IExecutiveOrderService
    {
        private ILogger<ExecutiveOrderService> _logger;
        private HttpClient _client;
        private const string _searchUrlPattern = "https://www.federalregister.gov/api/v1/documents?fields%5B%5D=abstract&fields%5B%5D=document_number&fields%5B%5D=executive_order_number&fields%5B%5D=html_url&fields%5B%5D=pdf_url&fields%5B%5D=public_inspection_pdf_url&fields%5B%5D=publication_date&fields%5B%5D=signing_date&fields%5B%5D=title&fields%5B%5D=type&conditions%5Bpresident%5D%5B%5D={1}&conditions%5Bpresidential_document_type%5D%5B%5D=executive_order&conditions%5Btype%5D%5B%5D=PRESDOCU&format=json&page={0}&per_page=1000";
        private Dictionary<string,string> _presidents = new Dictionary<string, string>()
        {
            {"william-j-clinton","Bill Clinton"},
            {"george-w-bush","George W. Bush"},
            {"barack-obama","Barack Obama"},
            {"donald-trump","Donald Trump"},
            {"joe-biden","Joe Biden"}
        };
        private List<ExecutiveOrder> _executiveOrders;
        private Random random = new Random();
        public ExecutiveOrderService(
            ILogger<ExecutiveOrderService> Logger,
            HttpClient Client)
        {
            _logger = Logger;
            _client = Client;
            var task = Init();
        }
        public ExecutiveOrder GetRandom()
        {
            int orderId = random.Next(0,_executiveOrders.Count -1);
            _logger.LogInformation($"Random id: {orderId}");
            return _executiveOrders[orderId];
        }

        public async Task Init()
        {
            _logger.LogInformation("Initializing Executove Order list...");
            _executiveOrders = new List<ExecutiveOrder>();
            foreach (var key in _presidents.Keys)
            {
                int page = 1;
                int totalPages = 100;
                var searchResults = new List<ExecutiveOrder>();
                while (page <= totalPages)
                {
                    _logger.LogInformation($"Fetching Executive Order Page {page} for President {_presidents[key]}...");
                    var url = string.Format(_searchUrlPattern, 1, key);
                    var response = await _client.GetAsync(url);
                    var result = await response.Content.ReadAsAsync<ExecutiveOrderResponse>();
                    totalPages = result.TotalPages;
                    searchResults.AddRange(result.Results);
                    page++;
                }
                foreach (var searchResult in searchResults)
                {
                    searchResult.President = _presidents[key];
                    _executiveOrders.Add(searchResult);
                }
            }
            _logger.LogInformation($"Initialized Executove Order list with {_executiveOrders.Count} items.");
        }
    }
}