using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;
using MVC.Models;
using Services;

namespace MVC.Controllers
{
    public class UppgifterController : Controller
    {
        IFriendsService _service = null;
        ILogger<UppgifterController> _logger = null;

        public async Task<IActionResult> FriendsByCountry()
        {
            var info = await _service.InfoAsync;

            var clist = info.Friends.Select(x => x.Country).Where(x => x != null).Distinct().ToList();


            var vm = new FriendsByCountryViewModel() { infoAllDtos = info, distinctCountries = clist };

            return View("FriendsByCountry", vm);
        }

        public async Task<IActionResult> FriendsByCity(string id)
        {
            var info = await _service.InfoAsync;
            var cntrylist = info.Friends.Where(x => x.Country == id).ToList();

            var vm = new FriendsByCityVwM()
            {
                infoAllDtos = info,
                distinctCities = cntrylist.Where(x => x.City != null).Distinct().ToList(),
                currentCountry = id
            };

            return View(vm);
        }

        public async Task<IActionResult> ListOfFriends(string id)
        {
            var friends = await _service.ReadFriendsAsync(null, true, false, "", 0, 1000);
            var createdFriends = await _service.ReadFriendsAsync(null, false, false, "", 0, 1000);

            var allFriends = friends.Concat(createdFriends);

            var allFriendsWithAdress = allFriends
                .Select(x => x)
                .Where(x => x.Address != null)
                .ToList();
            var vm = new ListOfFriends()
            {
                currentCity = id,

                Friends = allFriendsWithAdress
                .Where(x => x.Address.City == id)
                .ToList()
            };

            return View(vm);
        }

        public async Task<IActionResult> FriendView(Guid id)
        {
            var vm = new FriendViewModel()
            {
                CurrentFriend = await _service.ReadFriendAsync(null, id, false)
            };

            return View(vm);
        }

        public async Task<IActionResult> FriendInput(Guid id)
        {
            if (id != Guid.Empty)
            {
                var vm = new FriendInputViewModel()
                {
                    FriendIM = new FriendInputViewModel.csFriendIM(await _service.ReadFriendAsync(null, id, false))
                };

                return View(vm);
            }
            else
            {

                var vm = new FriendInputViewModel()
                {
                    FriendIM = new FriendInputViewModel.csFriendIM(),

                };

                vm.FriendIM.StatusIM = FriendInputViewModel.enStatusIM.Inserted;

                return View(vm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> FriendInputSave(FriendInputViewModel vm)
        {
            if (vm.FriendIM.StatusIM.Equals(FriendInputViewModel.enStatusIM.Inserted))
            {
                var createdAddress = await _service.CreateAddressAsync(null, new csAddressCUdto()
                {
                    AddressId = vm.FriendIM.AddressId,
                    StreetAddress = vm.FriendIM.StreetAddress,
                    ZipCode = vm.FriendIM.ZipCode,
                    City = vm.FriendIM.City,
                    Country = vm.FriendIM.Country,

                });
                var createdFriend = await _service.CreateFriendAsync(null, new csFriendCUdto()
                {
                    FriendId = vm.FriendIM.FriendId,
                    FirstName = vm.FriendIM.FirstName,
                    LastName = vm.FriendIM.LastName,
                    Email = vm.FriendIM.Email,
                    Birthday = vm.FriendIM.Birthday,
                    AddressId = createdAddress.AddressId,
                });

                return Redirect($"~/Uppgifter/FriendView?id={createdFriend.FriendId}");
            }
            else
            {
                var updatedFriend = await _service.UpdateFriendAsync(null, new csFriendCUdto()
                {
                    FriendId = vm.FriendIM.FriendId,
                    FirstName = vm.FriendIM.FirstName,
                    LastName = vm.FriendIM.LastName,
                    Email = vm.FriendIM.Email,
                    Birthday = vm.FriendIM.Birthday,
                    AddressId = vm.FriendIM.AddressId,
                    PetsId = vm.FriendIM.Pets,
                    QuotesId = vm.FriendIM.Quotes,
                });

                var updatedAddress = await _service.UpdateAddressAsync(null, new csAddressCUdto()
                {
                    AddressId = vm.FriendIM.AddressId,
                    StreetAddress = vm.FriendIM.StreetAddress,
                    ZipCode = vm.FriendIM.ZipCode,
                    City = vm.FriendIM.City,
                    Country = vm.FriendIM.Country,

                });

                return Redirect($"~/Uppgifter/FriendView?id={vm.FriendIM.FriendId}");
            }
        }

        
        public async Task<IActionResult> QuoteInput(Guid friendId, Guid quoteId)
        {
            if (quoteId != Guid.Empty)
            {

                var vm = new QuoteInputViewModel()
                {
                    QuoteIM = new QuoteInputViewModel.csQuoteIM(await _service.ReadQuoteAsync(null, quoteId, false)),
                };
                return View(vm);
            }
            else
            {
                var quotes = await _service.ReadQuotesAsync(null, true, false, "", 0, 1000);
                var distinctQuotes = quotes.Select(x => x.Quote).Distinct().ToList();

                csSeedGenerator rndName = new csSeedGenerator();

                var info = await _service.ReadFriendAsync(null, friendId, false);
                var existingQuotes = info.Quotes.Select(x => x.Quote).ToList();

                for (int i = 0; i < distinctQuotes.Count; i++)
                {
                    foreach (var Quote in existingQuotes)
                    {
                        if (Quote == distinctQuotes[i])
                        {
                            distinctQuotes.RemoveAt(i);
                        }
                    }
                }

                var vm = new QuoteInputViewModel()
                {
                    QuoteIM = new QuoteInputViewModel.csQuoteIM() { FriendId = friendId, StatusIM = QuoteInputViewModel.enStatusIM.Inserted},
                    Quotes = distinctQuotes,
                    Author = rndName.FullName.ToString(),
                };
                
                return View(vm);
            }  
        }

        [HttpPost]
        public async Task<IActionResult> OnPostSubmit(QuoteInputViewModel vm)
        {
            if (vm.QuoteIM.StatusIM.Equals(QuoteInputViewModel.enStatusIM.Inserted))
            {
                

                var createdQuoted = await _service.CreateQuoteAsync(null, new csQuoteCUdto()
                {
                    QuoteId = Guid.Empty,
                    Quote = vm.QuoteIM.Quote,
                    Author = vm.QuoteIM.Author


                });
                var friend = await _service.ReadFriendAsync(null, vm.QuoteIM.FriendId, false);

                var quoteList = friend.Quotes.Select(x => x.QuoteId).ToList();
                quoteList.Add(createdQuoted.QuoteId);

                var updateFriend = await _service.UpdateFriendAsync(null, new csFriendCUdto()
                {
                    FriendId = friend.FriendId,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Email = friend.Email,
                    Birthday = friend.Birthday,

                    AddressId = friend.Address.AddressId,
                    PetsId = friend.Pets.Select(x => x.PetId).ToList(),
                    QuotesId = quoteList
                });

                return Redirect($"/Uppgifter/FriendView?id={vm.QuoteIM.FriendId}");
            }
            else
            {
 

                var updatedDTO = _service.UpdateQuoteAsync(null, new csQuoteCUdto()
                {
                    QuoteId = vm.QuoteIM.QuoteId,
                    Author = vm.QuoteIM.Author,
                    Quote = vm.QuoteIM.Quote

                });

                return Redirect($"/Uppgifter/FriendView?id={vm.QuoteIM.FriendId}");
            }
        }

        public async Task<IActionResult> PetInput(Guid friendId, Guid petId)
        {
            if (petId != Guid.Empty)
            {
                var vm = new PetInputViewModel()
                {
                    PetIM = new PetInputViewModel.csPetIM(await _service.ReadPetAsync(null, petId, false))
                };

                return View(vm);
            }
            else
            {
                var vm = new PetInputViewModel()
                {
                    PetIM = new PetInputViewModel.csPetIM() { FriendId = friendId, StatusIM = PetInputViewModel.enStatusIM.Inserted }
                };
                return View(vm);
            }
        }

        public async Task<IActionResult> OnPostSave(PetInputViewModel vm)
        {
            if (vm.PetIM.StatusIM.Equals(PetInputViewModel.enStatusIM.Inserted))
            {

                await _service.CreatePetAsync(null, new csPetCUdto()
                {
                    PetId = vm.PetIM.PetId,
                    Name = vm.PetIM.Name,
                    Mood = vm.PetIM.Mood,
                    Kind = vm.PetIM.Kind,
                    FriendId = vm.PetIM.FriendId,
                });
                return Redirect($"/Uppgifter/FriendView?id={vm.PetIM.FriendId}");
            }
            else
            {

                var updatedDTO = await _service.UpdatePetAsync(null, new csPetCUdto()
                {
                    PetId = vm.PetIM.PetId,
                    Name = vm.PetIM.Name,
                    Kind = vm.PetIM.Kind,
                    Mood = vm.PetIM.Mood,
                    FriendId = vm.PetIM.FriendId,
                });
                return Redirect($"/Uppgifter/FriendView?id={vm.PetIM.FriendId}");
            }
        }

        public UppgifterController(IFriendsService service, ILogger<UppgifterController> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
