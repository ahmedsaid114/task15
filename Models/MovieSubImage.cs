
using System.ComponentModel.DataAnnotations;

namespace CinemaProject.Models
{
    public class MovieSubImage
    {
        public int Id { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public int MovieId { get; set; }

        public Movie? Movie { get; set; }
    }
}
