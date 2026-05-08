using System;
using System.Collections.Generic;
using System.Text;

namespace CGPA_Project
{
    public abstract class CourseResults
    {
        private string code;
        private double credit;
        private string grade;
        private double gradePoint;
        public string Code
        {
            get { return code; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Course code Cannot be empty");
                code = value;
            }
        }
        public double Credit
        {
            get { return credit; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Credit Cannot be negative");
                credit = value;

            }
        }
        public string Grade
        {
            get { return grade; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Grade Cannot be Empty");
                grade = value;
            }
        }
        public double GradePoint
        {
            get { return gradePoint; }
            set
            {
                if (value < 0 || value > 4.0)
                    throw new ArgumentException("Grade point must be between 0 and 4.0");
                gradePoint = value;
            }
        }
        public string LevelTerm
        {
            get;
            set;
        }

        protected CourseResults(string code, double credit, string grade, double gradePoint, string levelTerm)
        {
            Code = code;
            Credit = credit;
            Grade = grade;
            GradePoint = gradePoint;
            LevelTerm = levelTerm;
        }
        public abstract string GetCourseInfo();
    }
}
