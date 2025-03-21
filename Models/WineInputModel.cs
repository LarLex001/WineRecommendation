using System.ComponentModel.DataAnnotations;

namespace WineRecommendation.Models
{
    public class WineInputModel
    {
        [Required]
        [Display(Name = "Fixed Acidity (g/L)")]
        [Range(0.01, 30.0, ErrorMessage = "Value must be between 0.01 and 30.0")]
        public float FixedAcidity { get; set; } = 7.0f;

        [Required]
        [Display(Name = "Volatile Acidity (g/L)")]
        [Range(0.01, 5.0, ErrorMessage = "Value must be between 0.01 and 5.0")]
        public float VolatileAcidity { get; set; } = 0.4f;

        [Required]
        [Display(Name = "Citric Acid (g/L)")]
        [Range(0.0, 1.0, ErrorMessage = "Value must be between 0.0 and 1.0")]
        public float CitricAcid { get; set; } = 0.3f;

        [Required]
        [Display(Name = "Residual Sugar (g/L)")]
        [Range(0.01, 100.0, ErrorMessage = "Value must be between 0.01 and 100.0")]
        public float ResidualSugar { get; set; } = 2.0f;

        [Required]
        [Display(Name = "Chlorides (g/L)")]
        [Range(0.001, 1.0, ErrorMessage = "Value must be between 0.001 and 1.0")]
        public float Chlorides { get; set; } = 0.08f;

        [Required]
        [Display(Name = "Free Sulfur Dioxide (mg/L)")]
        [Range(1.0, 300.0, ErrorMessage = "Value must be between 1.0 and 300.0")]
        public float FreeSulfurDioxide { get; set; } = 15.0f;

        [Required]
        [Display(Name = "Total Sulfur Dioxide (mg/L)")]
        [Range(1.0, 500.0, ErrorMessage = "Value must be between 1.0 and 500.0")]
        public float TotalSulfurDioxide { get; set; } = 40.0f;

        [Required]
        [Display(Name = "Density (g/cm³)")]
        [Range(0.9, 1.1, ErrorMessage = "Value must be between 0.9 and 1.1")]
        public float Density { get; set; } = 0.996f;

        [Required]
        [Display(Name = "pH")]
        [Range(2.5, 4.5, ErrorMessage = "Value must be between 2.5 and 4.5")]
        public float PH { get; set; } = 3.3f;

        [Required]
        [Display(Name = "Sulphates (g/L)")]
        [Range(0.1, 2.0, ErrorMessage = "Value must be between 0.1 and 2.0")]
        public float Sulphates { get; set; } = 0.6f;

        [Required]
        [Display(Name = "Alcohol (%)")]
        [Range(8.0, 15.0, ErrorMessage = "Value must be between 8.0 and 15.0")]
        public float Alcohol { get; set; } = 10.5f;
    }
}