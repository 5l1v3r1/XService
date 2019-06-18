using System;
using System.ComponentModel.DataAnnotations;

namespace XService.Business.Models {
    public class SampleModel {
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 10)]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}
