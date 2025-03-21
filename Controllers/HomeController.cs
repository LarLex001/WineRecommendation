using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WineRecommendation.Models;
using WineRecommendation.Services;

namespace WineRecommendation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWineService _wineService;
        private readonly IWinePredictionService _predictionService;

        public HomeController(IWineService wineService, IWinePredictionService predictionService)
        {
            _wineService = wineService;
            _predictionService = predictionService;
        }

        public async Task<IActionResult> Index()
        {
            var dashboardViewModel = new DashboardViewModel
            {
                TotalWines = await _wineService.GetTotalRecordsCountAsync(),
                TotalPredictions = await _predictionService.GetTotalPredictionsAsync(),
                UntrainedPredictions = await _predictionService.GetUntrainedPredictionsCountAsync()
            };

            return View(dashboardViewModel);
        }

        public IActionResult Privacy() => View();
        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        
    }
}