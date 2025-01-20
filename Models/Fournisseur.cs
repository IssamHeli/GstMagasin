using System;
using System.ComponentModel.DataAnnotations;

namespace GstMagazin.Models
{
	public class Fournisseur
	{
		[Key]
		public int id { get; set; }

		public string Nom { get; set; }

		public string tele { get; set; }
	}
}

