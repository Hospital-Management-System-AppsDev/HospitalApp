using HospitalApp.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

class PdfServices
{
    public static void GenerateMedicalCertificate(Appointment appointment, Patient patient, MedicalCertificate medcert)
    {
        QuestPDF.Settings.License = LicenseType.Community; // <-- Add this line

        string documentType = "MEDICAL CERTIFICATE"; // Change to "Prescription" as needed
        string date = DateTime.Now.ToString("MM/dd/yyyy");

        string headerImagePath = "./Resources/ForPDF/mcheader.png"; // Update with actual image path
        string footerImagePath = "./Resources/ForPDF/mcfooter.png"; // Update with actual image path
        string signature = "./Resources/ForPDF/signature.png";
        var fileName = $"./Records/MedicalCertificates/MC_{appointment.PkId}_{DateTime.Now:yyyyMMdd}_{patient.PatientID}_{patient.Name}.pdf";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.PageColor(Colors.White);
                
                page.Header()
                    .Image(headerImagePath)
                    .FitWidth();

                page.Content()
                    .PaddingVertical(50)
                    .Column(col =>
                    {
                        col.Item().Text($"{documentType}").FontSize(20).Bold().AlignCenter();
                        col.Item().PaddingBottom(10).Text($"Date: {date}").FontSize(14).Underline().AlignEnd();
                        col.Item().PaddingBottom(10).Text("TO WHOMEVER IT MAY CONCERN").FontSize(14).Bold();
                        col.Item().PaddingBottom(20).Text(txt =>
                        {
                            txt.Span("This is to certify that ").FontSize(12);
                            txt.Span($"{(patient.Sex=="Male"? "Mr." : "Ms.")} {patient.Name}").FontSize(12).Underline().Italic();
                            txt.Span($", {patient.Sex}").FontSize(12).Underline().Italic();
                            txt.Span($", {patient.Age}").FontSize(12).Underline().Italic();
                            txt.Span($" years old, residing at ").FontSize(12);
                            txt.Span($"{patient.Address}").FontSize(12).Underline().Italic();
                            txt.Span(" was under my treatment since ").FontSize(12);
                            txt.Span($"{date}").FontSize(12).Underline().Italic();
                            txt.Span($" suffering from ").FontSize(12);
                            txt.Span($"{medcert.disease.Replace("\n", ", ")}").FontSize(12).Underline().Italic();
                            txt.Span($". {(patient.Sex == "Male" ? "He" : "She")} is/was advised treatment or rest for this period ").FontSize(12);
                            txt.Span($"{date} to {medcert.period:MM/dd/yyyy}").FontSize(12).Underline().Italic();
                            txt.Span(".").FontSize(12);
                        });

                        col.Item().PaddingBottom(20).Text("Sincerely,").FontSize(12);
                        col.Item()
                            .Width(100) // Limit the width of the signature
                            .Height(20) // Limit the height of the signature
                            .Image(signature);
                        col.Item().PaddingBottom(10).PaddingHorizontal(10).Text(appointment.AssignedDoctor.Name).FontSize(12).Underline();
                        col.Item().PaddingBottom(10).PaddingHorizontal(10).Text(appointment.AssignedDoctor.specialization).FontSize(12);
                    });

                page.Footer()
                    .Image(footerImagePath)
                    .FitWidth();
            });
        })
        .GeneratePdf(fileName);

        Console.WriteLine($"PDF Generated: {fileName}");
    }

    public static void GeneratePrescription(Appointment appointment, Patient patient, string prescription)
    {
        QuestPDF.Settings.License = LicenseType.Community; // <-- Add this line

        string date = DateTime.Now.ToString("MM/dd/yyyy");

        string headerImagePath = "./Resources/ForPDF/stet.webp"; // Update with actual image path
        string footerImagePath = "./Resources/ForPDF/mcfooter.png"; // Update with actual image path
        string rxlogo = "./Resources/ForPDF/rx.png";
        string signature = "./Resources/ForPDF/signature.png";
        string fileName = $"./Records/Prescriptions/RX_{appointment.PkId}_{DateTime.Now:yyyyMMdd}_{patient.PatientID}_{patient.Name}.pdf";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.PageColor(Colors.White);
                
                page.Header().Row(row =>{
                    row.ConstantItem(100)
                    .Height(100)
                    .Image(headerImagePath);

                    row.RelativeItem()
                    .PaddingLeft(50) // Space between image and text
                    .Column(col =>
                    {
                        col.Item().Text($"{appointment.AssignedDoctor.Name}")
                            .FontSize(50).Bold().FontColor("#004aad");

                        col.Item().Text($"{appointment.AssignedDoctor.specialization}")
                            .FontSize(20).Bold().FontColor("#004aad");
                    });
                });

                page.Content()
                    .PaddingVertical(50)
                    .Column(col =>
                    {
                        col.Item().AlignRight().Text(text=>{
                            text.Span("Date: ").FontSize(12).Bold();
                            text.Span($"{date}").FontSize(12).Underline().Italic();
                        });

                        col.Item().Row(row =>{
                            row.RelativeItem().Text(text=>{
                                text.Span("Patient Name:").FontSize(12).Bold();
                                text.Span($" {patient.Name}").FontSize(12).Bold().Italic().Underline();
                            });
                            row.RelativeItem().Text(text=>{
                                text.Span("Patient Id:").FontSize(12).Bold();
                                text.Span($" {patient.PatientID}").FontSize(12).Bold().Italic().Underline();
                            });
                        });

                        col.Item().Row(row =>{
                            row.RelativeItem().Text(text=>{
                            row.RelativeItem().Text(text=>{
                                text.Span("Age:").FontSize(12).Bold();
                                text.Span($" {patient.Age}").FontSize(12).Bold().Italic().Underline();
                            });
                                text.Span("Gender:").FontSize(12).Bold();
                                text.Span($" {patient.Sex}").FontSize(12).Bold().Italic().Underline();
                            });
                        });

                        col.Item().Text(text=>{
                            text.Span("Address: ").FontSize(12).Bold();
                            text.Span($"{patient.Address}").FontSize(12).Underline().Italic();
                        });

                        col.Item().Text("PRESCRIPTION").AlignCenter().FontSize(15).Bold();

                        col.Item().PaddingVertical(20).Height(50).Width(50).Image(rxlogo);

                        col.Item().Text($"• {prescription.Replace("\n", "\n• ")}");
                    });

                page.Footer().Column(col=>{

                    col.Item()
                            .Width(100) // Limit the width of the signature
                            .Height(20) // Limit the height of the signature
                            .Image(signature);
                        col.Item().PaddingBottom(10).PaddingHorizontal(10).Text(appointment.AssignedDoctor.Name).FontSize(12).Underline();
                        col.Item().PaddingBottom(10).PaddingHorizontal(10).Text(appointment.AssignedDoctor.specialization).FontSize(12);

                    col.Item().Image(footerImagePath)
                    .FitWidth();
                });
                    
            });

        })
        .GeneratePdf(fileName);

        Console.WriteLine($"PDF Generated: {fileName}");
    }

    public static void GenerateDiagnosis(Appointment appointment, Patient patient, Diagnosis diagnosis)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        string documentType = "MEDICAL DIAGNOSIS REPORT";
        string date = DateTime.Now.ToString("MM/dd/yyyy");

        string headerImagePath = "./Resources/ForPDF/mcheader.png";
        string footerImagePath = "./Resources/ForPDF/mcfooter.png";
        string signature = "./Resources/ForPDF/signature.png";

        var fileName = $"./Records/Diagnosis/D_{appointment.PkId}_{DateTime.Now:yyyyMMdd}_{patient.PatientID}_{patient.Name}.pdf";

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.PageColor(Colors.White);

                page.Header().Image(headerImagePath).FitWidth();

                page.Content().PaddingVertical(40).Column(col =>
                {
                    col.Item().Text(documentType).FontSize(20).Bold().AlignCenter();
                    col.Item().PaddingBottom(5).Text($"Date: {date}").FontSize(14).Underline().AlignEnd();
                    col.Item().PaddingBottom(5).Text($"Appointment ID: {appointment.PkId}").FontSize(12).AlignEnd();

                    col.Item().PaddingBottom(10).Text("TO WHOM IT MAY CONCERN").FontSize(14).Bold();

                    // Patient Information
                    col.Item().PaddingBottom(15).Text(txt =>
                    {
                        txt.Span("This is to certify that ").FontSize(12);
                        txt.Span($"{(patient.Sex == "Male" ? "Mr." : "Ms.")} {patient.Name}, {patient.Sex}, {patient.Age} years old")
                            .Italic().Underline();
                        txt.Span(" residing at ").FontSize(12);
                        txt.Span(patient.Address).Italic().Underline();
                        txt.Span(" has been examined and the following medical information was recorded:").FontSize(12);
                    });

                    // Diagnosis Sections
                    if (!string.IsNullOrWhiteSpace(diagnosis.condition))
                    {
                        col.Item().PaddingBottom(5).Text("Condition/s:").FontSize(12).Bold();

                        var cond = diagnosis.condition
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        col.Item().Column(list =>
                        {
                            foreach (var condition in cond)
                            {
                                list.Item().Text(text =>
                                {
                                    text.Span("• ").Bold();
                                    text.Span(condition);
                                });
                            }
                        });
                    }

                    col.Item().PaddingBottom(5).Text("").FontSize(12).Bold();

                    if (!string.IsNullOrWhiteSpace(diagnosis.symptoms))
                    {
                        col.Item().PaddingBottom(5).Text("Symptom/s:").FontSize(12).Bold();

                        var symp = diagnosis.symptoms
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        col.Item().Column(list =>
                        {
                            foreach (var sym in symp)
                            {
                                list.Item().Text(text =>
                                {
                                    text.Span("• ").Bold();
                                    text.Span(sym);
                                });
                            }
                        });
                    }

                    col.Item().PaddingBottom(5).Text("").FontSize(12).Bold();

                    if (!string.IsNullOrWhiteSpace(diagnosis.findings))
                    {
                        col.Item().PaddingBottom(5).Text("Finding/s:").FontSize(12).Bold();

                        var finds = diagnosis.findings
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        col.Item().Column(list =>
                        {
                            foreach (var find in finds)
                            {
                                list.Item().Text(text =>
                                {
                                    text.Span("• ").Bold();
                                    text.Span(find);
                                });
                            }
                        });
                    }

                    col.Item().PaddingBottom(5).Text("").FontSize(12).Bold();

                    // Recommendations
                    if (!string.IsNullOrWhiteSpace(diagnosis.recommendations))
                    {
                        col.Item().PaddingBottom(5).Text("Recommendations:").FontSize(12).Bold();

                        var recs = diagnosis.recommendations
                            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                        col.Item().Column(list =>
                        {
                            foreach (var rec in recs)
                            {
                                list.Item().Text(text =>
                                {
                                    text.Span("• ").Bold();
                                    text.Span(rec);
                                });
                            }
                        });
                    }

                    // Doctor Signature
                    col.Item().PaddingTop(30).Text("Sincerely,").FontSize(12);
                    col.Item().Width(100).Height(20).Image(signature);
                    col.Item().Text(appointment.AssignedDoctor.Name).FontSize(12).Underline();
                    col.Item().Text(appointment.AssignedDoctor.specialization).FontSize(12);
                });

                page.Footer().Image(footerImagePath).FitWidth();
            });
        })
        .GeneratePdf(fileName);

        Console.WriteLine($"PDF Generated: {fileName}");
    }

}
