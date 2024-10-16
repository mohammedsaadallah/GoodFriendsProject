using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using static Razor.Pages.Shared.Uppgifter.FriendInputModel;

namespace Razor.Pages.Shared.Uppgifter
{
    public class PetInputModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<PetInputModel> _logger = null;
 

        [BindProperty]
        public csPetIM PetIM { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _id))
            {
                PetIM = new csPetIM(await _service.ReadPetAsync(null, _id, false));
            }
            else
            {
                Guid.TryParse(Request.Query["id2"], out Guid id2);
                

                PetIM = new csPetIM() { FriendId = id2, StatusIM = enStatusIM.Inserted };

            }
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (PetIM.StatusIM == enStatusIM.Inserted) 
            {
                var updatedModel = PetIM.UpdateModel(new csPet()
                {
                    PetId = PetIM.PetId,
                    Name = PetIM.Name,
                    Mood = PetIM.Mood,
                    Kind = PetIM.Kind,
                });

                await _service.CreatePetAsync(null, new csPetCUdto()
                {
                    PetId = updatedModel.PetId,
                    Name = updatedModel.Name,
                    Mood = updatedModel.Mood,
                    Kind = updatedModel.Kind,
                    FriendId = PetIM.FriendId,
                });
                return Redirect($"/Shared/Uppgifter/FriendView?id={PetIM.FriendId}");
            }
            else
            {
                var updatedModel = PetIM.UpdateModel(new csPet()
                {
                    PetId = PetIM.PetId,
                    Name = PetIM.Name,
                    Kind = PetIM.Kind,
                    Mood = PetIM.Mood,
                });

                var updatedDTO = await _service.UpdatePetAsync(null, new csPetCUdto()
                {
                    PetId = updatedModel.PetId,
                    Name = updatedModel.Name,
                    Kind = updatedModel.Kind,
                    Mood = updatedModel.Mood,
                    FriendId = PetIM.FriendId,
                });
                return Redirect($"/Shared/Uppgifter/FriendView?id={PetIM.FriendId}");
            }
        }

        public PetInputModel(IFriendsService service, ILogger<PetInputModel> logger)
        {
            _logger = logger;
            _service = service;
        }

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


            public csPet UpdateModel(csPet model)
            {
                Name = model.Name;
                Kind = model.Kind;
                Mood = model.Mood;


                return model;
            }
        }
    }
}
