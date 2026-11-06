
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaProject.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Movie name is required")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool Status { get; set; } = true;

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public string? MainImg { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required(ErrorMessage = "Please select a cinema")]
        public int CinemaId { get; set; }

        public Cinema? Cinema { get; set; }

        public ICollection<Actor> Actors { get; set; } = new List<Actor>();

        public ICollection<MovieSubImage> SubImages { get; set; } =
            new List<MovieSubImage>();
    }
}
