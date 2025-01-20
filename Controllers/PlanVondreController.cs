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
    public class PlanVondreController : Controller
    {
        private readonly GstDbMagazin _context;

        public PlanVondreController(GstDbMagazin context)
        {
            _context = context;
        }
        // GET: PlanVondre
        [HttpGet]
        public async Task<IActionResult> Index(int? pageIndex)
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            var pl = await _context.PlanVondres.ToListAsync();
            if (pl != null)
            {
                int pageSize = 15; // Number of items per page

                var data = _context.PlanVondres.Include(p => p.produit).OrderByDescending(p => p.DateVondre).AsQueryable(); // Replace with your DbSet


                var paginatedDataaa = await PaginatedList<PlanVondre>.CreateAsync(data, pageIndex ?? 1, pageSize);

                return View(paginatedDataaa);

            }
            else
            {
                return Problem("Entity set 'GstDbMagazin.Produits'  is null.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> IndexForVendeur()
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();
            List<PlanVondre> planVentefiltrer = new List<PlanVondre>();

           string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }

            var PlanVentes = await _context.PlanVondres.Include(p=>p.produit).ToListAsync();

            if (PlanVentes != null)
            {
                foreach (var item in PlanVentes)
                {
                    if (item.DateVondre.Date.ToShortDateString().Equals(moroccoTime.ToShortDateString()))
                    {
                        planVentefiltrer.Add(item);
                    }

                }
                var planVente = planVentefiltrer.OrderByDescending(p => p.DateVondre);
                var totalVenteForThisday = planVentefiltrer.Sum(pl => pl.Total_Vondre);
                ViewData["totalVenteForThisday"] = totalVenteForThisday.ToString();
                return View(planVente);
            }
            
            else
            {
                return Problem("il y'a pas Des Plan Vente Pour aujourd'hui");
            }
        }
        // GET: PlanVondre/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.PlanVondres == null)
            {
                return NotFound();
            }

            var planVondre = await _context.PlanVondres
                .Include(p => p.produit)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planVondre == null)
            {
                return NotFound();
            }

            return View(planVondre);
        }
        [HttpGet]
        public async Task<IActionResult> EditPlanVenteforVendeur(int? id)
        {
           string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            
            if (id == null || _context.PlanVondres == null)
            {
                return NotFound();
            }

            var planVondre = await _context.PlanVondres.FindAsync(id);
            if (planVondre == null)
            {
                return NotFound();
            }
            ViewData["Produitid"] = new SelectList(_context.Produits, "id", "Nom", planVondre.Produitid);
            var produit = _context.Produits.Where(p => p.id == planVondre.Produitid).FirstOrDefault();
            if (produit == null)
            {
                return NotFound();
            }
            ViewData["Produitnom"] = produit.Nom.ToString();
            ViewData["ancienquantitieVente"] = planVondre.Qnt_vondre.ToString();
            return View(planVondre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlanVenteforVendeur(int id, [Bind("id,Reference,Produitid,Qnt_vondre,Prix_Vondre,Total_Vondre,DateVondre")] PlanVondre planVondre)
        {
            DateTime moroccoTime = TimeZoneHelper.GetCurrentMoroccoTime();

           string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {

                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }

            int ancienquantitieVente = int.Parse(Request.Form["ancienquantitieVente"].ToString());
            var produit = _context.Produits.Where(p => p.id == planVondre.Produitid).FirstOrDefault();
            if (id != planVondre.id)
            {
                return NotFound();
            }

            if (produit == null)
            {
                return NotFound();
            }

            if(planVondre.DateVondre.AddHours(1) >= moroccoTime)
            {
                if (produit.quantity + ancienquantitieVente >= planVondre.Qnt_vondre)
                {
                    try
                    {
                        produit.quantity = produit.quantity + ancienquantitieVente;
                        produit.quantity = produit.quantity - planVondre.Qnt_vondre;
                        planVondre.Total_Vondre = planVondre.Prix_Vondre * planVondre.Qnt_vondre;

                        _context.Update(planVondre);
                        _context.Produits.Update(produit);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(IndexForVendeur));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!PlanVondreExists(planVondre.id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                ViewData["qntinsifusant"] = "Quantitié Stock Est insuffisante";
                ViewData["Produitid"] = new SelectList(_context.Produits, "id", "id", planVondre.Produitid);
                ViewData["Produitnom"] = produit.Nom.ToString();
                ViewData["ancienquantitieVente"] = ancienquantitieVente.ToString();
                return View(planVondre);
            }

            ViewData["Vouspouverpas"] = "Vous Ne Pouvez Pas Modifier Ce Plan De Vente. Veuillez Contacter Votre Directeur pour le régler. ";
            ViewData["Produitid"] = new SelectList(_context.Produits, "id", "id", planVondre.Produitid);
            ViewData["Produitnom"] = produit.Nom.ToString();
            ViewData["ancienquantitieVente"] = ancienquantitieVente.ToString();
            return View(planVondre);

        }
        // GET: PlanVondre/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            
            if (id == null || _context.PlanVondres == null)
            {
                return NotFound();
            }

            var planVondre = await _context.PlanVondres.FindAsync(id);
            if (planVondre == null)
            {
                return NotFound();
            }
            var produite = _context.Produits.Where(p => p.id == planVondre.Produitid).FirstOrDefault();
            if(produite == null)
            {
                return NotFound();
            }
            ViewData["Produitid"] = new SelectList(_context.Produits, "id", "Nom", planVondre.Produitid);
            ViewData["Produitnom"] = produite.Nom.ToString();
            ViewData["ancienquantitieVente"] = planVondre.Qnt_vondre.ToString();
            return View(planVondre);
        }

        // POST: PlanVondre/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,Produitid,Reference,Qnt_vondre,Prix_Vondre,Total_Vondre,DateVondre")] PlanVondre planVondre)
        {

            string? directeurs = HttpContext.Session.GetString("Directeur");
            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            int ancienquantitieVente = int.Parse(Request.Form["ancienquantitieVente"].ToString());
            var produit =  _context.Produits.Where(p => p.id == planVondre.Produitid).FirstOrDefault();
            if (id != planVondre.id)
            {
                return NotFound();
            }

            if(produit == null)
            {
                return NotFound();
            }
            if (produit.quantity + ancienquantitieVente >= planVondre.Qnt_vondre)
            {
                try
                {
                    produit.quantity = produit.quantity + ancienquantitieVente;
                    produit.quantity = produit.quantity - planVondre.Qnt_vondre;
                    planVondre.Total_Vondre = planVondre.Prix_Vondre * planVondre.Qnt_vondre;

                    _context.Update(planVondre);
                    _context.Produits.Update(produit);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanVondreExists(planVondre.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["Vouspouverpas"] = "Quantitié De Produit Est insuffisante";
            ViewData["Produitid"] = new SelectList(_context.Produits, "id", "id", planVondre.Produitid);
            ViewData["Produitnom"] = produit.Nom.ToString();
            ViewData["ancienquantitieVente"] = ancienquantitieVente.ToString();
            return View(planVondre);
            
            
        }
        /*
        // GET: PlanVondre/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.PlanVondres == null)
            {
                return NotFound();
            }

            var planVondre = await _context.PlanVondres
                .Include(p => p.produit)
                .FirstOrDefaultAsync(m => m.id == id);
            if (planVondre == null)
            {
                return NotFound();
            }

            return View(planVondre);
        }

        // POST: PlanVondre/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (_context.PlanVondres == null)
            {
                return Problem("Entity set 'GstDbMagazin.PlanVondres'  is null.");
            }
            var planVondre = await _context.PlanVondres.FindAsync(id);
            if (planVondre != null)
            {
                _context.PlanVondres.Remove(planVondre);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        private bool PlanVondreExists(int id)
        {
          return (_context.PlanVondres?.Any(e => e.id == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> Chercherproduitnom()
        {
            string? nom = Request.Form["produitnom"].ToString();

            if (nom == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).Where(p =>p.produit.Nom == nom).ToListAsync();
            if (planVondres == null)
            {
                return View();
            }

            ViewData["nomChercher"] = nom ;
            ViewData["totalparnom"] = "' " + nom + " ' est: " + planVondres.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(planVondres);



        }
        [HttpPost]
        public async Task<IActionResult> ChercherproduitReference()
        {
            string? Reference = Request.Form["Reference"].ToString();

            if (Reference == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).Where(p =>p.produit.Reference == Reference).ToListAsync();
            if (planVondres == null)
            {
                return View();
            }

            ViewData["ReferenceChercher"] = Reference ;
            ViewData["totalparReference"] = planVondres.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(planVondres);



        }
        [HttpPost]
        public async Task<IActionResult> ChercherdateVondre()
        {

            var date = DateTime.Parse(Request.Form["datevondre"].ToString()).ToShortDateString();

            if (date == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).ToListAsync();

            if (planVondres == null)
            {
                return View();
            }

            var pl =  planVondres.Where(p=>p.DateVondre.ToShortDateString()==date);
            ViewData["totalavondrepardate"] = "' " + date.ToString() + " ' est : " + pl.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(pl);



        }




        [HttpPost]
        public async Task<IActionResult> Chercherproduitnom1()
        {
            string? nom = Request.Form["produitnom"].ToString();

            if (nom == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).Where(p =>p.produit.Nom == nom).ToListAsync();
            if (planVondres == null)
            {
                return View();
            }

            ViewData["totalparnom"] = "' " + nom + " ' est: " + planVondres.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(planVondres);



        }
        [HttpPost]
        public async Task<IActionResult> ChercherproduitReference1()
        {
            string? Reference = Request.Form["Reference"].ToString();

            if (Reference == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).Where(p =>p.produit.Reference == Reference).ToListAsync();
            if (planVondres == null)
            {
                return View();
            }

            ViewData["ReferenceChercher"] = Reference ;
            ViewData["totalparReference"] = planVondres.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(planVondres);



        }
        [HttpPost]
        public async Task<IActionResult> ChercherdateVondre1()
        {

            var date = DateTime.Parse(Request.Form["datevondre"].ToString()).ToShortDateString();

            if (date == null || _context.PlanVondres == null)
            {
                return View();
            }

            var planVondres = await _context.PlanVondres.Include(p => p.produit).ToListAsync();

            if (planVondres == null)
            {
                return View();
            }

            var pl =  planVondres.Where(p=>p.DateVondre.ToShortDateString()==date);
            ViewData["totalavondrepardate"] = "' " + date.ToString() + " ' est : " + pl.Sum(p => p.Total_Vondre).ToString() + " DH";
            return View(pl);



        }
    }
}
