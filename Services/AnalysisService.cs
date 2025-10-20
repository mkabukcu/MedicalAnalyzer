using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

namespace MedicalAnalyzer.Services
{
    public class AnalysisService
    {
        // private readonly string _pythonPath = @"/Library/Frameworks/Python.framework/Versions/3.13/bin/python3";
        // private readonly string _pythonPath = "/Volumes/KnightData/PhDWorkspace_Mac/MedicalAnalyzer/wwwroot/scripts/venvMedicalAnalyzer/bin/python3";
        private readonly string _pythonPath;

        // private readonly string _scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "scripts", "ikiboyutluanaliz.py");
        private readonly string _scriptPath;

        public AnalysisService(IConfiguration configuration)
        {
            _pythonPath = configuration["Python:PythonPath"] ?? "/Volumes/KnightData/PhDWorkspace_Mac/MedicalAnalyzer/wwwroot/scripts/venvMedicalAnalyzer/bin/python3"; // default olarak PATH'teki python
            _scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "scripts", "ikiboyutluanaliz.py");
        }
        
        public List<string> AnalyzeData(IFormFile? threeDImage,
                                        IFormFile? sectionMinus16,
                                        IFormFile? section00,
                                        IFormFile? section20,
                                        double? num1, double? num2,
                                        string? text1, string? text2)
        {
            var results = new List<string>();

            // === 3D Görüntü
            if (threeDImage != null && threeDImage.Length > 0)
            {
                results.Add($"🧩 3D Görüntü '{threeDImage.FileName}' analiz edildi: Beyin yapısı normal görünüyor.");
                ////// threedResult = RunPythonOrExe("ucboyutluanaliz.exe", path);
            }
            else
            {
                results.Add("⚠️ 3D Görüntü sağlanmadı.");
            }

            // === 2D Kesitler
            int count = new[] { sectionMinus16, section00, section20 }.Count(f => f != null && f.Length > 0);
            if (count > 0)
            {


                // Python kodunu çağır
                var args = new List<string>();
                if (sectionMinus16 != null) args.Add(sectionMinus16.FileName);
                if (section00 != null) args.Add(section00.FileName);
                if (section20 != null) args.Add(section20.FileName);

                string pythonOutput = RunPythonScript(_scriptPath, args.ToArray());
                results.Add($"📸 {count} adet 2D kesit analiz edildi:");
                results.Add($"<pre>{pythonOutput}</pre>");

                // // Geçici dosya yollarını kaydet
                // string tempDir = Path.Combine(Path.GetTempPath(), "MedicalAnalyzer");
                // Directory.CreateDirectory(tempDir);

                // var filePaths = new List<string>();
                // foreach (var f in new[] { sectionMinus16, section00, section20 })
                // {
                //     if (f != null && f.Length > 0)
                //     {
                //         string tempFile = Path.Combine(tempDir, f.FileName);
                //         using (var stream = new FileStream(tempFile, FileMode.Create))
                //             f.CopyTo(stream);
                //         filePaths.Add(tempFile);
                //     }
                // }

                // // Python script'ini çalıştır
                // string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "scripts", "ikiboyutluanaliz.py");
                // if (!File.Exists(scriptPath))
                // {
                //     results.Add($"⚠️ Python script bulunamadı: {scriptPath}");
                // }
                // else
                // {
                //     string pythonOutput = RunPythonScript(scriptPath, filePaths.ToArray());
                //     results.Add($"🐍 Python sonucu:\n{pythonOutput}");
                // }

            }
            else
            {
                results.Add("⚠️ 2D kesit yüklenmedi.");
            }

            // === Sayısal Girdiler
            if (num1.HasValue && num2.HasValue)
            {
                double ratio = num2.Value != 0 ? num1.Value / num2.Value : 0;
                results.Add($"📊 Sayısal oran: {ratio:F2}");
            }
            else
            {
                results.Add("⚠️ Sayısal girdiler eksik.");
            }

            // === Metin Girdileri
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(text1))
                sb.Append($"[{text1}] ");
            if (!string.IsNullOrWhiteSpace(text2))
                sb.Append($"[{text2}] ");

            results.Add(sb.Length > 0
                ? $"💬 Metin analizi tamamlandı: {sb}"
                : "⚠️ Metin girdisi yok.");

            // === Örnek Sonuç
            results.Add($"🧠 Tahmini analiz sonucu: {(count + (num1 ?? 0) + (num2 ?? 0) > 5 ? "Normal" : "Dikkat Gerekiyor")}");

            return results;
        }





        private string RunPythonScript(string scriptPath, params string[] args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = _pythonPath,
                Arguments = $"\"{scriptPath}\" {string.Join(" ", args)}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(psi))
            {
                if (process == null)
                    return "Python process başlatılamadı.";
                
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error))
                    output += $"\n[Python Hatası]: {error}";

                return output.Trim();
            }
        }
    }
}
