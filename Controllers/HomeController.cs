using Mailo.IRepo;
using Mailo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Mailo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
private readonly IUnitOfWork _unitOfWork;
public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
{
    _logger = logger;
    _unitOfWork = unitOfWork;

}

public async Task<IActionResult> Index()
{
            return View(await _unitOfWork.products.GetAll());
}

      

     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
