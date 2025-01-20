using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GstMagazin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TestHostAppAndBaseDonnes.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GstMagazin.Controllers
{
    public class ProduitAfficheController : Controller
    {
        private readonly GstDbMagazin _context;

        public ProduitAfficheController(GstDbMagazin context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(int? pageIndex)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if(directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            if (_context.ProduitAffiches != null)
            {
                int pageSize = 15; // Number of items per page

                var data = _context.ProduitAffiches.OrderByDescending(p => p.id).AsQueryable(); // Replace with your DbSet

                var paginatedData = await PaginatedList<ProduitAffiche>.CreateAsync(data, pageIndex ?? 1, pageSize);


                ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");
                ViewBag.Lieudestocks = new SelectList(_context.LieuStock.OrderByDescending(c => c.id), "Designation", "Designation");
                
                return View(paginatedData);


            }
            else
            {
                return Problem("Entity set 'GstDbMagazin.Produits'  is null.");
            }

        }

        public IActionResult Create()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Create([Bind("id,Reference,Nom,Categorie")] ProduitAffiche produitAffiche)
        {
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
                _context.ProduitAffiches.Add(produitAffiche);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");
            return View(produitAffiche);

        }

        [HttpPost]
        public async Task<IActionResult> ChercherNom()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            string? nom = Request.Form["Nom"].ToString();

            if (nom == null || _context.ProduitAffiches == null)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + nom + "' N'exists pas";
                return View();
            }

            var produits = await _context.ProduitAffiches.Where(p => p.Nom == nom).AnyAsync();
            if (produits == false)
            {
                ViewData["NomNotTrouver"] = "Les Produits Avec le Nom  : '" + nom + "' N'exists pas";
                return View();
            }
            var produitss = await _context.ProduitAffiches.Where(p => p.Nom == nom).ToListAsync();
            ViewData["NomChercher"] = "Nom Article : " + nom;
            return View(produitss);

        }
        [HttpPost]
        public async Task<IActionResult> ChercherReference()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            string? reference = Request.Form["Reference"].ToString();

            if (reference == null || _context.ProduitAffiches == null)
            {
                ViewData["ReferenceNotTrouver"] = "Les Affiches des  Produits Avec la Reference  : '" + reference + "' N'exists pas";
                return View();
            }

            var produits = await _context.ProduitAffiches.Where(p => p.Reference == reference).AnyAsync();
            if (produits == false)
            {
                ViewData["ReferenceNotTrouver"] = "Les Affiches des Produits Avec la reference  : '" + reference + "' N'exists pas";
                return View();
            }
            var produitss = await _context.ProduitAffiches.Where(p => p.Reference == reference).ToListAsync();
            ViewData["ReferenceChercher"] = "Reference produit chercher : " + reference;
            return View(produitss);

        }
        [HttpPost]
        public async Task<IActionResult> ChercherCategorie()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            string? categorie = Request.Form["categorie"].ToString();

            if (categorie == null || _context.ProduitAffiches == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec Categorie : '" + categorie + "' N'exist Pas";
                return View();
            }
            var produits = await _context.ProduitAffiches.Where(p => p.Categorie == categorie).ToListAsync();
            if (produits == null)
            {
                ViewData["IdNotTrouver"] = "Le Produit Avec categorie : '" + categorie + "' N'exist Pas";
                return View();
            }

            ViewData["CategorieChercher"] = categorie;
            return View(produits);
        }
        [HttpGet]
        public async Task<IActionResult> delete(int? id)
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            if (id == null || _context.ProduitAffiches == null)
            {
                return NotFound();
            }

            var produitaffiche = await _context.ProduitAffiches.FindAsync(id);
            if (produitaffiche == null)
            {
                return NotFound();
            }

            return View(produitaffiche);
        }

        [HttpPost, ActionName("delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (_context.ProduitAffiches == null)
            {
                return Problem("La List Des Produit Est Vide !!");
            }
            var produitaffich = await _context.ProduitAffiches.FindAsync(id);
            if (produitaffich != null)
            {
                _context.ProduitAffiches.Remove(produitaffich);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if(directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            if (id == null || _context.ProduitAffiches == null)
            {
                return NotFound();
            }

            var produitaffiche = await _context.ProduitAffiches.FindAsync(id);
            if (produitaffiche == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Categorie.OrderByDescending(c => c.id), "Designation", "Designation");

            return View(produitaffiche);
        }

        // POST: Acheteur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Reference,Nom,Categorie")] ProduitAffiche produitAffiche)
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            if (directeurs == null && acheteurs == null)
            {
                return RedirectToAction("loginDirecteur", "Directeur");
            }
            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            if (id != produitAffiche.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(produitAffiche);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!produitafficheExists(produitAffiche.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(produitAffiche);
        }


        private bool produitafficheExists(int id)
        {
            return (_context.ProduitAffiches?.Any(e => e.id == id)).GetValueOrDefault();
        }



    }


}

