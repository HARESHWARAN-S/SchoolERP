using SchoolERP.Exceptions;
using SchoolERP.Repositories.Interfaces;

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

        // Resolves ClassId from class+sec using current academic year
        public static async Task<int> GetClassIdAsync(
            string Class,
            string sec,
            IStudentClassRepository studentClassRepo)
        {
            string academicYear = GetCurrentAcademicYear();
            var studentClass = await studentClassRepo.GetCurrentAsync(Class, sec, academicYear);
            if (studentClass == null)
                throw new NoActiveClassForCurrentYearException(Class, sec, academicYear);
            return studentClass.ClassId;
        }
    }
}