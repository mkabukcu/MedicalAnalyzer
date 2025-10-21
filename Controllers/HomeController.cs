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
        public IActionResult Analyze(IFormFile? threeDImage,
                                     IFormFile? sectionMinus16,
                                     IFormFile? section00,
                                     IFormFile? section20,
                                     double? numericInput1, double? numericInput2,
                                     string? textInput1, string? textInput2)
        {
            AnalysisOutputModel outputModel = new AnalysisOutputModel();

            // Upload klasörünü hazırla
            string uploadsFolder = Path.Combine(_env.WebRootPath ?? string.Empty, "uploads");
            Directory.CreateDirectory(uploadsFolder);

            // Dosyaları kaydet (varsa)
            SaveFile(threeDImage, uploadsFolder);
            SaveFile(sectionMinus16, uploadsFolder);
            SaveFile(section00, uploadsFolder);
            SaveFile(section20, uploadsFolder);

            if (threeDImage != null) { outputModel.ThreeDImagePath = "/uploads/" + Path.GetFileName(threeDImage.FileName); }
            if (sectionMinus16 != null) { outputModel.SectionMinus16Path = "/uploads/" + Path.GetFileName(sectionMinus16.FileName); }
            if (section00 != null) { outputModel.Section00Path = "/uploads/" + Path.GetFileName(section00.FileName); }
            if (section20 != null) { outputModel.Section20Path = "/uploads/" + Path.GetFileName(section20.FileName); }

            outputModel.NumericInput1 = numericInput1;
            outputModel.NumericInput2 = numericInput2;
            outputModel.TextInput1 = textInput1;
            outputModel.TextInput2 = textInput2;

            var results = _analysisService.AnalyzeData(
                threeDImage, sectionMinus16, section00, section20, numericInput1, numericInput2, textInput1, textInput2);

            outputModel.Outputs = results;
            
            return View(outputModel); // results bir List<string>
        }

        private void SaveFile(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0) return;

            var safeFileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(folder, safeFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);
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

            return View(model);
        }

        // [HttpGet]
        // public IActionResult Analyze()
        // {
        //     return View(new List<string>()); // Sayfa boş açılır
        // }

    }
}
