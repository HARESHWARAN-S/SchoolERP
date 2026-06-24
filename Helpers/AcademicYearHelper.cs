namespace SchoolERP.Helpers
{
    public static class AcademicYearHelper
    {
        public static string GetCurrentAcademicYear()
        {
            var today = DateTime.UtcNow;
            int year = today.Month <= 3
                ? today.Year - 1  
                : today.Year;     
            return $"{year}-{year + 1}";
        }

        public static bool IsValidAcademicYear(string academicYear)
        {
            string current = GetCurrentAcademicYear();
            return academicYear == current;
        }
    }
}