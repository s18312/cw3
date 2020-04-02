using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cw3.DTOs.Requests
{
    public class EnrollRequest
    {

        [Required]
        public string IndexNumber { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Student musi miec Imie")]
        [MaxLength(10)]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Student musi miec Nazwisko")]
        [MaxLength(255)]
        public string LastName { get; set; }
        [Required]
        public DateTime Birthdate { get; set; }
        [Required]
        public string Studies { get; set; }
    }
}
