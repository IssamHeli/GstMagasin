using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestHostAppAndBaseDonnes.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GstMagazin.Controllers
{
    public class MagasinController : Controller
    {
        private readonly GstDbMagazin _context;

        public MagasinController(GstDbMagazin context)
        {
            _context = context;
        }
        // GET: /<controller>/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            calculatedData cd = new calculatedData();
            var PlanVentes =  await _context.PlanVondres.ToListAsync();
            var PlanAchats =  await _context.PlanAchats.ToListAsync();

            if(PlanAchats == null)
            {
                return View();
            }
            if (PlanAchats == null)
            {
                return View();
            }

            cd.TotalSales = PlanVentes.Sum(a => a.Total_Vondre);
            cd.TotalPurchases = PlanAchats.Sum(a => a.Total_Achat);

            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }

        [HttpGet]
        public async Task<IActionResult> LastWeek()
        {

            // Retrieve Directeur's ID from local storage
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId != null)
            {
                var purchases = await  _context.PlanAchats.ToListAsync();
                var sales = await  _context.PlanVondres.ToListAsync();
                if (purchases == null)
                {
                    return NotFound();
                }
                if (sales == null)
                {
                    return NotFound();
                }

                var sevenDaysAgo = DateTime.Today.AddDays(-7);
                purchases = purchases.Where(p => p.DateAchat >= sevenDaysAgo).ToList();
                sales = sales.Where(s => s.DateVondre >= sevenDaysAgo).ToList();

                // Group purchases by date and calculate total purchase for each date
                var purchasesGroupedByDate = purchases
                    .GroupBy(p => p.DateAchat.Date)
                    .Select(group => new { Date = group.Key, TotalAchat = group.Sum(p => p.Total_Achat) })
                    .ToList();

                // Group sales by date and calculate total sales for each date
                var salesGroupedByDate = sales
                    .GroupBy(s => s.DateVondre.Date)
                    .Select(group => new { Date = group.Key, TotalVente = group.Sum(s => s.Total_Vondre) })
                    .ToList();

                // Create a list of all distinct dates from both purchases and sales
                var allDates = purchasesGroupedByDate.Select(p => p.Date)
                    .Union(salesGroupedByDate.Select(s => s.Date))
                    .Distinct()
                    .OrderBy(date => date)
                    .ToList();

                var chartLabels = allDates.Select(date => date.ToString("dd/MM/yyyy")).ToList();

                var chartData = allDates
                    .Select(date => new DataPoint
                    {
                        Date = date,
                        Difference = (salesGroupedByDate.FirstOrDefault(s => s.Date == date)?.TotalVente ?? 0) -
                                     (purchasesGroupedByDate.FirstOrDefault(p => p.Date == date)?.TotalAchat ?? 0)
                    })
                    .ToList();
                
                var model = new ChartViewModel
                {
                    Labels = chartLabels,
                    Data = chartData
                };
                
                var jsonChartData = JsonConvert.SerializeObject(model.Data.Select(d => d.Difference));

                ViewBag.ChartData = jsonChartData;

                return View(model);
            }
            else
            {
                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }



        }


        [HttpGet]
        public async Task<IActionResult> ThisDay()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            DateTime date = DateTime.Now;

            calculatedData cd = new calculatedData();
            cd.date = date;
            var PlanVentes = await  _context.PlanVondres.ToListAsync();
            var PlanAchats = await  _context.PlanAchats.ToListAsync();
            if(PlanVentes == null)
            {
                return NotFound();
            }
            if (PlanAchats == null)
            {
                return NotFound();
            }
            foreach (var item in PlanVentes)
            {
                if (item.DateVondre.Date.ToShortDateString().Equals(date.ToShortDateString()))
                {
                    cd.TotalSales = cd.TotalSales + item.Total_Vondre;
                }

            }

            foreach (var item in PlanAchats)
            {
                if (item.DateAchat.Date.ToShortDateString().Equals(date.ToShortDateString()))
                {
                    cd.TotalPurchases = cd.TotalPurchases + item.Total_Achat;
                }

            }
            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                date = cd.date

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }


        [HttpGet]
        public async Task<IActionResult> thismonth()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            calculatedData cd = new calculatedData();
            cd.month = DateTime.Now.Month;
            cd.year = DateTime.Now.Year;
            var PlanVentes = await _context.PlanVondres.ToListAsync();
            var PlanAchats = await _context.PlanAchats.ToListAsync();


            if(PlanAchats == null)
            {
                return View();
            }
            if (PlanVentes == null)
            {

                return View();
            }
            cd.TotalSales = PlanVentes.Where(a =>a.DateVondre.Year == DateTime.Now.Year && a.DateVondre.Month == DateTime.Now.Month).Sum(a => a.Total_Vondre);
            cd.TotalPurchases = PlanAchats.Where(a => a.DateAchat.Year == DateTime.Now.Year && a.DateAchat.Month == DateTime.Now.Month).Sum(a => a.Total_Achat);

            
            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                year = cd.year,
                month = cd.month

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }

        [HttpGet]
        public async Task<IActionResult> thisyear()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }

            calculatedData cd = new calculatedData();
            cd.year = DateTime.Now.Year;

            var PlanVentes = await _context.PlanVondres.ToListAsync();
            var PlanAchats = await _context.PlanAchats.ToListAsync();


            if (PlanAchats == null)
            {
                return View();
            }
            if (PlanVentes == null)
            {

                return View();
            }
            cd.TotalSales = PlanVentes.Where(a => a.DateVondre.Year == DateTime.Now.Year ).Sum(a => a.Total_Vondre);
            cd.TotalPurchases = PlanAchats.Where(a => a.DateAchat.Year == DateTime.Now.Year ).Sum(a => a.Total_Achat);


            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                year = cd.year,

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }



        


        [HttpPost]
        public async Task<IActionResult> filterParJour()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            DateTime date = DateTime.Parse(Request.Form["Joure"].ToString());

            calculatedData cd = new calculatedData();
            cd.date = date;
            var PlanVentes = await _context.PlanVondres.ToListAsync();
            var PlanAchats = await _context.PlanAchats.ToListAsync();

            foreach (var item in PlanVentes)
            {
                if (item.DateVondre.Date.ToShortDateString().Equals(date.ToShortDateString()))
                {
                    cd.TotalSales = cd.TotalSales + item.Total_Vondre;
                }

            }

            foreach (var item in PlanAchats)
            {
                if (item.DateAchat.Date.ToShortDateString().Equals(date.ToShortDateString()))
                {
                    cd.TotalPurchases = cd.TotalPurchases + item.Total_Achat;
                }

            }
            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                date = cd.date

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }

        [HttpPost]
        public async Task<IActionResult> filterParAnne()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            int annee = int.Parse(Request.Form["filterAnne"].ToString());

            calculatedData cd = new calculatedData();
            cd.year = annee;
            var PlanVentes = await _context.PlanVondres.ToListAsync();
            var PlanAchats = await _context.PlanAchats.ToListAsync();

            foreach (var item in PlanVentes)
            {
                if (item.DateVondre.Year.Equals(annee))
                {
                    cd.TotalSales = cd.TotalSales + item.Total_Vondre;
                }

            }

            foreach (var item in PlanAchats)
            {
                if (item.DateAchat.Year == annee)
                {
                    cd.TotalPurchases = cd.TotalPurchases + item.Total_Achat;
                }

            }
            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                year = cd.year

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }

        [HttpPost]
        public async Task<IActionResult> filterParMonth()
        {
            string? directeurId = HttpContext.Session.GetString("Directeur");

            if (directeurId == null)
            {

                return RedirectToAction(actionName: "loginDirecteur", controllerName: "Directeur");
            }
            int annee = int.Parse(Request.Form["anne"].ToString());
            int mois = int.Parse(Request.Form["mois"].ToString());

            calculatedData cd = new calculatedData();
            cd.year = annee;
            cd.month = mois;
            var PlanVentes = await _context.PlanVondres.ToListAsync();
            var PlanAchats = await _context.PlanAchats.ToListAsync();

            foreach (var item in PlanVentes)
            {
                if (item.DateVondre.Year.Equals(annee) && item.DateVondre.Month.Equals(mois))
                {
                    cd.TotalSales = cd.TotalSales + item.Total_Vondre;
                }

            }

            foreach (var item in PlanAchats)
            {
                if (item.DateAchat.Year == annee && item.DateAchat.Month.Equals(mois))
                {
                    cd.TotalPurchases = cd.TotalPurchases + item.Total_Achat;
                }

            }
            calculatedData CalculatedData = new calculatedData
            {
                TotalSales = cd.TotalSales,
                TotalPurchases = cd.TotalPurchases,
                Profit = cd.TotalSales - cd.TotalPurchases,
                year = cd.year,
                month = cd.month

            };


            var jsonChartData = JsonConvert.SerializeObject(CalculatedData);

            ViewBag.ChartData = jsonChartData;

            return View(CalculatedData);
        }

    }
}

