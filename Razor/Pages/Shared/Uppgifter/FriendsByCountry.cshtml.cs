using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Models;
using Models.DTO;
namespace Razor.Pages.Shared.Uppgifter
{
    public class FriendsByCountryModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<FriendsByCountryModel> _logger = null;

        public gstusrInfoAllDto infoAllDtos { get; set; }
        public List<string> distinctCountries {  get; set; }

        public async Task<IActionResult> OnGet()
        {
            var info = await _service.InfoAsync;
            infoAllDtos = info;

            var clist = info.Friends.Select(x => x.Country).Where(x => x != null).Distinct().ToList();
            distinctCountries = clist;

            return Page();
        }

        public FriendsByCountryModel(IFriendsService service, ILogger<FriendsByCountryModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
