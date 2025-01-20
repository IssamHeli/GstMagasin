using System;
using System.ComponentModel.DataAnnotations;

namespace TestHostAppAndBaseDonnes.Models
{
	public class Categorie
	{
		[Key]
		public int id { get; set; }
		public  string Designation { get; set; }
    }
}

