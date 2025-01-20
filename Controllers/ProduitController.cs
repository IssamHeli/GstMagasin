using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GstMagazin.Data;
using TestHostAppAndBaseDonnes.Models;
using Newtonsoft.Json;
using System.Drawing;
using GstMagazin.Models;

namespace GstMagazin.Controllers
{
    public class ProduitController : Controller
    {
        private readonly GstDbMagazin _context;

        public ProduitController(GstDbMagazin context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int? pageIndex)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");
            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
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
                return Problem("Entity set 'GstDbMagazin.Produits'  is null.");
            }
            /*
            List<ProduitQnt0export> prs = new List<ProduitQnt0export>();
            var produits = _context.Produits.OrderByDescending(p => p.id);

            foreach(var item in produits)
            {
                ProduitQnt0export pr = new ProduitQnt0export();
                pr.id = new Guid();
                pr.produit = item;
                pr.QuantiteAchat = _context.PlanAchats.Where(pl => pl.ProduitId == item.id).Sum(pl=>pl.Qnt_achat);
                pr.TotalAchat = _context.PlanAchats.Where(pl => pl.ProduitId == item.id).Sum(pl=>pl.Total_Achat);
                pr.TotalVonte = _context.PlanVondres.Where(pl => pl.Produitid == item.id).Sum(pl=>pl.Total_Vondre);
                pr.Profite = pr.TotalVonte - pr.TotalAchat;
                prs.Add(pr);
            }*/

        }





        // GET: Produit/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Produits == null)
            {
                return NotFound();
            }

            var produit = await _context.Produits.FirstOrDefaultAsync(m => m.id == id);

            if (produit == null)
            {
                return NotFound();
            }

            return View(produit);
        }

        // GET: Produit/Create
        public IActionResult Create()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
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

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

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
                if(directeurs != null)
                {
                    return RedirectToAction("Details", "Produit", new { id = produit.id });
                }else if (acheteurs != null)
                {
                    return RedirectToAction("Details", "ProduitAcheteur", new { id = produit.id });
                }

            }
            return View();
        }

        // GET: Produit/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Reference,Nom,quantity,Categorie,LieuDeStock,Prix_unity")] Produit produit)
        {

            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            if (id != produit.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var pl =  _context.PlanAchats.FirstOrDefault(p => p.ProduitId == id);

                    if(pl != null)
                    {
                        var plv = _context.PlanVondres.Where(pv => pv.Produitid == id).Any();
                        if (plv == true)
                        {
                            var plvv = _context.PlanVondres.Where(pv => pv.Produitid == id).ToList();
                            var totalquantitievente = plvv.Sum(pv => pv.Qnt_vondre);

                            if(produit.quantity >= totalquantitievente)
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
                                return RedirectToAction("Details", "Produit", new { id = produit.id });
                            }
                            else
                            {
                               
                                ViewData["QuantitiéDejaVenter"] = totalquantitievente.ToString();
                                ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c=>c.id), "Designation", "Designation");
                                ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c=>c.id), "Designation", "Designation");
                                ViewData["NvqntPlusDeTotalVente"] = "Vous ne pouvez pas enregistrer le produit. N'oubliez pas d'ajouter la 'Quantité déjà vendue' à votre 'Quantité stock'." ;
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
                        return RedirectToAction("Details", "Produit", new { id = produit.id });
                        
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

        // GET: Produit/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
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

        // POST: Produit/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (_context.Produits == null)
            {
                return Problem("La List Des Produit Est Vide !!");
            }
            var produit = await _context.Produits.FindAsync(id);
            if (produit != null)
            {
                _context.Produits.Remove(produit);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Vondre(int? id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
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

            if (produit.quantity == 0)
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
        public async Task<IActionResult> Vondre(int id, [Bind("id,Reference,Nom,quantity,Categorie,LieuDeStock,Prix_unity")] Produit produit)
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
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
                        PlanVondre pl = new PlanVondre();
                        pl.Produitid = produit.id;
                        pl.produit = produit;
                        pl.Reference = produit.Reference;
                        pl.Prix_Vondre = prix_Vondre;
                        pl.Qnt_vondre = qnt_Vondre;
                        pl.Total_Vondre = pl.Prix_Vondre * pl.Qnt_vondre;
                        pl.DateVondre = moroccoTime;

                        _context.PlanVondres.Add(pl);

                        produit.quantity -= pl.Qnt_vondre;

                        _context.Update(produit);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", "Produit", new { id = produit.id });
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
                ViewData["QuantiteNotTrouver"] = "Les Produits Avec la Quantite  : '" + Quantite + "' N'exists pas";
                return View();
            }
            var produitss = await _context.Produits.Where(p => p.quantity == Quantite).ToListAsync();
            ViewData["QuantiteChercher"] = "Quantite Charcher Par est  : " + Quantite;

            List<ProduitQnt0export> prs0 = new List<ProduitQnt0export>();

            foreach (var pr in produitss)
            {
                var planAchat = _context.PlanAchats.Where(pl => pl.ProduitId == pr.id).ToList();
                var planvonte = _context.PlanVondres.Where(pl => pl.Produitid == pr.id);


                ProduitQnt0export ProduitQnt0exporte = new ProduitQnt0export();
                ProduitQnt0exporte.id = new Guid();
                ProduitQnt0exporte.produit = pr;

                ProduitQnt0exporte.QuantiteAchat = planAchat.Sum(pl => pl.Qnt_achat);

                ProduitQnt0exporte.TotalAchat = planAchat.Sum(pl => pl.Total_Achat);

                ProduitQnt0exporte.QuantiteVente = planvonte.Sum(pl => pl.Qnt_vondre);

                ProduitQnt0exporte.TotalVonte = planvonte.Sum(pv => pv.Total_Vondre);

                ProduitQnt0exporte.Profite = (double)(ProduitQnt0exporte.TotalVonte - ProduitQnt0exporte.TotalAchat);

                prs0.Add(ProduitQnt0exporte);



            }
            return View(prs0);

        }


        [HttpGet]
        public async Task<IActionResult> acheter(int? id)
        {
            if (id == null)
            {
                return View();
            }
            var produitaffiche = await _context.ProduitAffiches.FindAsync(id);
            if (produitaffiche == null)
            {
                return View();
            }

            var produit = new Produit
            {
                Reference = produitaffiche.Reference,
                Nom = produitaffiche.Nom,

                quantity = 0,
                Categorie = produitaffiche.Categorie,
                Prix_unity = 0.00,
            };
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");
            ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c => c.id), "Designation", "Designation");

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            return View(produit);
        }

        // POST: Produit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> acheter([Bind("id,Reference,Nom,quantity,Prix_unity,Categorie,LieuDeStock")] Produit produit)
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
                return RedirectToAction("Details", "Produit", new { id = produit.id });
            }
            return View();
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
        [HttpPost]
        public async Task<IActionResult> ChercherNom()
        {
            string? nom = Request.Form["Nom"].ToString();

            if (nom == null || _context.Produits == null)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + nom + "' N'exists pas";
                return View();
            }
            
            var produits = await _context.Produits.Where(p => p.Nom == nom).AnyAsync();
            if (produits == false)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '"+ nom + "' N'exists pas";
                return View();
            }
            var produitss = await _context.Produits.Where(p => p.Nom == nom).ToListAsync();
            ViewData["NomChercher"] = "Nom produit : " + nom;
            ViewData["TotalqntParProduit"] = "Total Quantité Stock Par Nom: " + produitss.Sum(p => p.quantity).ToString();
            return View(produitss);

        }
        [HttpPost]
        public async Task<IActionResult> Chercherquantity()
        {
            int? Quantity = int.Parse(Request.Form["Quantity"].ToString());

            if (Quantity == null || _context.Produits == null)
            {
                return View();
            }

            var produits = await _context.Produits.Where(p => p.quantity == Quantity).AnyAsync();
            if (produits == false)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + Quantity + "' N'exists pas";
                return View();
            }
            var produitss = await _context.Produits.Where(p => p.quantity == Quantity).ToListAsync();
            ViewData["NomChercher"] = "Quantity stockée : " + Quantity;
            ViewData["TotalqntParProduit"] = "Total Des produits avec la meme quantity stockée est  : " + produitss.Count().ToString();
            return View(produitss);





        }
        private bool ProduitExists(int id)
        {
            return (_context.Produits?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpGet]
        public async Task<IActionResult> Analyse(int? id)
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }

            if (id != null)
            {
                double totalachat = 0;
                double totalvondre = 0;
                var planachats = await _context.PlanAchats.Where(p => p.produit.id == id).ToListAsync();
                var PlanVondre = await _context.PlanVondres.Where(p => p.produit.id == id).ToListAsync();
                var produit = await _context.Produits.Where(p => p.id == id).FirstAsync();
                foreach (var item in planachats)
                {
                    totalachat += item.Total_Achat;
                }
                foreach (var item in PlanVondre)
                {
                    totalvondre += item.Total_Vondre;
                }

                ProduitAnalyse pa = new ProduitAnalyse();
                pa.Produit = produit;
                pa.ProduitId = (int)id;
                pa.Total_Achats = totalachat;
                pa.Total_Ventes = totalvondre;
                pa.Profite = pa.Total_Ventes - pa.Total_Achats;

                return View(pa);
            }

            return View();
        }


        [HttpGet]

        public IActionResult AjouterQnt(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            var produit = _context.Produits.FirstOrDefault(p=>p.id == id);

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
                return RedirectToAction("Details", "Produit", new { id = produit.id });
            }

            return View();
        }




        [HttpGet]
        public IActionResult ExportProduits()
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null )
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            var produit =  _context.Produits.ToList();

            if (produit == null)
            {
                ViewBag.Produit = "la table des produit est vide ";
                return View();
            }
            var listpr = new List<ProduitQnt0export>();

            foreach (var item in produit)
            {
                var allachat = _context.PlanAchats.Where(p => p.ProduitId == item.id).ToList();
                var allVentes = _context.PlanVondres.Where(p => p.Produitid == item.id).ToList();

                var pr = new ProduitQnt0export();

                pr.id = new Guid();

                pr.produit = item;

                pr.QuantiteAchat = allachat.Sum(p => p.Qnt_achat);

                pr.TotalAchat = allachat.Sum(p => p.Total_Achat);


                pr.QuantiteVente = allVentes.Sum(p => p.Qnt_vondre);

                pr.TotalVonte = allVentes.Sum(p => p.Total_Vondre);

                pr.Profite = (double)(pr.TotalVonte - pr.TotalAchat);

                listpr.Add(pr);
            }

            return View(listpr);
        }

    }
    
}
