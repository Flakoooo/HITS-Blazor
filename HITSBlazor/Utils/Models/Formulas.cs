namespace HITSBlazor.Utils.Models
{
    public static class Formulas
    {
        public static double CalculcateRating(params int[] values)
            => Math.Round((double)values.Sum() / (double)values.Length, 1);
    }
}
