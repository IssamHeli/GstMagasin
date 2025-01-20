using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GstMagazin.Data;
using TestHostAppAndBaseDonnes.Models;
using System.Security.Cryptography.Xml;
using System.Data;

namespace GstMagazin.Controllers
{
    public class ProduitAcheteurController : Controller
    {
        private readonly GstDbMagazin _context;

        public ProduitAcheteurController(GstDbMagazin context)
        {
            _context = context;
        }

        // GET: ProduitAcheteur
        public async Task<IActionResult> Index(int? pageIndex)
        {

            // Retrieve Directeur's ID from local storage
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "loginAcheteur");
            }


            if (_context.Produits != null)
                {
                    int pageSize = 15; // Number of items per page

                    var data = _context.Produits.OrderByDescending(p => p.id).AsQueryable(); // Replace with your DbSet

                    var paginatedData = await PaginatedList<Produit>.CreateAsync(data, pageIndex ?? 1, pageSize);


                    
                    ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
                    ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation");
                    return View(paginatedData);


                }
                else
                {
                    return Problem("La Table Des Produits Est Vide");
                }
            
        }
        // GET: ProduitAcheteur/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits
                .FirstOrDefaultAsync(m => m.id == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

         public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation");
            return View();
        }

        // POST: Produit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Reference,Nom,quantity,Prix_unity,Categorie,LieuDeStock")] Produit produit)
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            if (ModelState.IsValid)
            {
                
                _context.Produits.Add(produit);
                
                PlanAchat pl = new PlanAchat();
                pl.produit = produit;
                pl.ProduitId = produit.id;
                pl.Reference = produit.Reference;
                pl.Prix_achat = produit.Prix_unity;
                pl.Qnt_achat = produit.quantity;
                pl.Total_Achat = pl.Prix_achat * pl.Qnt_achat;
                pl.DateAchat = moroccoTime;

                _context.PlanAchats.Add(pl);

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });
            }
            return View();
        }

        // GET: ProduitAcheteur/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
            {
                return NotFound();
            }

            var plv = _context.PlanVondres.Where(pv => pv.Produitid == id).Any();
            if (plv == true)
            {
                var plvv = _context.PlanVondres.Where(pv => pv.Produitid == id).ToList();
                var totalquantitievente = plvv.Sum(pv => pv.Qnt_vondre);
                ViewData["QuantitiéDejaVenter"] = totalquantitievente.ToString();
                

            }
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation");
            return View(produit);
        }

        // POST: ProduitAcheteur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Reference,Nom,quantity,Categorie,LieuDeStock,Prix_unity")] Produit produit)
        {

            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id != produit.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pl = await _context.PlanAchats.Where(p => p.ProduitId == id).FirstAsync();

                    if(pl != null)
                    {
                        if ((pl.DateAchat).AddHours(1) < moroccoTime)
                        {
                            ViewData["Vouspouverpas"] = "Vous ne pouvez pas modifier ce produit, contactez votre directeur pour le régler.";
                            
                            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
                            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation"); 
                            return View(produit);
                        }

                        var plv = _context.PlanVondres.Where(pv => pv.Produitid == id).Any();
                        if (plv == true)
                        {
                            var plvv = _context.PlanVondres.Where(pv => pv.Produitid == id).ToList();
                            var totalquantitievente = plvv.Sum(pv => pv.Qnt_vondre);

                            if (produit.quantity >= totalquantitievente)
                            {
                                pl.produit = produit;
                                pl.Reference = produit.Reference;
                                pl.Prix_achat = produit.Prix_unity;
                                pl.Qnt_achat = produit.quantity;
                                pl.Total_Achat = pl.Prix_achat * pl.Qnt_achat;

                                _context.PlanAchats.Update(pl);

                                produit.quantity = pl.Qnt_achat - totalquantitievente;

                                _context.Produits.Update(produit);
                                await _context.SaveChangesAsync();
                                return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });

                            }
                            else
                            {
                                ViewData["QuantitiéDejaVenter"] = totalquantitievente.ToString();
                                ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
                                ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation");
                                ViewData["NvqntPlusDeTotalVente"] = "Quantité Stock est insuffisante , N'oublie pas d'ajouter 'Quantitie Deja Vondue' a votre 'Quantitie Stock";
                                return View(produit);
                            }

                        }
                        pl.produit = produit;
                        pl.Reference = produit.Reference;
                        pl.Prix_achat = produit.Prix_unity;
                        pl.Qnt_achat = produit.quantity;
                        pl.Total_Achat = pl.Prix_achat * pl.Qnt_achat;

                        _context.PlanAchats.Update(pl);
                        _context.Produits.Update(produit);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduitExists(produit.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            var plvv1 = _context.PlanVondres.Where(pv => pv.Produitid == id).ToList();
            var totalquantitievente1 = plvv1.Sum(pv => pv.Qnt_vondre);
            ViewData["QuantitiéDejaVenter"] = totalquantitievente1.ToString();
            
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation"); 
            return View(produit);
        }
        /*
        // GET: ProduitAcheteur/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {

            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "Create", controllerName: "Directeur");
            }
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits
                .FirstOrDefaultAsync(m => m.id == id);
            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // POST: ProduitAcheteur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "Create", controllerName: "Directeur");
            }
            if (_context.Produits == null)
            {
                return Problem("Entity set 'GstDbMagazin.Produits'  is null.");
            }
            var produit = await _context.Produits.FindAsync(id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        private bool ProduitExists(int id)
        {
          return (_context.Produits?.Any(e => e.id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ChercherId()
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            int? id = int.Parse(Request.Form["id"].ToString());

            if (id == null || _context.Produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec Id : '" + id.ToString() + "' N'exist Pas";
                return View();
            }
            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
            {
                
                ViewData["IdNotTrouver"] = "Le Produit Avec Id : '" + id.ToString() + "' N'exist Pas";
                return View();
            }

            ViewData["IdChercher"] = Convert.ToString(id);
            return View(produit);
        }

        [HttpPost]
        public async Task<IActionResult> ChercherNom()
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            string? nom = Request.Form["Nom"].ToString();

            if (nom == null || _context.Produits == null)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + nom + "' N'exists pas";
                return View();
            }

            var produits = await _context.Produits.Where(p => p.Nom == nom).AnyAsync();
            if (produits == false)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + nom + "' N'exists pas";
                return View();
            }

            var produitss = await _context.Produits.Where(p => p.Nom == nom).ToListAsync();
            ViewData["NomChercher"] = "Nom produit : " + nom;
            ViewData["TotalqntParProduit"] = "Total Quantité Stock : " + produitss.Sum(p => p.quantity).ToString();
            return View(produitss);
        }

        
    public async Task<IActionResult> Vondre(int? id)
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits.FindAsync(id);
            if (produit == null)
            {
                return NotFound();
            }

            if(produit.quantity == 0)
            {
                ViewData["stockage"] = "Quantité Stock insuffisante";
            }
            return View(produit);
        }

        // POST: Produit/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vondre(int id, [Bind("id,Reference,Nom,quantity,Prix_unity,Categorie,LieuDeStock")] Produit produit)
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
           string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id != produit.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int qnt_Vondre = int.Parse(Request.Form["Qnt_vondre"].ToString());
                    double prix_Vondre = double.Parse(Request.Form["Prix_Vondre"].ToString());

                    if (produit.quantity >= qnt_Vondre)
                    {
                        PlanVondre pv = new PlanVondre();
                        pv.Produitid = produit.id;
                        pv.produit = produit;
                        pv.Reference = produit.Reference;
                        pv.Prix_Vondre = prix_Vondre;
                        pv.Qnt_vondre = qnt_Vondre;
                        pv.Total_Vondre = pv.Prix_Vondre * pv.Qnt_vondre;
                        pv.DateVondre = moroccoTime;
                       
                        _context.PlanVondres.Add(pv);

                        produit.quantity -= pv.Qnt_vondre;

                        _context.Update(produit);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });

                    }
                    else
                    {

                        ViewData["stockage"] = "Quantité Stock insuffisante";
                        return View(produit);
                    }
                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProduitExists(produit.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(produit);
        }


        
        [HttpPost]
        public async Task<IActionResult> Chercherquantite()
        {
            int? Quantite = int.Parse(Request.Form["Quantite"].ToString());

            if (Quantite == null || _context.Produits == null)
            {
                ViewData["QuantiteNotTrouver"] = "Les Produits Avec la Quantite  : '" + Quantite + "' N'exists pas";
                return View();
            }
            
            var produits = await _context.Produits.Where(p => p.quantity == Quantite).AnyAsync();
            if (produits == false)
            {
                ViewData["QuantiteNotTrouver"] = "Les Produits Avec la Quantite  : '"+ Quantite + "' N'exists pas";
                return View();
            }
            var produitss = await _context.Produits.Where(p => p.quantity == Quantite).ToListAsync();
            ViewData["QuantiteChercher"] = "Quantite Charcher Par est  : " + Quantite;

            List<ProduitQnt0export> prs0 = new List<ProduitQnt0export>();

            foreach(var pr in produitss)
            {
                var planAchat = _context.PlanAchats.Where(pl => pl.ProduitId == pr.id).ToList();

                var planvonte = _context.PlanVondres.Where(pl => pl.Produitid == pr.id);

                ProduitQnt0export ProduitQnt0exporte = new ProduitQnt0export();
                ProduitQnt0exporte.id = new Guid();
                ProduitQnt0exporte.produit = pr;
                
                ProduitQnt0exporte.QuantiteAchat = planAchat.Sum(pl => pl.Qnt_achat);
                ProduitQnt0exporte.TotalAchat = planAchat.Sum(pl=>pl.Total_Achat);
                ProduitQnt0exporte.TotalVonte = planvonte.Sum(pv=>pv.Total_Vondre);
                ProduitQnt0exporte.Profite = (double)(ProduitQnt0exporte.TotalVonte - ProduitQnt0exporte.TotalAchat);

                prs0.Add(ProduitQnt0exporte);



            }
            return View(prs0);

        }




        [HttpPost]
        public async Task<IActionResult> ChercherReference()
        {
            string? reference = Request.Form["Reference"].ToString();

            if (reference == null || _context.Produits == null)
            {
                ViewData["ReferenceNotTrouver"] = "Les Produits Avec la Reference  : '" + reference + "' N'exists pas";
                return View();
            }
            
            var produits = await _context.Produits.Where(p => p.Reference == reference).AnyAsync();
            if (produits == false)
            {
                ViewData["ReferenceNotTrouver"] = "Les Produits Avec la reference  : '"+ reference + "' N'exists pas";
                return View();
            }
            var produitss = await _context.Produits.Where(p => p.Reference == reference).ToListAsync();
            ViewData["ReferenceChercher"] = "Reference produit : " + reference;
            ViewData["TotalqntParReference"] = "Total Quantité Stock Par Reference : " + produitss.Sum(p => p.quantity).ToString();
            return View(produitss);

        }




        [HttpPost]
        public async Task<IActionResult> ChercherCategorie()
        {
            string? categorie = Request.Form["categorie"].ToString();

            if (categorie == null || _context.Produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec Categorie : '" + categorie + "' N'exist Pas";
                return View();
            }
            var produits = await _context.Produits.Where(p =>p.Categorie == categorie).ToListAsync();
            if (produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec categorie : '" + categorie+ "' N'exist Pas";
                return View();
            }

            ViewData["CategorieChercher"] = categorie;
            ViewData["TotalqntParCategorie"] = produits.Sum(p=>p.quantity).ToString();
            return View(produits);
        }
        
        [HttpPost]
        public async Task<IActionResult> ChercherLieuStock()
        {
            string? LieuStock = Request.Form["LieuDeStock"].ToString();

            if (LieuStock == null || _context.Produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec Lieu De Stock  : '" + LieuStock + "' N'exist Pas";
                return View();
            }
            var produits = await _context.Produits.Where(p =>p.LieuDeStock == LieuStock).ToListAsync();
            if (produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec Lieu De Stock  : '" + LieuStock+ "' N'exist Pas";
                return View();
            }

            ViewData["LieuStockChercher"] = LieuStock;
            ViewData["TotalqntParLieuStock"] = produits.Sum(p=>p.quantity).ToString();
            return View(produits);
        }

        
        [HttpGet]
        public IActionResult AjouterQnt(int? id)
        {
            var produit = _context.Produits.FirstOrDefault(p => p.id == id);
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");
            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c => c.id), "Designation", "Designation");
            return View(produit);
        }


        [HttpPost]
        public async Task<IActionResult> AjouterQnt(int id, [Bind("id,Reference,Nom,quantity,Categorie,LieuDeStock,Prix_unity")] Produit produit)
        {


            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();


            string selectedCard = Request.Form["cardSelection"].ToString();

            if (ModelState.IsValid)
            {

                int nouveauQnt = int.Parse(Request.Form["NouvauxQnt1"].ToString());

                PlanAchat pl = new PlanAchat();
                pl.produit = produit;
                pl.ProduitId = produit.id;
                pl.Reference = produit.Reference;
                pl.Prix_achat = produit.Prix_unity;
                pl.Qnt_achat = nouveauQnt;
                pl.Total_Achat = pl.Prix_achat * pl.Qnt_achat;
                pl.DateAchat = moroccoTime;

                _context.PlanAchats.Add(pl);



                produit.quantity += nouveauQnt;

                _context.Produits.Update(produit);




                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });
            }

            return View();
        }



        [HttpGet]
        public IActionResult ExportProduits()
        {
            var produit = _context.Produits.ToList();

            if(produit == null)
            {
                ViewBag.Produit = "la table des produit est vide ";
                return View();
            }
            var listpr = new List<ProduitQnt0export>();

            foreach(var item in produit)
            {
                var allachat = _context.PlanAchats.Where(p => p.ProduitId == item.id).ToList();
                var allVentes = _context.PlanVondres.Where(p => p.Produitid == item.id).ToList();

                var pr = new ProduitQnt0export();

                pr.id = new Guid();
                pr.produit = item;

                pr.QuantiteAchat = allachat.Sum(p => p.Qnt_achat);
                pr.TotalAchat = allachat.Sum(p => p.Total_Achat);
                pr.TotalVonte = allVentes.Sum(p => p.Total_Vondre);

                pr.Profite = (double)(pr.TotalVonte - pr.TotalAchat);

                listpr.Add(pr);
            }

            return View(listpr);
        }

    }

}
