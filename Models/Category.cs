using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
