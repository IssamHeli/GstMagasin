using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GstMagazin.Data;
using TestHostAppAndBaseDonnes.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace GstMagazin.Controllers
{
    public class DirecteurController : Controller
    {
        private readonly GstDbMagazin _context;



        public DirecteurController(GstDbMagazin context )
        {
            _context = context;
        }


        public IActionResult loginDirecteur()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs != null)
            {
                return RedirectToAction(actionName: "Index", controllerName: "produit");
            }
            return View();
        }

        public async Task<string> validaterecapatcha(string token)
        {

            var secretKey = "your secretKey";
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
        public async Task<IActionResult> loginDirecteur([Bind("id,UserName,Password")] Directeur directeur)
        {
            var token = Request.Form["recaptcha_token1"].ToString();

            if (token == " ")
            {

                ViewData["InfoIncorrect"] = "Ops , Actualiser la Page ";

                return View(directeur);

            }

            var respons = await validaterecapatcha(token);

            if (respons == "error")
            {
                ViewBag.ErrorMessage = "Échec de la vérification . Veuillez réessayer plus tard ";

                return View(directeur);

            }

            if (respons == "scorelow")
            {
                ViewBag.ErrorMessage = "Eror 404 : Vous Pouvez Pas Acceder a GstMagasin ... ";


                return View(directeur);
            }
            if (respons == "success")
            {
                try
                {
                     if (ModelState.IsValid)
                     {

                        var directeurr = await _context.Directeurs.FirstOrDefaultAsync(d => d.UserName == directeur.UserName && d.Password == directeur.Password);
                        if (directeurr != null)
                        {
                            HttpContext.Session.SetString("Directeur", directeur.it.ToString());
                            return RedirectToAction(actionName: "Index", controllerName: "produit");
                        }

                        ViewData["InfoIncorrect"] = "Nom d'utilisateur ou mot de passe incorrect";
                        return View(directeur);

                     }

                    ViewBag.ErrorMessage = "Une erreur s'est produite lors de traitement";
                    return View(directeur);


                }
                catch (SqlException)
                    {

                    ViewBag.ErrorMessage = " c'est parce que le plan gratuit est utilisé. \n Essayez jusqu'à 4 fois Pour Connecter ";
                    return View(directeur);
                }


                
            }

            ViewBag.ErrorMessage = "Eror 404 : Vous Pouvez Pas Acceder a GstMagasin ... ";
            return View(directeur);


        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Remove("Directeur");
            return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
        }

        // GET: Directeur
        public async Task<IActionResult> Index()
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            return _context.Directeurs != null ?
                      View(await _context.Directeurs.ToListAsync()) :
                      Problem("la table des directeurs est vide ");

        }
        /*
        // GET: Directeur/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Directeurs == null)
            {
                return NotFound();
            }

            var directeur = await _context.Directeurs
                .FirstOrDefaultAsync(m => m.it == id);
            if (directeur == null)
            {
                return NotFound();
            }

            return View(directeur);
        }
        */
        /*
        // GET: Directeur/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Directeur/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("it,UserName,Password")] Directeur directeur)
        {

            var Directeurs = await _context.Directeurs.ToListAsync();
            if (Directeurs == null)
            {
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Add(directeur);
                        await _context.SaveChangesAsync();
                        HttpContext.Session.SetString("Directeur", directeur.it.ToString());
                        return RedirectToAction(actionName: "Index", controllerName: "Produit");

                    }

                    catch
                    {
                        return NotFound();
                    }
                }

            }

            return View(directeur);
        }
        
        */
        // GET: Directeur/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Directeurs == null)
            {
                return NotFound();
            }

            var directeur = await _context.Directeurs.FindAsync(id);
            if (directeur == null)
            {
                return NotFound();
            }
            return View(directeur);
        }
        // POST: Directeur/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("it,UserName,Password")] Directeur directeur)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            string ancienpassword = Request.Form["ancienpassword"].ToString();
            string Nouvauxpassword = Request.Form["Nouvauxpassword"].ToString();
            if (id != directeur.it)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(ancienpassword == directeur.Password)
                {
                    try
                    {
                        directeur.Password = Nouvauxpassword;
                        _context.Update(directeur);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DirecteurExists(directeur.it))
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
                    return View(directeur);
                }
                
                return RedirectToAction(nameof(Index));
            }
            return View(directeur);
        }
        /*
        // GET: Directeur/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (id == null || _context.Directeurs == null)
            {
                return NotFound();
            }

            var directeur = await _context.Directeurs
                .FirstOrDefaultAsync(m => m.it == id);
            if (directeur == null)
            {
                return NotFound();
            }

            return View(directeur);
        }

        // POST: Directeur/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string? directeurs = HttpContext.Session.GetString("Directeur");

            if (directeurs == null)
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            if (_context.Directeurs == null)
            {
                return Problem("Entity set 'GstDbMagazin.Directeurs'  is null.");
            }
            var directeur = await _context.Directeurs.FindAsync(id);
            if (directeur != null)
            {
                _context.Directeurs.Remove(directeur);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */
        private bool DirecteurExists(int id)
        {
          return (_context.Directeurs?.Any(e => e.it == id)).GetValueOrDefault();
        }
    }
}
