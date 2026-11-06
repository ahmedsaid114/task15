
using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Actor name is required")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Biography { get; set; }

        public string? ProfileImage { get; set; }

        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
