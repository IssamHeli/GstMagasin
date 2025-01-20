using System;
using System.ComponentModel.DataAnnotations;
using TestHostAppAndBaseDonnes.Models;

namespace GstMagazin.Models
{
	public class ProduitAnalyse
	{
		[Key]
		public int id { get; set; }

		public virtual Produit Produit { get; set; }

		public  int ProduitId { get; set; }


		public  double Total_Achats { get; set; }


        public  double Total_Ventes { get; set; }


		public double Profite { get; set; }
    }
}

