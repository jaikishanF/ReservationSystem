using System.ComponentModel.DataAnnotations;

namespace WebReservation.Models
{
    public class CreateRole
    {
        [Required]
        public string RoleName { get; set; }
    }
}
