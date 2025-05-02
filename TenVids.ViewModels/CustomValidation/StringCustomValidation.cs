using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TenVids.ViewModels.CustomValidation
{
    public class StringCustomValidation : ValidationAttribute
    {
        private readonly string _name;
        private readonly bool _required;
        private readonly int _minLength;
        private readonly int _maxLength;
        private readonly string _regex;
        private readonly string _regexErrorMessage;

        public StringCustomValidation(string name, bool required, int minLength, int maxLength, string regex, string regexErrorMessage)
        {
            _name = name;
            _required = required;
            _minLength = minLength;
            _maxLength = maxLength;
            _regex = regex;
            _regexErrorMessage = regexErrorMessage;
        }

        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value as string;

            if (_required && string.IsNullOrWhiteSpace(strValue))
            {
                return new ValidationResult($"{_name} is required");
            }

            if (_minLength > 0 && !string.IsNullOrEmpty(strValue))
            {
                if (strValue.Length < _minLength)
                {
                    return new ValidationResult($"{_name} must be at least {_minLength} characters long");
                }
            }

            if (_maxLength > 0 && !string.IsNullOrEmpty(strValue))
            {
                if (strValue.Length > _maxLength)
                {
                    return new ValidationResult($"{_name} must be less than or equal to {_maxLength} characters");
                }
            }

            if (!string.IsNullOrEmpty(_regex) && !string.IsNullOrEmpty(strValue))
            {
                var regex = new Regex(_regex);
                if (!regex.IsMatch(strValue))
                {
                    return new ValidationResult(!string.IsNullOrEmpty(_regexErrorMessage)
                        ? _regexErrorMessage
                        : $"{_name} does not match the required pattern: {_regex}");
                }
            }

            return ValidationResult.Success;
        }
    }
}
