
using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class Cinema
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Cinema name is required")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
