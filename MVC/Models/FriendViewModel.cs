using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MVC.Models
{
    public class FriendViewModel
    {
        public IFriend CurrentFriend { get; set; }

    }
}
