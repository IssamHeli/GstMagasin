using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestHostAppAndBaseDonnes.Models
{
	public class Produit
	{
        [Key]
        public int id { get; set; }
        public required string Reference {get;set;}  
        public required string Nom { get; set; }
        public required int quantity { get; set; }


        public required double Prix_unity { get; set; }


        public string Categorie {get;set;}
        public string LieuDeStock {get;set;}

    }
}

