using System;
using System.ComponentModel.DataAnnotations;

namespace TestHostAppAndBaseDonnes.Models
{
	public class Directeur
	{
		[Key]
		public int it { get; set; }

		public required String UserName { get; set; }
		public required String Password { get; set; }
    }

}

