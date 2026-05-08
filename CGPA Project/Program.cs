using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace CGPA_Project
{
    class Program
    {
        static List<CourseResult> courseResults = new List<CourseResult>();

        static double currentGpa = 0.0;
        static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.Clear();
                Console.WriteLine("╔══════════════════════════════════╗");
                Console.WriteLine("║    CGPA Calculator — CUET        ║");
                Console.WriteLine("╠══════════════════════════════════╣");
                Console.WriteLine("║  1. Paste & Calculate GPA        ║");
                Console.WriteLine("║  2. View Detailed Results        ║");
                Console.WriteLine("║  3. Export PDF Grade Sheet       ║");
                Console.WriteLine("║  4. Exit                         ║");
                Console.WriteLine("╚══════════════════════════════════╝");
                Console.Write("Select an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleInput();
                        break;
                    case "2":
                        ViewResults();
                        break;
                    case "3":
                        ExportPdf();
                        break;
                    case "4":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again");
                        Console.ReadKey();
                        break;
                }

            }
        }
        static void HandleInput()
        {
            Console.Clear();
            Console.WriteLine("Paste raw text from CUET portal");
            Console.WriteLine("Type END ona new line and press Enter when finished :\n");
            StringBuilder sb = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != "END")
            {

                if (line == null)
                    break;
                sb.AppendLine(line);


            }
            try
            {
                courseResults = GpaParser.Parse(sb.ToString());
                currentGpa = GpaCalculator.CalculateGpa(courseResults);

                if (courseResults.Count > 0)
                {
                    Console.WriteLine($"successfully parsed {courseResults.Count} courses");
                    Console.WriteLine($"Calculated CGPA: {currentGpa:F2}");
                }
                else
                {
                    Console.WriteLine("\n No valid courses parsed.Ensure exact column Format");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Error :{ex.Message}");
            }
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
        }
        static void ViewResults()
        {
            Console.Clear();
            if (courseResults.Count == 0)
            {
                Console.WriteLine("No data available.Please paste data first");

            }
            else
            {
                Console.WriteLine("Detailed Results:\n");
                foreach (var course in courseResults)
                {
                    Console.WriteLine(course.GetCourseInfo());
                }
                Console.WriteLine($"\n Overall total credits:{GetTotalCredits():F2}");
                Console.WriteLine($"Overall CGPA : {currentGpa:F2}");

            }
            Console.WriteLine("\n Press any key to return to menu");
            Console.ReadKey();
        }

        static void ExportPdf()
        {
            Console.Clear();
            if (courseResults.Count == 0)
            {
                Console.WriteLine("No data available to export.Please paste data first");

            }

            else
            {
                Console.Write("Enter filename to save PDF (e.g., GradeSheet.pdf): ");
                string filename = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(filename)) filename = "GradeSheet.pdf";
                if (!filename.EndsWith(".pdf")) filename += ".pdf";

                string outputDirectory = AppContext.BaseDirectory;
                string path = System.IO.Path.Combine(outputDirectory, filename);

                try
                {
                    System.IO.Directory.CreateDirectory(outputDirectory);
                    byte[] pdfBytes = PdfExporter.Export(courseResults, currentGpa);
                    System.IO.File.WriteAllBytes(path, pdfBytes);
                    Console.WriteLine($"\n PDF successfully exported and saved to:");
                    Console.WriteLine($"{System.IO.Path.GetFullPath(path)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error exporting PDF : {ex.Message}");
                }
            }
            Console.WriteLine("Press any key to return to menu");
            Console.ReadKey();
        }
        private static double GetTotalCredits()
        {
            double total = 0;
            foreach (var c in courseResults)
            {
                total += c.Credit;

            }
            return total;
        }
    }
}