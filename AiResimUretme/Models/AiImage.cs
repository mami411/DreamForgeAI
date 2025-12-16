using System.ComponentModel.DataAnnotations;

namespace AiResimUretme.Models
{
    public class AiImage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Prompt alanı boş bırakılamaz.")]
        [StringLength(500)]
        public string Prompt { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}