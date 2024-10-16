using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace Razor.Pages.Shared.Uppgifter
{
    public class FriendViewModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<FriendViewModel> _logger = null;

        public Guid CurrentFriendId {  get; set; }
        public csFriend CurrentFriend {  get; set; }

        [BindProperty]
        public Guid PetId { get; set; }
        

        

        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _id)) 
            {  
                CurrentFriendId = _id;
                var friend = await _service.ReadFriendAsync(null, CurrentFriendId, false);

                

                csFriend _friend = new csFriend()
                {
                    FriendId = friend.FriendId,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Email = friend.Email,
                    Birthday = friend.Birthday,
                    Address = friend.Address,
                    Quotes = friend.Quotes,
                    Pets = friend.Pets,
                };
                CurrentFriend = _friend;

                
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeletePet()
        {
            string id = Request.Form["id"];
            _service.DeletePetAsync(null, PetId);

            return Redirect($"~/Shared/Uppgifter/FriendView?id={id}");
        }


        public FriendViewModel(IFriendsService service, ILogger<FriendViewModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }
}
