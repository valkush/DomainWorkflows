using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models
{
    public class NewQuestionModel
    {
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [MaxLength(256)]
        [DataType(DataType.MultilineText)]
        [Display(Name="Your Question")]
        public string Text { get; set; }
    }
}
