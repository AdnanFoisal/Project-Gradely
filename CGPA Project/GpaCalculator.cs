using System;
using System.Collections.Generic;
using System.Text;

namespace CGPA_Project
{
    public static class GpaCalculator
    {
        public static double CalculateGpa(List<CourseResult> courses)
        {
            if (courses == null || courses.Count == 0)
                return 0.0;

            double totalPoints = 0;
            double totalCredits;

            foreach(var course in courses)
            {
                totalPoints += (course.Credit * course.GradePoint);
                totalCredits += course.Credits;
            }
            return totalCredits == 0 ? 0.0 : totalPoints / totalCredits;
        }
    }
}
