using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Models
{
    public class AnswerModel
    {
        [HiddenInput]
        public int QuestionId { get; set; }
                
        [Display(Name = "Customer Question")]
        [DataType(DataType.MultilineText)]
        public string QuestionText { get; set; }


        [Required]
        [MaxLength(256)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Answer")]
        public string AnswerText { get; set; }
    }
}
