using System.Globalization;

namespace HITSBlazor.Utils.Validation
{
    public static class DateValidation
    {
        private const string EmptyStartDateError = "Дата начала не может быть пустой";
        private const string BadFormatStartDateError = "Дата начала имеет неверный формат";
        private const string OldStartDateError = "Дата начала не может быть раньше текущей";
        private const string EmptyFinishDateError = "Дата окончания не может быть пустой";
        private const string BadFormatFinishDateError = "Дата окончания имеет неверный формат";
        private const string OldFinishDateError = "Дата окончания не может быть раньше текущей";
        private const string FinishDateOlderStartDateError = "Дата окончания не может быть раньше даты начала";

        private static class DateHelper<T> where T : struct
        {
            public static readonly bool IsDateOnly = typeof(T) == typeof(DateOnly);
            public static readonly bool IsDateTime = typeof(T) == typeof(DateTime);
            public static readonly bool IsSupported = IsDateOnly || IsDateTime;

            public static int CompareToNow(T date)
            {
                if (IsDateOnly)
                {
                    var dateOnly = (DateOnly)(object)date;
                    var nowDateOnly = DateOnly.FromDateTime(DateTime.Now);
                    return dateOnly.CompareTo(nowDateOnly);
                }
                else
                {
                    var dateTime = (DateTime)(object)date;
                    return dateTime.CompareTo(DateTime.Now);
                }
            }

            public static int Compare(T date1, T date2)
            {
                if (IsDateOnly)
                {
                    var d1 = (DateOnly)(object)date1;
                    var d2 = (DateOnly)(object)date2;
                    return d1.CompareTo(d2);
                }
                else
                {
                    var d1 = (DateTime)(object)date1;
                    var d2 = (DateTime)(object)date2;
                    return d1.CompareTo(d2);
                }
            }
        }

        public static ValidationResult StartDateValidation<T>(T verifiableStartDate) where T : struct
        {
            if (!DateHelper<T>.IsSupported)
                return ValidationResult.Fail(BadFormatStartDateError);

            return DateHelper<T>.CompareToNow(verifiableStartDate) < 0
                ? ValidationResult.Fail(OldStartDateError)
                : ValidationResult.Ok();
        }

        public static ValidationResult FinishDateValidation<T>(T verifiableFinishDate, T verifiableStartDate) where T : struct
        {
            if (!DateHelper<T>.IsSupported)
                return ValidationResult.Fail(BadFormatFinishDateError);

            if (DateHelper<T>.CompareToNow(verifiableFinishDate) < 0)
                return ValidationResult.Fail(OldFinishDateError);

            if (DateHelper<T>.Compare(verifiableFinishDate, verifiableStartDate) < 0)
                return ValidationResult.Fail(FinishDateOlderStartDateError);

            return ValidationResult.Ok();
        }

        private static class StringConverter<T> where T : struct
        {
            public static readonly Func<string, T?> TryParse;

            static StringConverter()
            {
                string format = "yyyy-MM-dd";
                IFormatProvider provider = CultureInfo.InvariantCulture;
                DateTimeStyles style = DateTimeStyles.None;
                if (DateHelper<T>.IsDateOnly)
                {
                    TryParse = date => DateOnly.TryParseExact(date, format, provider, style, out DateOnly result) 
                        ? (T)(object)result
                        : null;
                }
                else if (DateHelper<T>.IsDateTime)
                {
                    TryParse = date => DateTime.TryParseExact(date, format, provider, style, out DateTime result)
                        ? (T)(object)result
                        : null;
                }
                else
                {
                    TryParse = _ => null;
                }
            }
        }

        public static T? ConvertStringDate<T>(string date) where T : struct 
            => !string.IsNullOrWhiteSpace(date) ? StringConverter<T>.TryParse(date) : null;

        public static ValidationResult StartDateValidation<T>(string verifiableStartDate, ref T? convertedDate) where T : struct
        {
            if (string.IsNullOrWhiteSpace(verifiableStartDate))
                return ValidationResult.Fail(EmptyStartDateError);

            convertedDate = ConvertStringDate<T>(verifiableStartDate);
            return convertedDate.HasValue
                ? StartDateValidation(convertedDate.Value)
                : ValidationResult.Fail(BadFormatStartDateError);
        }

        public static ValidationResult FinishDateValidation<T>(string verifiableFinishDate, string verifiableStartDate, ref T? convertedFinishDate) where T : struct
        {
            if (string.IsNullOrWhiteSpace(verifiableFinishDate))
                return ValidationResult.Fail(EmptyFinishDateError);

            convertedFinishDate = ConvertStringDate<T>(verifiableFinishDate);
            if (!convertedFinishDate.HasValue)
                return ValidationResult.Fail(BadFormatFinishDateError);

            if (string.IsNullOrWhiteSpace(verifiableStartDate))
                return ValidationResult.Fail(EmptyStartDateError);

            var convertedStartDate = ConvertStringDate<T>(verifiableStartDate);
            if (!convertedStartDate.HasValue)
                return ValidationResult.Fail(BadFormatStartDateError);

            return FinishDateValidation(convertedFinishDate.Value, convertedStartDate.Value);
        }
    }
}
