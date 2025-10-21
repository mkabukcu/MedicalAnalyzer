using Microsoft.AspNetCore.Http;

namespace MedicalAnalyzer.Models
{
    public class AnalysisOutputModel
    {
        public string? ThreeDImagePath { get; set; }
        public string? SectionMinus16Path { get; set; }
        public string? Section00Path { get; set; }
        public string? Section20Path { get; set; }

        public double? NumericInput1 { get; set; }
        public double? NumericInput2 { get; set; }

        public string? TextInput1 { get; set; }
        public string? TextInput2 { get; set; }



        public List<string>? Outputs { get; set; }
    } 
}
