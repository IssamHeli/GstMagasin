using System;
using TestHostAppAndBaseDonnes.Models;

namespace GstMagazin.Models
{
	public class ProduitQnt0export
	{
		public Guid id { get; set; }

		public virtual Produit produit { get; set; }

        public int? QuantiteAchat { get; set; }


        public int? QuantiteVente { get; set; }


        public double? TotalAchat { get; set; }

		public double? TotalVonte { get; set; }


		public double Profite { get; set; }
	}
}

