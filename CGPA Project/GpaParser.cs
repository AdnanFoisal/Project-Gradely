using System;
using System.Collections.Generic;
using System.Text;

namespace CGPA_Project
{
    public static class GpaParser
    {
        public static double GetGradePoint(string grade)
        {
            return grade.ToUpper().Trim() switch
            {
                "A+" => 4.00,
                "A" => 3.75,
                "A-" => 3.50,
                "B+" => 3.25,
                "B" => 3.00,
                "B-" => 2.75,
                "C+" => 2.50,
                "C" => 2.25,
                "D" => 2.00,
                "F" => 0.00,
                _ => 0.00
            };
        }
        public static List<CourseResult> Parse(string rawInput)
        {
            var courses = new List<CourseResult>();
            
                try
                {
                    string[] lines = rawInput.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    bool startParsing = false;

                    var regex = new System.Text.RegularExpressions.Regex(@"^(\S+)\s+([\d\.]+)\s+(.*?)\s+(Yes|No)\s+([A-DF][+-]?)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                    foreach (var line in lines)
                    {
                        string tLine = line.Trim();
                        if (tLine.StartsWith("Showing", StringComparison.OrdinalIgnoreCase))
                            break;

                        if (tLine.Contains("Course Code") && tLine.Contains("Result"))
                        {
                            startParsing = true;
                            continue;
                        }

                        if (startParsing)
                        {
                            var match = regex.Match(tLine);
                            if (match.Success)
                            {
                                string code = match.Groups[1].Value;
                                if (!double.TryParse(match.Groups[2].Value, out double credit)) continue;
                                string levelTerm = match.Groups[3].Value.Trim();
                                string sessionalStr = match.Groups[4].Value;
                                string grade = match.Groups[5].Value.ToUpper();

                                double gradePoint = GetGradePoint(grade);

                                if (sessionalStr.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                                {
                                    courses.Add(new SessionalCourse(code, credit, grade, gradePoint, levelTerm));
                                }
                                else
                                {
                                    courses.Add(new RegularCourse(code, credit, grade, gradePoint, levelTerm));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error parsing data. Please ensure it matches the portal format.", ex);
                }

                return courses;
            }
    }
    }