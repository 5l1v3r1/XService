using System;
using System.ComponentModel.DataAnnotations;

namespace XService.Business.Models {
    public class SampleModel {
        [Required]
        [StringLength(25, MinimumLength = 4)]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}