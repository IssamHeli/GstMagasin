using System;
using System.ComponentModel.DataAnnotations;

namespace TestHostAppAndBaseDonnes.Models
{
	public class LieuStock
	{
		[Key]
		public int id { get; set; }
		public  string Designation { get; set; }
    }
}

