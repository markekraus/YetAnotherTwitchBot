using System;
using Newtonsoft.Json;

namespace YetAnotherTwitchBot.Models
{
    public class ExecutiveOrder
    {
        public string Title;
        public string Type;
        public string Abstract;
        [JsonProperty(PropertyName = "document_number")]
        public string DocumentNumber;
        [JsonProperty(PropertyName = "html_url")]
        public Uri HtmlUrl;
        [JsonProperty(PropertyName = "pdf_url")]
        public Uri PdfUrl;
        [JsonProperty(PropertyName = "public_inspection_pdf_url")]
        public Uri PublicInspectionPdfUrl;
        [JsonProperty(PropertyName = "publication_date")]
        public DateTime PublicationDate;
        public string President;
        [JsonProperty(PropertyName = "executive_order_number")]
        public string ExecutiveOrderNumber;
        [JsonProperty(PropertyName = "signing_date")]
        public string SigningDate;
    }
}