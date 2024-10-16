using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Services;

namespace Razor.Pages.Shared.Uppgifter
{
    public class ListOfFriendsModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<ListOfFriendsModel> _logger = null;

        public List<IFriend> Friends { get; set; }
        public string currentCity { get; set; }
        [BindProperty]
        public Guid RemoveFriendId { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string id = Request.Query["id"];
            currentCity = id;


            var friends = await _service.ReadFriendsAsync(null, true, false, "", 0, 1000);
            var createdFriends = await _service.ReadFriendsAsync(null, false, false, "", 0, 1000);

            var allFriends = friends.Concat(createdFriends);

            var allFriendsWithAdress = allFriends.Select(x => x)
                .Where(x => x.Address != null)
                .ToList();

            Friends = allFriendsWithAdress
                .Where(x => x.Address.City == currentCity)
                .ToList();

            return Page();
        }        

        public async Task<IActionResult> OnPostDelete()
        {
            string id = Request.Form["id"];
            await _service.DeleteFriendAsync(null, RemoveFriendId);
            return Redirect($"~/Shared/Uppgifter/ListOfFriends?id={id}");
        }

        public ListOfFriendsModel(IFriendsService service, ILogger<ListOfFriendsModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}