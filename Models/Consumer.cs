using System.ComponentModel.DataAnnotations;

namespace StudentSync.Models
{
    public class Consumer 
    {

        [Key]
        [Display(Name = "ConsumerId")]
        [Required(ErrorMessage = "Consumer ID is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "The Consumer ID may " +
            "only be 10 digits")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only digits are allowed")]
        public string? ConsumerId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The Name may not be" +
            " shorter 2 characters or more than 50 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only characters are allowed")]
        public string? Name { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Surname is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The Surname may not be" +
            " shorter 2 characters or more than 50 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only characters are allowed")]
        public string? Surname { get; set; }

        [Display(Name = "Enrollment Date")]
        [Required(ErrorMessage = "Enrollment Date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }


        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The Email may not be" +
            " shorter 2 characters or more than 50 characters")]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Only characters are allowed")]
        public string? Email { get; set; }

        [Display(Name = "Profile Image")]
        public string? Photo { get; set; }


    }
}


