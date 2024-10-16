using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace MVC.Models
{
	public class ListOfFriends
	{
		public List<IFriend> Friends { get; set; }
		public string currentCity { get; set; }
		
	}
}
