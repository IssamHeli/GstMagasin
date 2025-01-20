using System;
using System.ComponentModel.DataAnnotations;

namespace TestHostAppAndBaseDonnes.Models
{

	public class PlanAchat
    {


        [Key]
		public int id { get; set; }

        public  virtual Produit produit { get; set; }

        public  int ProduitId { get; set; }

        public  string Reference { get; set; }

        public  int Qnt_achat { get; set; }

        public  double Prix_achat { get; set; }

        public  double Total_Achat { get; set; }

		public  DateTime DateAchat { get; set; }

    }
}

