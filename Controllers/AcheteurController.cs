using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GstMagazin.Data;
using TestHostAppAndBaseDonnes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace GstMagazin.Controllers
{
    public class AcheteurController : Controller
    {
        private readonly GstDbMagazin _context;

        public AcheteurController(GstDbMagazin context)
        {
            _context = context;
        }

        public IActionResult loginAcheteur()
        {
            string? acheteurs = HttpContext.Session.GetString("Acheteur");

            if (acheteurs != null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "ProduitAcheteur");
            }
            return View();
        }



        public async Task<string> validaterecapatcha(string token)
        {

            var secretKey = "Your secretKey";
            var client = new HttpClient();
            var result = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", secretKey),
            new KeyValuePair<string, string>("response", token)
            }));

            var jsonResponse = await result.Content.ReadAsStringAsync();
            var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(jsonResponse);

            if (!reCaptchaResponse.Success)
            {
                return "error";
            }
            if (reCaptchaResponse.Score < 0.8)
            {
                return "scorelow ";
            }
            return "success";
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> loginAcheteur([Bind("id,UserName,Password")] Acheteur acheteur)
        {

            var token = Request.Form["recaptcha_token2"].ToString();

            if (token == null)
            {

                ViewData["InfoIncorrect"] = "Ops , Actualiser la Page ";

                return View(acheteur);

            }

            var respons = await validaterecapatcha(token);

            if (respons == "error")
            {
                ViewBag.ErrorMessage = "Échec de la vérification . Veuillez réessayer plus tard ";

                return View(acheteur);

            }

            if (respons == "scorelow")
            {
                ViewBag.ErrorMessage = "Eror 404 : Vous Pouvez Pas Acceder a GstMagasin ... ";

                return View(acheteur);
            }
            if (respons == "success")
            {
                try
                {

                     if (ModelState.IsValid)
                    {

                        var Acheteur = await _context.Acheteurs.FirstOrDefaultAsync(d => d.UserName == acheteur.UserName && d.Password == acheteur.Password);
                        if (Acheteur != null)
                        {
                            HttpContext.Session.SetString("acheteur", acheteur.id.ToString());
                            return RedirectToAction(actionName: "index", controllerName: "ProduitAcheteur");

                        }

                        ViewData["InfoIncorrect"] = "Nom d'utilisateur ou mot de passe incorrect";
                        return View(acheteur);
                    }
                    ViewBag.ErrorMessage = "Une erreur s'est produite lors de traitement";
                    return View(acheteur);

                }
                catch (SqlException)
                {
                    ViewData["InfoIncorrect"] = " c'est parce que le plan gratuit est utilisé. \n Essayez jusqu'à 4 fois pour Connecter";

                    return View(acheteur);
                }
               
            }

            ViewBag.ErrorMessage = "Eror 404 : Vous Pouvez Pas Acceder a GstMagasin ... ";
            return View(acheteur);      
            
        }
        // GET: Acheteur


        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("acheteur");
            return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            return _context.Acheteurs != null ? 
                          View(await _context.Acheteurs.OrderByDescending(p => p.id).ToListAsync()) :
                          Problem("Entity set 'GstDbMagazin.Acheteurs'  is null.");
        }
        [HttpGet]
        public async Task<IActionResult> Indexacheteur()
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {
                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            return _context.Acheteurs != null ?
                          View(await _context.Acheteurs.OrderByDescending(p => p.id).ToListAsync()) :
                          Problem("Entity set 'GstDbMagazin.Acheteurs'  is null.");
        }
        // GET: Acheteur/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Acheteurs == null)
            {
                return NotFound();
            }

            var acheteur = await _context.Acheteurs
                .FirstOrDefaultAsync(m => m.id == id);
            if (acheteur == null)
            {
                return NotFound();
            }

            return View(acheteur);
        }

        // GET: Acheteur/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
           
            var countacheteur = await _context.Acheteurs.CountAsync();
            if(countacheteur >= 1)
            {
                ViewData["countacheteur"] = countacheteur.ToString();
            }
            return View();
        }

        // POST: Acheteur/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,UserName,Password")] Acheteur acheteur)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            if (ModelState.IsValid)
            {
                _context.Add(acheteur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(acheteur);
        }

        // GET: Acheteur/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null )
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Acheteurs == null)
            {
                return NotFound();
            }

            var acheteur = await _context.Acheteurs.FindAsync(id);
            if (acheteur == null)
            {
                return NotFound();
            }
            return View(acheteur);
        }

        // POST: Acheteur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,UserName,Password")] Acheteur acheteur)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null )
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            string ancienpassword = Request.Form["ancienpassword"].ToString();
            string Nouvauxpassword = Request.Form["Nouvauxpassword"].ToString();

            if (id != acheteur.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (ancienpassword == acheteur.Password)
                {
                    try
                    {
                        acheteur.Password = Nouvauxpassword;
                        _context.Update(acheteur);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AcheteurExists(acheteur.id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    ViewData["ancienincorect"] = "Ancien mot de passe est incorrect";
                    return View(acheteur);
                }
                return RedirectToAction(actionName:"Details",controllerName:"Acheteur",new {id = acheteur.id});
            }
            return View(acheteur);
        }

        [HttpGet]
        public async Task<IActionResult> Editforacheteur(int? id)
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {
                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            if (id == null || _context.Acheteurs == null)
            {
                return NotFound();
            }

            var acheteur = await _context.Acheteurs.FindAsync(id);
            if (acheteur == null)
            {
                return NotFound();
            }
            return View(acheteur);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editforacheteur(int id, [Bind("id,UserName,Password")] Acheteur acheteur)
        {
            string? acheteurs = HttpContext.Session.GetString("acheteur");

            if (acheteurs == null)
            {
                return RedirectToAction(actionName: "loginAcheteur", controllerName: "Acheteur");
            }
            string ancienpassword = Request.Form["ancienpassword"].ToString();
            string Nouvauxpassword = Request.Form["Nouvauxpassword"].ToString();

            if (id != acheteur.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (ancienpassword == acheteur.Password)
                {
                    try
                    {
                        acheteur.Password = Nouvauxpassword;
                        _context.Update(acheteur);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!AcheteurExists(acheteur.id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    ViewData["ancienincorect"] = "Ancien mot de passe est incorrect";
                    return View(acheteur);
                }
                return RedirectToAction(nameof(Indexacheteur));
            }
            return View(acheteur);
        }
        // GET: Acheteur/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Acheteurs == null)
            {
                return NotFound();
            }

            var acheteur = await _context.Acheteurs
                .FirstOrDefaultAsync(m => m.id == id);
            if (acheteur == null)
            {
                return NotFound();
            }

            return View(acheteur);
        }

        // POST: Acheteur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (_context.Acheteurs == null)
            {
                return Problem("Entity set 'GstDbMagazin.Acheteurs'  is null.");
            }
            var acheteur = await _context.Acheteurs.FindAsync(id);
            if (acheteur != null)
            {
                _context.Acheteurs.Remove(acheteur);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcheteurExists(int id)
        {
          return (_context.Acheteurs?.Any(e => e.id == id)).GetValueOrDefault();
        }
    }
}
