using System;
using System.ComponentModel.DataAnnotations;

namespace GstMagazin.Models
{
	public class UserContact
	{
        [Key]
        public int id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
    }
}

