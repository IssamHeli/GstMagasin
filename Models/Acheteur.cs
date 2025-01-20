using System;
using System.ComponentModel.DataAnnotations;

namespace TestHostAppAndBaseDonnes.Models
{
	public class Acheteur
	{
		[Key]
		public int id { get; set; }
		public  string UserName { get; set; }
		public string Password { get; set; }
    }
}

