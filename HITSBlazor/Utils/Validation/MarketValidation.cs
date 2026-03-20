using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HITSBlazor.Utils.Validation
{
    public class MarketValidation
    {
        public static ValidationEvaluation NameValidation(string verifiableName)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableName))
            {
                result.IsValid = false;
                result.Message = "Название не может быть пустым";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation StartDateValidation(DateTime verifiableStartDate)
        {
            var result = new ValidationEvaluation();

            if (DateTime.UtcNow.Date > verifiableStartDate.Date)
            {
                result.IsValid = false;
                result.Message = "Дата начала не может быть раньше текущей";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation FinishDateValidation(DateTime verifiableFinishDate, DateTime verifiableStartDate)
        {
            var result = new ValidationEvaluation();

            if (DateTime.UtcNow.Date > verifiableFinishDate.Date)
            {
                result.IsValid = false;
                result.Message = "Дата окончания не может быть раньше текущей";
                return result;
            }

            if (verifiableFinishDate.Date < verifiableStartDate.Date)
            {
                result.IsValid = false;
                result.Message = "Дата окончания должна быть позже даты начала";
                return result;
            }

            return result;
        }

        private static DateTime ConvertStringToDate(string date) => DateTimeOffset.Parse(
            date, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal
        ).UtcDateTime;

        public static ValidationEvaluation StartDateValidation(string verifiableStartDate)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableStartDate))
            {
                result.IsValid = false;
                result.Message = "Дана начала не может быть пустой";
                return result;
            }

            if (DateTime.UtcNow.Date > ConvertStringToDate(verifiableStartDate).Date)
            {
                result.IsValid = false;
                result.Message = "Дата начала не может быть раньше текущей";
                return result;
            }

            return result;
        }

        public static ValidationEvaluation FinishDateValidation(string verifiableFinishDate, string verifiableStartDate)
        {
            var result = new ValidationEvaluation();

            if (string.IsNullOrWhiteSpace(verifiableFinishDate))
            {
                result.IsValid = false;
                result.Message = "Дана окончания не может быть пустой";
                return result;
            }

            var finishDate = ConvertStringToDate(verifiableFinishDate);

            if (DateTime.UtcNow.Date > finishDate.Date)
            {
                result.IsValid = false;
                result.Message = "Дата окончания не может быть раньше текущей";
                return result;
            }

            if (finishDate.Date < ConvertStringToDate(verifiableStartDate).Date)
            {
                result.IsValid = false;
                result.Message = "Дата окончания должна быть позже даты начала";
                return result;
            }

            return result;
        }
    }
}
