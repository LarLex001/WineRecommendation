using Microsoft.AspNetCore.Mvc;
using WineRecommendation.Models;
using WineRecommendation.Services;

namespace WineRecommendation.Controllers
{
    public class PredictionController : Controller
    {
        private readonly IWinePredictionService _predictionService;

        public PredictionController(IWinePredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        public IActionResult Index() => View(new WineInputModel());
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Predict(WineInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _predictionService.PredictWineAsync(inputModel);
                return View("Result", result);
            }
            return View("Index", inputModel);
        }

        public async Task<IActionResult> Result(int id)
        {
            var prediction = await _predictionService.GetPredictionByIdAsync(id);
            if (prediction == null) return NotFound();
            return View(prediction);
        }

        public async Task<IActionResult> History()
        {
            var predictions = await _predictionService.GetAllPredictionsAsync();
            return View(predictions);
        }
    }
}