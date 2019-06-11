using System;
using System.ComponentModel.DataAnnotations;

namespace XService.Business.Models
{
    public class SampleModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}