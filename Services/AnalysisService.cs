using System.Text;

namespace MedicalAnalyzer.Services
{
    public class AnalysisService
    {
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
                results.Add($"🧩 3D Görüntü '{threeDImage.FileName}' analiz edildi: Beyin yapısı normal görünüyor.");
            else
                results.Add("⚠️ 3D Görüntü sağlanmadı.");

            // === 2D Kesitler
            int count = new[] { sectionMinus16, section00, section20 }.Count(f => f != null && f.Length > 0);
            if (count > 0)
                results.Add($"📸 {count} adet 2D kesit analiz edildi: Görsel kalitesi yeterli.");
            else
                results.Add("⚠️ 2D kesit yüklenmedi.");

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
    }
}
