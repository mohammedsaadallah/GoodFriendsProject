using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace Razor.Pages.Shared.Uppgifter
{
    public class QuoteInputModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<QuoteInputModel> _logger = null;
        public List<string> Quotes { get; set; }
        [BindProperty]
        public csQuoteIM QuoteIM { get; set; }
        public string Author { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                QuoteIM = new csQuoteIM(await _service.ReadQuoteAsync(null, id, false));
            }
            else
            {
                Guid.TryParse(Request.Query["id2"], out Guid id2);
                QuoteIM = new csQuoteIM();
                Quotes = new List<string>();

                var quotes = await _service.ReadQuotesAsync(null, true, false, "", 0, 1000);
                var distinctQuotes = quotes.Select(x => x.Quote).Distinct().ToList();

                csSeedGenerator rndName = new csSeedGenerator();
                Author = rndName.FullName.ToString();   

                var info = await _service.ReadFriendAsync(null, id2, false);
                var existingQuotes = info.Quotes.Select(x => x.Quote).ToList();
                    
                for (int i = 0; i < distinctQuotes.Count; i++)
                {
                    foreach (var Quote in existingQuotes)
                    {
                        if(Quote == distinctQuotes[i])
                        {
                            distinctQuotes.RemoveAt(i);
                        }
                    }
                }
                Quotes = distinctQuotes;
                QuoteIM.FriendId = id2;
                QuoteIM.StatusIM = enStatusIM.Inserted;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            if (QuoteIM.StatusIM == enStatusIM.Inserted)
            {
                Quotes = new List<string>();

                var updateModel = QuoteIM.UpdateModel(new csQuote()
                {
                    Author = QuoteIM.Author,
                    Quote = QuoteIM.Quote,

                });
                var updatedDTO = await _service.CreateQuoteAsync(null, new Models.DTO.csQuoteCUdto()
                {
                    QuoteId = Guid.Empty,
                    Quote = updateModel.Quote,
                    Author = updateModel.Author,


                });
                var friend = await _service.ReadFriendAsync(null, QuoteIM.FriendId, false);

                var test = friend.Quotes.Select(x => x.QuoteId).ToList();
                test.Add(updatedDTO.QuoteId);

                var updateFriend = await _service.UpdateFriendAsync(null, new Models.DTO.csFriendCUdto()
                {
                    FriendId = friend.FriendId,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Email = friend.Email,
                    Birthday = friend.Birthday,

                    AddressId = friend.Address.AddressId,
                    PetsId = friend.Pets.Select(x => x.PetId).ToList(),
                    QuotesId = test
                });

                return Redirect($"/Shared/Uppgifter/FriendView?id={QuoteIM.FriendId}");
            }
            else
            {
                var updateModel = QuoteIM.UpdateModel(new csQuote()
                {
                    Author = QuoteIM.Author,
                    Quote = QuoteIM.Quote,
                });

                var updatedDTO = _service.UpdateQuoteAsync(null, new Models.DTO.csQuoteCUdto()
                {
                    QuoteId = QuoteIM.QuoteId,
                    Author = updateModel.Author,
                    Quote = updateModel.Quote

                });

                return Redirect($"/Shared/Uppgifter/FriendView?id={QuoteIM.FriendId}");
            }
        }


        public QuoteInputModel(IFriendsService service, ILogger<QuoteInputModel> logger)
        {
            _logger = logger;
            _service = service;
        }
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

            public csQuote UpdateModel(csQuote model)
            {
                Quote = model.Quote;
                Author = model.Author;

                return model;
            }
        }
    }
}
