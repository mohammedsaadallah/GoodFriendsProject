using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MVC.Models
{
    public class QuoteInputViewModel
    {
        [BindProperty]
        public csQuoteIM QuoteIM { get; set; }
        public List<string> Quotes { get; set; }
        public string Author { get; set; }

        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }
        public class csQuoteIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid QuoteId { get; set; }
            public string Quote { get; set; }
            public string Author { get; set; }
            public Guid FriendId { get; set; }

            public csQuoteIM() { StatusIM = enStatusIM.Unchanged; }

            public csQuoteIM(csQuoteIM original)
            {
                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
                FriendId = original.FriendId;
            }

            public csQuoteIM(IQuote model)
            {
                QuoteId = model.QuoteId;
                Quote = model.Quote;
                Author = model.Author;
                FriendId = model.Friends.Select(f => f.FriendId).FirstOrDefault();
            }
        }
    }
}
