using Microsoft.AspNetCore.Mvc;
using MedicalAnalyzer.Models;
using MedicalAnalyzer.Services;

namespace MedicalAnalyzer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private AnalysisService _analysisService;

        public HomeController(IWebHostEnvironment env, AnalysisService analysisService)
        {
            _env = env;
            _analysisService = analysisService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public IActionResult Index(MedicalInputModel model)
        {
            // Model binding başarısızsa bilgilendir
            if (model == null)
            {
                ViewBag.Outputs = new List<string> { "❌ Model null geldi — model binding başarısız." };
                return View("Index");
            }

            // Upload klasörünü hazırla
            string uploadsFolder = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            // Dosyaları kaydet (varsa)
            SaveFile(model.ThreeDImage, uploadsFolder);
            SaveFile(model.SectionMinus16, uploadsFolder);
            SaveFile(model.Section00, uploadsFolder);
            SaveFile(model.Section20, uploadsFolder);

            // Sahte analiz servisini çağır
            var results = _analysisService.AnalyzeData(
                model.ThreeDImage,
                model.SectionMinus16,
                model.Section00,
                model.Section20,
                model.NumericInput1,
                model.NumericInput2,
                model.TextInput1,
                model.TextInput2
            );

            model.Outputs = results;

            ViewBag.Outputs = results;

            // Section00 için gösterim linki (varsa)
            if (model.Section00 != null && model.Section00.Length > 0)
            {
                ViewBag.Section00Image = "/uploads/" + Path.GetFileName(model.Section00.FileName);
            }
            else
            {
                ViewBag.Section00Image = null;
            }

            return View();
        }

        private void SaveFile(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0) return;

            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(folder, safeFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
        }

        [HttpGet]
        public IActionResult Analyze()
        {
            return View(new List<string>()); // Sayfa boş açılır
        }

        [HttpPost]
        public IActionResult Analyze(IFormFile? threeDImage,
                                     IFormFile? sectionMinus16,
                                     IFormFile? section00,
                                     IFormFile? section20,
                                     double? num1, double? num2,
                                     string? text1, string? text2)
        {
            var results = _analysisService.AnalyzeData(
                threeDImage, sectionMinus16, section00, section20, num1, num2, text1, text2);

            return View(results); // results bir List<string>
        }




    }
}
