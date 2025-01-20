using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GstMagazin.Data;
using TestHostAppAndBaseDonnes.Models;

namespace GstMagazin.Controllers
{
    public class CategorieController : Controller
    {
        private readonly GstDbMagazin _context;



        public CategorieController(GstDbMagazin context )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            var categories = await _context.Categorie.ToListAsync();
            return View(categories);
        }

        [HttpGet]
         public IActionResult Create()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View();
        }

        // POST: Produit/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Designation")] Categorie categorie)
        {
            
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            if (ModelState.IsValid)
            {
                _context.Categorie.Add(categorie);

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Acheteurs == null)
            {
                return NotFound();
            }

            var Categorie = await _context.Categorie.FindAsync(id);
            if (Categorie == null)
            {
                return NotFound();
            }
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(Categorie);
        }

        // POST: Acheteur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Designation")] Categorie categorie)
        {

            if (id != categorie.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    try
                    {
                        _context.Update(categorie);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("index");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategorieExists(categorie.id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
            }
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(categorie);
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categorie == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categorie
                .FirstOrDefaultAsync(m => m.id == id);
            if (categorie == null)
            {
                return NotFound();
            }
            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(categorie);
        }

        // POST: Directeur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categorie == null)
            {
                return Problem("Entity set 'GstDbMagazin.Directeurs'  is null.");
            }
            var categorie = await _context.Categorie.FindAsync(id);
            if (categorie != null)
            {
                _context.Categorie.Remove(categorie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategorieExists(int id)
        {
          return (_context.Categorie?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpPost]
        public async Task<IActionResult> Chercherpardesignation()
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            string? Designation = Request.Form["Designation"].ToString();

            if (Designation == null || _context.LieuStock == null)
            {
                ViewData["DesignationNotTrouver"] = "Les Categories Avec la Designation   : '" + Designation + "' N'exists pas";
                return View();
            }
            
            var Categories = await _context.Categorie.Where(p => p.Designation == Designation).AnyAsync();
            if (Categories == false)
            {
                ViewData["DesignationNotTrouver"] = "Les Categories Avec la Designation : '"+ Designation + "' N'exists pas";
                ViewData["DesignationChercher"] = Designation;

                ViewBag.directeurlogin = directeurs;
                ViewBag.acheteurslogin = acheteurs;
                
                return View();
            }


            ViewData["DesignationChercher"] = Designation;



            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            var Categoriess = await _context.Categorie.Where(p => p.Designation == Designation).ToListAsync();
            return View(Categoriess);

        }

    }
}