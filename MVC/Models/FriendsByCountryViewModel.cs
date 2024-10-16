using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Services;

namespace MVC.Models
{
	public class FriendsByCountryViewModel
	{

		public gstusrInfoAllDto infoAllDtos { get; set; }

		public List<string> distinctCountries { get; set; }


	}
}
