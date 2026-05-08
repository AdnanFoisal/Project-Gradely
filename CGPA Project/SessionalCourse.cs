using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CGPA_Project
{
    public class SessionalCourse : CourseResult
    {
        public SessionalCourse(string code,double credit,string grade,double gradePoint,string levelTerm) : base(code,credit,grade,gradePoint,levelTerm)
        { }

        public override string GetCourseInfo()
        {
            return $"[Sessional]{Code} | Credits: {Credit:F2} | Grade: {Grade} ({GradePoint:F2}) | Level-Term:{LevelTerm}";
        }
    }
}
