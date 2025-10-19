using Microsoft.AspNetCore.Http;

namespace MedicalAnalyzer.Models
{
    public class MedicalInputModel
    {
        public IFormFile? ThreeDImage { get; set; }
        public IFormFile? SectionMinus16 { get; set; }
        public IFormFile? Section00 { get; set; }
        public IFormFile? Section20 { get; set; }

        public double? NumericInput1 { get; set; }
        public double? NumericInput2 { get; set; }

        public string? TextInput1 { get; set; }
        public string? TextInput2 { get; set; }
    }
}
