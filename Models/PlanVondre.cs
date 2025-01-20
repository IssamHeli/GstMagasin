using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestHostAppAndBaseDonnes.Models
{
	public class PlanVondre
	{
        [Key]
        public int id { get; set; }

        public virtual Produit produit { get; set; }
        
        public int Produitid { get; set; }
        public  string Reference { get; set; }

        public  int Qnt_vondre { get; set; }

        public  double Prix_Vondre { get; set; }

        public  double Total_Vondre { get; set; }

        public DateTime DateVondre { get; set; }
    }
}

