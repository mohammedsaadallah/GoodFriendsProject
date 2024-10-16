using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace Razor.Pages.Shared.Uppgifter
{
    public class FriendInputModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<FriendInputModel> _logger = null;

        [BindProperty]
        public csFriendIM FriendIM { get; set; }

        public IFriend Friends { get; set; }

        public async Task<IActionResult> OnGet()
        {

            if (Guid.TryParse(Request.Query["id"], out Guid _id))
            {

                FriendIM = new csFriendIM(await _service.ReadFriendAsync(null, _id, false));
            }
            else
            {

                FriendIM = new csFriendIM();
                FriendIM.StatusIM = enStatusIM.Inserted;

            }
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (FriendIM.StatusIM == enStatusIM.Inserted)
            {
                try
                {

                    var updatedModel = FriendIM.UpdateModel(new csFriend()
                    {
                        FriendId = Guid.Empty,
                        FirstName = FriendIM.FirstName,
                        LastName = FriendIM.LastName,
                        Email = FriendIM.Email,
                        Birthday = FriendIM.Birthday,
                        Address = new csAddress()
                        {
                            AddressId = Guid.Empty,
                            StreetAddress = FriendIM.StreetAddress,
                            ZipCode = FriendIM.ZipCode,
                            City = FriendIM.City,
                            Country = FriendIM.Country
                        },
                    });

                    var updatedAdress = await _service.CreateAddressAsync(null, new csAddressCUdto()
                    {
                        AddressId = updatedModel.Address.AddressId,
                        StreetAddress = updatedModel.Address.StreetAddress,
                        ZipCode = updatedModel.Address.ZipCode,
                        City = updatedModel.Address.City,
                        Country = updatedModel.Address.Country,
                    });

                    var updatedFriend = await _service.CreateFriendAsync(null, new csFriendCUdto()
                    {
                        FriendId = updatedModel.FriendId,
                        FirstName = updatedModel.FirstName,
                        LastName = updatedModel.LastName,
                        Email = updatedModel.Email,
                        Birthday = updatedModel.Birthday,
                        AddressId = updatedAdress.AddressId
                    });

                    return Redirect($"/Shared/Uppgifter/FriendView?id={updatedFriend.FriendId}");

                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
            else
            {
                try
                {

                    var updatedModel = FriendIM.UpdateModel(new csFriend()
                    {
                        FriendId = FriendIM.FriendId,
                        FirstName = FriendIM.FirstName,
                        LastName = FriendIM.LastName,
                        Email = FriendIM.Email,
                        Birthday = FriendIM.Birthday,
                        Address = new csAddress()
                        {
                            AddressId = FriendIM.AddressId,
                            StreetAddress = FriendIM.StreetAddress,
                            ZipCode = FriendIM.ZipCode,
                            City = FriendIM.City,
                            Country = FriendIM.Country
                        },
                    });

                    var updatedFriend = await _service.UpdateFriendAsync(null, new csFriendCUdto()
                    {
                        FriendId = updatedModel.FriendId,
                        FirstName = updatedModel.FirstName,
                        LastName = updatedModel.LastName,
                        Email = updatedModel.Email,
                        Birthday = updatedModel.Birthday,
                        AddressId = updatedModel.Address.AddressId,
                        PetsId = FriendIM.Pets,
                        QuotesId = FriendIM.Quotes,

                    });
                    var updatedAdress = await _service.UpdateAddressAsync(null, new csAddressCUdto()
                    {
                        AddressId = updatedModel.Address.AddressId,
                        StreetAddress = updatedModel.Address.StreetAddress,
                        ZipCode = updatedModel.Address.ZipCode,
                        City = updatedModel.Address.City,
                        Country = updatedModel.Address.Country
                    });
                    return Redirect($"/Shared/Uppgifter/FriendView?id={updatedFriend.FriendId}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500);
                }
            }
        }

        public FriendInputModel(IFriendsService service, ILogger<FriendInputModel> logger)
        {
            _logger = logger;
            _service = service;
        }

        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }
        public class csFriendIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid FriendId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string Email { get; set; }
            public DateTime? Birthday { get; set; }

            public Guid AddressId { get; set; }
            public string StreetAddress { get; set; }
            public int ZipCode { get; set; }
            public string City { get; set; }
            public string Country { get; set; }

            public List<Guid> Pets { get; set; }
            public List<Guid> Quotes { get; set; }

            public csFriendIM() { StatusIM = enStatusIM.Unchanged; }

            public csFriendIM(csFriendIM original)
            {
                StatusIM = original.StatusIM;

                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;

                Email = original.Email;
                Birthday = original.Birthday;

                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;

                Pets = original.Pets;
                Quotes = original.Quotes;
            }

            public csFriendIM(IFriend model)
            {

                FriendId = model.FriendId;
                FirstName = model.FirstName;
                LastName = model.LastName;

                Email = model.Email;
                Birthday = model.Birthday;

                AddressId = model.Address.AddressId;
                StreetAddress = model.Address.StreetAddress;
                ZipCode = model.Address.ZipCode;
                City = model.Address.City;
                Country = model.Address.Country;

                Pets = model.Pets.Select(x => x.PetId).ToList();
                Quotes = model.Quotes.Select(x => x.QuoteId).ToList();
            }

            public csFriend UpdateModel(csFriend model)
            {
                model.FirstName = FirstName;
                model.LastName = LastName;

                model.Email = Email;
                model.Birthday = Birthday;

                model.Address.StreetAddress = StreetAddress;
                model.Address.ZipCode = ZipCode;
                model.Address.City = City;
                model.Address.Country = Country;

                return model;
            }
        }
    }
}
