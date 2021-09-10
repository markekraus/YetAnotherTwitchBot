using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace YetAnotherTwitchBot.Models
{
    public class ExecutiveOrderResponse
    {
        public int Count;
        public string Description;
        [JsonProperty(PropertyName = "total_pages")]
        public int TotalPages;
        [JsonProperty(PropertyName = "next_page_url")]
        public Uri NextPageUrl;
        public IList<ExecutiveOrder> Results;
    }
}