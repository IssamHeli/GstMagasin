using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GstMagazin.Models;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Microsoft.SqlServer.Server;
using Microsoft.EntityFrameworkCore;

namespace GstMagazin.Controllers;

public class HomeController : Controller
{

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger )
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        ViewData["statuscodecontact"] = " ";
        return View();
    }



    public IActionResult LoginSelect()
    {
        return View();
    }

    public IActionResult presenteapp()
    {
        return View();

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

