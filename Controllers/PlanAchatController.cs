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
    public class PlanAchatController : Controller
    {
        private readonly GstDbMagazin _context;

        public PlanAchatController(GstDbMagazin context)
        {
            _context = context;
        }

        // GET: PlanAchat
        [HttpGet]
        public async Task<IActionResult> Index(int? pageIndex)
        {

            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }


            var pl = await _context.PlanAchats.ToListAsync();

            if (pl != null)
            {
                int pageSize = 15; // Number of items per page

                var data = _context.PlanAchats.Include(p=>p.produit).OrderByDescending(p => p.DateAchat).AsQueryable(); // Replace with your DbSet

                var paginatedDataaa = await PaginatedList<PlanAchat>.CreateAsync(data, pageIndex ?? 1, pageSize);
                return View(paginatedDataaa);

            }
            else
            {
                return Problem("la table PlanAchats Est Vide ");
            }
        }

        // GET: PlanAchat/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.PlanAchats == null)
            {
                return NotFound();
            }

            var planAchat = await _context.PlanAchats
                .Include(p => p.produit)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planAchat == null)
            {
                return NotFound();
            }

            return View(planAchat);
        }

       
        /*
          
        // GET: PlanAchat/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.PlanAchats == null)
            {
                return NotFound();
            }

            var planAchat = await _context.PlanAchats.FindAsync(id);
            if (planAchat == null)
            {
                return NotFound();
            }
            ViewData["ProduitId"] = new SelectList(_context.Produits, "id", "Nom", planAchat.ProduitId);
            return View(planAchat);
        }

        // POST: PlanAchat/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,ProduitId,Qnt_achat,Prix_achat,Total_Achat,DateAchat")] PlanAchat planAchat)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            if (id != planAchat.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planAchat);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(actionName:"Details",controllerName:"PlanAchat", new {id = planAchat.id});
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanAchatExists(planAchat.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["ProduitId"] = new SelectList(_context.Produits, "id", "id", planAchat.ProduitId);
            return View(planAchat);
        }

        // GET: PlanAchat/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.PlanAchats == null)
            {
                return NotFound();
            }

            var planAchat = await _context.PlanAchats
                .Include(p => p.produit)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planAchat == null)
            {
                return NotFound();
            }

            return View(planAchat);
        }

        // POST: PlanAchat/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            if (_context.PlanAchats == null)
            {
                return Problem("Entity set 'GstDbMagazin.PlanAchats'  is null.");
            }
            var planAchat = await _context.PlanAchats.FindAsync(id);
            if (planAchat != null)
            {
                _context.PlanAchats.Remove(planAchat);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanAchatExists(int id)
        {
          return (_context.PlanAchats?.Any(e => e.id == id)).GetValueOrDefault();
        }

        */

        [HttpPost]
        public async Task<IActionResult> ChercherproduitReference()
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            string? Reference = Request.Form["Reference"].ToString();

            if (Reference == null || _context.PlanAchats == null)
            {
                return View();
            }

            var PlanAchats = await _context.PlanAchats.Include(p => p.produit).Where(p => p.Reference == Reference).ToListAsync();
            if (PlanAchats == null)
            {
                return View();
            }
            ViewData["ReferenceChercher"] = Reference;
            ViewData["totalparReference"] =PlanAchats.Sum(p => p.Total_Achat).ToString() + " DH";
            return View(PlanAchats);



        }

        [HttpPost]
        public async Task<IActionResult> Chercherproduitnom()
        {
            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return RedirectToAction(actionName: "LoginDirecteur", controllerName: "Directeur");
            }
            string? nom = Request.Form["produitnom"].ToString();

            if (nom == null || _context.PlanAchats == null)
            {
                return View();
            }

            var PlanAchats = await _context.PlanAchats.Include(p => p.produit).Where(p => p.produit.Nom == nom).ToListAsync();
            if (PlanAchats == null)
            {
                return View();
            }
            
            ViewData["nomchercher"] = nom;
            ViewData["totalparnom"] ="' "+ nom + " ' est: " + PlanAchats.Sum(p => p.Total_Achat).ToString() + " DH";
            return View(PlanAchats);



        }
        [HttpPost]
        public async Task<IActionResult> Chercherdateachat()
        {
            
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            var date = DateTime.Parse(Request.Form["dateachat"].ToString()).ToShortDateString();

            if (date == null || _context.PlanAchats == null)
            {
                return View();
            }

            var PlanAchats = await _context.PlanAchats.Include(p => p.produit).ToListAsync();

            if (PlanAchats == null)
            {
                return View();
            }

            var pl = PlanAchats.Where(p => p.DateAchat.ToShortDateString() == date);
            ViewData["datechercher"] = date;

            ViewData["totalachatpardate"] = "' "+ date.ToString() + " ' est : " + pl.Sum(p => p.Total_Achat).ToString() +" DH";
            return View(pl);



        }
    }
}
