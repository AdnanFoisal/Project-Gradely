using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CGPA_Project
{
    public class RegularCourse: CourseResult
    {
        public RegularCourse(string code,double credit,string grade,double gradePoint,string levelTerm) : base(code,credit,grade,gradePoint,levelTerm)
        {

        }
        public override string GetCourseInfo()
        {
            return $"[Theory] {Code} | Credits: {Credit:F2} | Grade: {Grade} ({GradePoint:F2}) | Level-Term: {LevelTerm}";
        }
    }
}
