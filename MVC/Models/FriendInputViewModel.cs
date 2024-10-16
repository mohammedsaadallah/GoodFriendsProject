using Microsoft.AspNetCore.Mvc;
using Models;

namespace MVC.Models
{
    public class FriendInputViewModel
    {
        [BindProperty]
        public csFriendIM FriendIM { get; set; }



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
                Quotes = model.Quotes.Select(q => q.QuoteId).ToList();
            }
        }
    }
}
