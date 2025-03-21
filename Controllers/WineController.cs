using Microsoft.AspNetCore.Mvc;
using WineRecommendation.Models;
using WineRecommendation.Services;

namespace WineRecommendation.Controllers
{
    public class WineController : Controller
    {
        private readonly IWineService _wineService;

        public WineController(IWineService wineService)
        {
            _wineService = wineService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var totalRecords = await _wineService.GetTotalRecordsCountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var wines = await _wineService.GetWinePageAsync(page, pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(wines);
        }

        public async Task<IActionResult> Details(int id)
        {
            var wine = await _wineService.GetWineByIdAsync(id);
            if (wine == null) return NotFound();
            return View(wine);
        }

        public IActionResult Create() => View();
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WineData wine)
        {
            if (ModelState.IsValid)
            {
                wine.CreatedDate = DateTime.Now;
                wine.IsTrainingData = true;
                await _wineService.AddWineAsync(wine);
                return RedirectToAction(nameof(Index));
            }
            return View(wine);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var wine = await _wineService.GetWineByIdAsync(id);
            if (wine == null) return NotFound();
            return View(wine);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WineData wine)
        {
            if (id != wine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _wineService.UpdateWineAsync(wine);
                }
                catch (Exception)
                {
                    if (await _wineService.GetWineByIdAsync(id) == null)
                        return NotFound();
                    else
                        throw; 
                }
                return RedirectToAction(nameof(Index));
            }
            return View(wine);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var wine = await _wineService.GetWineByIdAsync(id);
            if (wine == null) return NotFound();    
            return View(wine);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _wineService.DeleteWineAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}