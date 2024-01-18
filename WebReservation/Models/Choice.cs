using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using WebReservation.Data;

namespace WebReservation.Models
{
    public class Choice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [BindProperty, Display(Name = "Date"), DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateChoice { get; set; }

        public string DateChoiceString => this.DateChoice.ToString();
        public int MaxCapacity { get; set; }
    }
}
