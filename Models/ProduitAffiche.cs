using System;
using System.ComponentModel.DataAnnotations;

namespace GstMagazin.Models
{
    public class ProduitAffiche
    {
        [Key]
        public int id { get; set; }
        public required string Reference { get; set; }
        public required string Nom { get; set; }
        public required string Categorie { get; set; }

    }
}

