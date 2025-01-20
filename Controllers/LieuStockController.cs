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
    public class LieuStockController : Controller
    {
        private readonly GstDbMagazin _context;



        public LieuStockController(GstDbMagazin context )
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var lieuStocks= await _context.LieuStock.ToListAsync();

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(lieuStocks);
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
        public async Task<IActionResult> Create([Bind("id,Designation")] LieuStock lieuStock)
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            if (ModelState.IsValid)
            {
                _context.LieuStock.Add(lieuStock);

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
            if (id == null || _context.LieuStock == null)
            {
                return NotFound();
            }

            var LieuStocks = await _context.LieuStock.FindAsync(id);
            if (LieuStocks == null)
            {
                return NotFound();
            }

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(LieuStocks);
        }

        // POST: Acheteur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Designation")] LieuStock lieuStock)
        {

            if (id != lieuStock.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    try
                    {
                        _context.Update(lieuStock);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("index");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LieuStockExists(lieuStock.id))
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
            return View(lieuStock);
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LieuStock == null)
            {
                return NotFound();
            }

            var LieuStocks = await _context.LieuStock
                .FirstOrDefaultAsync(m => m.id == id);
            if (LieuStocks == null)
            {
                return NotFound();
            }

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;
            return View(LieuStocks);
        }

        // POST: Directeur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LieuStock == null)
            {
                return Problem("Entity set 'GstDbMagazin.Directeurs'  is null.");
            }
            var LieuStocks= await _context.LieuStock.FindAsync(id);
            if (LieuStocks != null)
            {
                _context.LieuStock.Remove(LieuStocks);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LieuStockExists(int id)
        {
          return (_context.LieuStock?.Any(e => e.id == id)).GetValueOrDefault();
        }


        [HttpPost]
        public async Task<IActionResult> Chercherpardesignation()
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            string? acheteurs = HttpContext.Session.GetString("acheteur");
            string? Designation = Request.Form["Designation"].ToString();

            if (Designation == null || _context.LieuStock == null)
            {
                ViewData["DesignationNotTrouver"] = "Les Lieux De Stock Avec la Designation   : '" + Designation + "' N'exists pas";
                return View();
            }
            
            var LieuStock = await _context.LieuStock.Where(p => p.Designation == Designation).AnyAsync();
            if (LieuStock == false)
            {
                ViewData["DesignationNotTrouver"] = "Les Lieux De Stock Avec la Designation : '"+ Designation + "' N'exists pas";
                ViewData["DesignationChercher"] = Designation;

                ViewBag.directeurlogin = directeurs;
                ViewBag.acheteurslogin = acheteurs;
                
                return View();
            }


            ViewData["DesignationChercher"] = Designation;



            ViewBag.directeurlogin = directeurs;
            ViewBag.acheteurslogin = acheteurs;

            var LieuStocks = await _context.LieuStock.Where(p => p.Designation == Designation).ToListAsync();
            return View(LieuStocks);

        }

    }
}