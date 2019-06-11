using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

using Castle.DynamicProxy;
using XService.Enterprise.Aspects;

namespace XService.Enterprise.Tests.Aspects
{
    [TestClass]
    public class ModelValidatorAspectTests
    {
        IInterceptor _uut;

        [TestInitialize]
        public void Setup() {
            _uut = new ModelValidatorAspect();
        }

        [TestMethod]
        public void Should_Validate_Model() {

        }

        [TestMethod]
        [ExpectedException(typeof(ValidationException))]
        public void Should_Not_Validate_Model() {

        }
    }

    internal class DemoModel {
        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Name {get;set;}
    }
}