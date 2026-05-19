namespace Models.Classes
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }

        public string ErrorMessage { get; set; }

        public ValidationResult()
        {
            IsValid = false;
            ErrorMessage = string.Empty;
        }
        public ValidationResult(bool isValid, string errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
