using System.ComponentModel.DataAnnotations;

namespace WebReservation.Models
{
    public class Building
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Building"), Required]
        public string Name { get; set; }
        [Required]
        public string Room { get; set; }
        public string FullName { get; set; }
        [Display(Name = "Capacity"), Required]
        public int Capacity { get; set; }
    }
}
