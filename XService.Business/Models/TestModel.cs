using System;
using System.ComponentModel.DataAnnotations;

namespace XService.Business.Models
{
    public class TestModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime Created { get; set; }
    }
}