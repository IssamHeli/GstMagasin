using System;
using Microsoft.EntityFrameworkCore;
using TestHostAppAndBaseDonnes.Models;

namespace GstMagazin.Data
{
	public class GstDbMagazin :DbContext
	{
		public GstDbMagazin()
		{
		}

        public GstDbMagazin(DbContextOptions options) : base(options)
        {
        }
		public DbSet<Directeur> Directeurs { get; set; }
		public DbSet<Vondoure> Vondoures { get;set; }
		public DbSet<Acheteur> Acheteurs { get;set; }
		public DbSet<Produit> Produits { get;set; }
		public DbSet<PlanAchat> PlanAchats { get;set; }
		public DbSet<PlanVondre> PlanVondres { get;set; }
        public DbSet<ProduitAnalyse> ProduitAnalyses { get; set; }

        public DbSet<ProduitAffiche> ProduitAffiches { get; set; }

        public DbSet<Categorie> Categorie { get;set; }
		public DbSet<LieuStock> LieuStock { get;set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }

        public DbSet<UserContact> userscontact { get; set; }
    }
}

