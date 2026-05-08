using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace CGPACalculator
{
    public static class PdfExporter
    {
        public static byte[] Export(List<CourseResult> courses, double gpa)
        {
            try
            {
                PdfDocument document = new PdfDocument();
                document.Info.Title = "CUET Grade Sheet";

                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont titleFont = new XFont("Times New Roman", 15, XFontStyleEx.Bold);
                XFont deptFont = new XFont("Times New Roman", 14, XFontStyleEx.Regular);
                XFont subtitleFont = new XFont("Times New Roman", 13, XFontStyleEx.Bold | XFontStyleEx.Underline);
                XFont boldFont = new XFont("Times New Roman", 11, XFontStyleEx.Bold);
                XFont regularFont = new XFont("Times New Roman", 11, XFontStyleEx.Regular);
                XFont smallFont = new XFont("Times New Roman", 9, XFontStyleEx.Regular);
                XFont smallBoldFont = new XFont("Times New Roman", 9, XFontStyleEx.Bold);

                int yPoint = 40;

                // Center logo
                try
                {
                    string logoPath = Path.Combine(AppContext.BaseDirectory, "cuet_logo.png");
                    if (File.Exists(logoPath))
                    {
                        XImage logo = XImage.FromFile(logoPath);
                        double logoWidth = 60;
                        double logoHeight = 60;
                        gfx.DrawImage(logo, (page.Width - logoWidth) / 2, yPoint, logoWidth, logoHeight);
                        yPoint += (int)logoHeight + 10;
                    }
                    else
                    {
                        yPoint += 60;
                    }
                }
                catch { yPoint += 60; }

                // University Name
                gfx.DrawString("CHITTAGONG UNIVERSITY OF ENGINEERING & TECHNOLOGY", titleFont, XBrushes.DarkBlue, new XRect(0, yPoint, page.Width, 20), XStringFormats.Center);
                yPoint += 25;

                // Grade Sheet
                gfx.DrawString("Grade Sheet", subtitleFont, XBrushes.Black, new XRect(0, yPoint, page.Width, 20), XStringFormats.Center);
                yPoint += 35;

                string levelTerm = courses.FirstOrDefault()?.LevelTerm ?? "Unknown";
                
                // Info on left
                gfx.DrawString($"{levelTerm}", boldFont, XBrushes.Black, 40, yPoint);
                yPoint += 30;

                // Table Layout
                int tableStartX = 40;
                int colNoWidth = 120;
                int colCreditWidth = 70;
                int colGradeWidth = 70;
                int colPointWidth = 70;
                
                int tableWidth = colNoWidth + colCreditWidth + colGradeWidth + colPointWidth;

                // Draw Table Header
                gfx.DrawRectangle(XPens.Black, tableStartX, yPoint, colNoWidth, 24);
                gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth, yPoint, colCreditWidth, 24);
                gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth + colCreditWidth, yPoint, colGradeWidth, 24);
                gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth + colCreditWidth + colGradeWidth, yPoint, colPointWidth, 24);

                DrawCenteredText(gfx, "Course No.", boldFont, tableStartX, yPoint, colNoWidth, 24);
                
                DrawCenteredText(gfx, "Credit", boldFont, tableStartX + colNoWidth, yPoint + 2, colCreditWidth, 10);
                DrawCenteredText(gfx, "Hours", boldFont, tableStartX + colNoWidth, yPoint + 12, colCreditWidth, 10);

                DrawCenteredText(gfx, "Grade", boldFont, tableStartX + colNoWidth + colCreditWidth, yPoint + 2, colGradeWidth, 10);
                DrawCenteredText(gfx, "(G)", boldFont, tableStartX + colNoWidth + colCreditWidth, yPoint + 12, colGradeWidth, 10);

                DrawCenteredText(gfx, "Grade", boldFont, tableStartX + colNoWidth + colCreditWidth + colGradeWidth, yPoint + 2, colPointWidth, 10);
                DrawCenteredText(gfx, "Point", boldFont, tableStartX + colNoWidth + colCreditWidth + colGradeWidth, yPoint + 12, colPointWidth, 10);

                yPoint += 24;

                double totalCredits = 0;
                foreach (var course in courses)
                {
                    totalCredits += course.Credit;

                    gfx.DrawRectangle(XPens.Black, tableStartX, yPoint, colNoWidth, 20);
                    gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth, yPoint, colCreditWidth, 20);
                    gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth + colCreditWidth, yPoint, colGradeWidth, 20);
                    gfx.DrawRectangle(XPens.Black, tableStartX + colNoWidth + colCreditWidth + colGradeWidth, yPoint, colPointWidth, 20);

                    gfx.DrawString(" " + course.Code, regularFont, XBrushes.Black, new XRect(tableStartX, yPoint, colNoWidth, 20), XStringFormats.CenterLeft);
                    DrawCenteredText(gfx, course.Credit.ToString("F2"), regularFont, tableStartX + colNoWidth, yPoint, colCreditWidth, 20);
                    DrawCenteredText(gfx, course.Grade, regularFont, tableStartX + colNoWidth + colCreditWidth, yPoint, colGradeWidth, 20);
                    DrawCenteredText(gfx, course.GradePoint.ToString("F2"), regularFont, tableStartX + colNoWidth + colCreditWidth + colGradeWidth, yPoint, colPointWidth, 20);
                    yPoint += 20;
                }

                // Summary
                yPoint += 15;
                XFont gpaFont = new XFont("Times New Roman", 14, XFontStyleEx.Bold);
                gfx.DrawString($"GPA:    {gpa:F2}", gpaFont, XBrushes.Black, tableStartX, yPoint + 10);
                yPoint += 35;

                gfx.DrawString($"Registered Credit Hours in this Term:  {totalCredits:F2}", regularFont, XBrushes.Black, tableStartX, yPoint);
                yPoint += 20;
                gfx.DrawString($"Credit Hours earned in this Term:      {totalCredits:F2}", regularFont, XBrushes.Black, tableStartX, yPoint);

                // Date at the bottom
                gfx.DrawString($"Date: {DateTime.Now:dd/MM/yyyy}", boldFont, XBrushes.Black, tableStartX, page.Height - 100);

                // GRADING SYSTEM TABLE (on the right)
                int gradingStartX = tableStartX + tableWidth + 40;
                int gradingStartY = 160;

                gfx.DrawString("GRADING SYSTEM", smallBoldFont, XBrushes.Black, new XRect(gradingStartX, gradingStartY - 15, 140, 15), XStringFormats.Center);
                
                string[,] gradingSystem = new string[,] {
                    { "80% or Above", "A+", "4.00" },
                    { ">= 75% but < 80%", "A", "3.75" },
                    { ">= 70% but < 75%", "A-", "3.50" },
                    { ">= 65% but < 70%", "B+", "3.25" },
                    { ">= 60% but < 65%", "B", "3.00" },
                    { ">= 55% but < 60%", "B-", "2.75" },
                    { ">= 50% but < 55%", "C+", "2.50" },
                    { ">= 45% but < 50%", "C", "2.25" },
                    { ">= 40% but < 45%", "D", "2.00" },
                    { "Less than 40%", "F", "0.00" }
                };

                int gCol1 = 80;
                int gCol2 = 30;
                int gCol3 = 30;
                int gRowH = 12;

                // header
                gfx.DrawRectangle(XPens.Black, gradingStartX, gradingStartY, gCol1, gRowH * 2);
                gfx.DrawRectangle(XPens.Black, gradingStartX + gCol1, gradingStartY, gCol2, gRowH * 2);
                gfx.DrawRectangle(XPens.Black, gradingStartX + gCol1 + gCol2, gradingStartY, gCol3, gRowH * 2);

                DrawCenteredText(gfx, "Marks", smallBoldFont, gradingStartX, gradingStartY, gCol1, gRowH * 2);
                
                DrawCenteredText(gfx, "Letter", smallBoldFont, gradingStartX + gCol1, gradingStartY + 2, gCol2, gRowH);
                DrawCenteredText(gfx, "Grade", smallBoldFont, gradingStartX + gCol1, gradingStartY + gRowH + 2, gCol2, gRowH);
                
                DrawCenteredText(gfx, "Grade", smallBoldFont, gradingStartX + gCol1 + gCol2, gradingStartY + 2, gCol3, gRowH);
                DrawCenteredText(gfx, "Point", smallBoldFont, gradingStartX + gCol1 + gCol2, gradingStartY + gRowH + 2, gCol3, gRowH);

                gradingStartY += gRowH * 2;

                for (int i = 0; i < gradingSystem.GetLength(0); i++)
                {
                    gfx.DrawRectangle(XPens.Black, gradingStartX, gradingStartY, gCol1, gRowH);
                    gfx.DrawRectangle(XPens.Black, gradingStartX + gCol1, gradingStartY, gCol2, gRowH);
                    gfx.DrawRectangle(XPens.Black, gradingStartX + gCol1 + gCol2, gradingStartY, gCol3, gRowH);

                    gfx.DrawString(" " + gradingSystem[i, 0], smallFont, XBrushes.Black, new XRect(gradingStartX, gradingStartY, gCol1, gRowH), XStringFormats.CenterLeft);
                    DrawCenteredText(gfx, gradingSystem[i, 1], smallFont, gradingStartX + gCol1, gradingStartY, gCol2, gRowH);
                    DrawCenteredText(gfx, gradingSystem[i, 2], smallFont, gradingStartX + gCol1 + gCol2, gradingStartY, gCol3, gRowH);
                    gradingStartY += gRowH;
                }

                gradingStartY += 10;
                gfx.DrawString("Grade point average in a term is", smallFont, XBrushes.Black, gradingStartX, gradingStartY);
                gradingStartY += 12;
                gfx.DrawString("defined as,", smallFont, XBrushes.Black, gradingStartX, gradingStartY);
                gradingStartY += 15;
                XFont mathFont = new XFont("Times New Roman", 11, XFontStyleEx.Bold);
                gfx.DrawString("GPA = Σ(C_i * G_i) / Σ C_i", mathFont, XBrushes.Black, gradingStartX, gradingStartY);
                
                // Save to memory stream instead of disk
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream, false);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to export PDF.", ex);
            }
        }

        private static void DrawCenteredText(XGraphics gfx, string text, XFont font, double x, double y, double width, double height)
        {
            gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, height), XStringFormats.Center);
        }
    }
}