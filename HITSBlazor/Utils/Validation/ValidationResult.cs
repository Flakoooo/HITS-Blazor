namespace HITSBlazor.Utils.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }

        private ValidationResult(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }

        public static ValidationResult Ok() => new(true);
        public static ValidationResult Fail(string message) => new(false, message);
    }
}
