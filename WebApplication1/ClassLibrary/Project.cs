using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary
{
    public class Project
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Du måste ange titel på projektet.")]
        [MaxLength(50, ErrorMessage = "Titeln får inte vara längre än 50 tecken.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Du måste ange en beskrivning av projektet.")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
