using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;

namespace MVC.Models
{
    public class FriendsByCityVwM
    {
        public gstusrInfoAllDto infoAllDtos { get; set; }
        public List<gstusrInfoFriendsDto> distinctCities { get; set; }
        public string currentCountry { get; set; }
    }
}
