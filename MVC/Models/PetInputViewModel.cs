using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.DTO;

namespace MVC.Models
{
    public class PetInputViewModel
    {
        [BindProperty]
        public csPetIM PetIM { get; set; }

        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

        public class csPetIM
        {
            public enStatusIM StatusIM { get; set; }
            public Guid PetId { get; set; }
            public string Name { get; set; }
            public enAnimalKind Kind { get; set; }
            public enAnimalMood Mood { get; set; }
            public Guid FriendId { get; set; }

            public csPetIM() { StatusIM = enStatusIM.Unchanged; }

            public csPetIM(csPetIM original)
            {
                Name = original.Name;
                Kind = original.Kind;
                Mood = original.Mood;
                FriendId = original.FriendId;
            }

            public csPetIM(IPet model)
            {
                PetId = model.PetId;
                Name = model.Name;
                Kind = model.Kind;
                Mood = model.Mood;
                FriendId = model.Friend.FriendId;
            }
        }
    }
}
