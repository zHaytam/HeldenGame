using Assets.Scripts.UI.Validation.Validators;
using NUnit.Framework;

namespace Assets.Scripts.Tests.UI.Vallidation.Validators
{
    public class RequiredUiValidatorTests
    {

        [Test]
        public void Validate_ShouldReturnFalseWhenStringIsEmpty()
        {
            var attr = new RequiredUiValidator();
            bool isValid = attr.Validate("");
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void Validate_ShouldReturnFalseWhenStringIsWhitespaces()
        {
            var attr = new RequiredUiValidator();
            bool isValid = attr.Validate(" ");
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void Validate_ShouldReturnFalseWhenObjIsNull()
        {
            var attr = new RequiredUiValidator();
            bool isValid = attr.Validate(null);
            Assert.That(isValid, Is.False);
        }

        [Test]
        public void Validate_ShouldReturnTrueWhenStringIsValid()
        {
            var attr = new RequiredUiValidator();
            bool isValid = attr.Validate("Something");
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Validate_ShouldReturnTrueWhenAnyOtherType()
        {
            var attr = new RequiredUiValidator();
            bool isValid = attr.Validate(0);
            Assert.That(isValid, Is.True);
        }

    }
}
