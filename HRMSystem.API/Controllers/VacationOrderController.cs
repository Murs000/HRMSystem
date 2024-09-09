using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;
using System;
using System.IO;


namespace HRMSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class VacationOrderController : ControllerBase
{
    [HttpGet("generate-vacation-order")]
    public IActionResult GenerateVacationOrder()
    {
        // Generate the Word document and return it as a file
        var wordFile = CreateFancyVacationOrderWithQuids();
        return File(wordFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "VacationOrder.docx");
    }

    // Function to create a fancy Word file with quids
    private byte[] CreateFancyVacationOrderWithQuids()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            // Create a new Word document
            XWPFDocument doc = new XWPFDocument();

            // Fancy Header
            XWPFParagraph header = doc.CreateParagraph();
            header.Alignment = ParagraphAlignment.CENTER;
            XWPFRun headerRun = header.CreateRun();
            headerRun.SetText("Vacation Leave Order");
            headerRun.IsBold = true;
            headerRun.FontFamily = "Times New Roman";
            headerRun.FontSize = 20;

            // Add some space between header and content
            AddEmptyLine(doc, 2);

            // Add detailed information
            AddParagraph(doc, "Order No: [QUID-ORDER-ID]", true, 12);
            AddParagraph(doc, "Date: [QUID-ORDER-DATE]", true, 12);
            AddEmptyLine(doc, 1);

            // Body Content: Employee details
            AddParagraph(doc, "To:", false, 12);
            AddParagraph(doc, "[QUID-EMPLOYEE-NAME] [QUID-EMPLOYEE-SURNAME]", true, 14);
            AddParagraph(doc, "Position: [QUID-EMPLOYEE-POSITION]", false, 12);
            AddEmptyLine(doc, 1);

            // Body Content: Vacation details
            AddParagraph(doc, "Subject: Grant of Vacation Leave", true, 14);
            AddParagraph(doc, "Dear [QUID-EMPLOYEE-NAME],", false, 12);
            AddParagraph(doc, "We are pleased to inform you that your request for vacation leave has been approved.", false, 12);
            AddParagraph(doc, "You will be on vacation from [QUID-VACATION-START] to [QUID-VACATION-END].", false, 12);
            AddParagraph(doc, "Please make sure to hand over your tasks before leaving.", false, 12);
            AddEmptyLine(doc, 2);

            // Footer: Admin details
            AddParagraph(doc, "Sincerely,", false, 12);
            AddParagraph(doc, "_______________________", false, 12);  // For signature
            AddParagraph(doc, "Admin: [QUID-ADMIN-NAME]", false, 12);
            AddParagraph(doc, "Position: [QUID-ADMIN-POSITION]", false, 12);

            AddEmptyLine(doc, 3);

            // Add QR Code placeholder in the right bottom corner
            XWPFParagraph qrParagraph = doc.CreateParagraph();
            XWPFRun qrRun = qrParagraph.CreateRun();
            qrRun.SetText("[QUID-QR-CODE]");
            qrParagraph.Alignment = ParagraphAlignment.RIGHT;
            qrRun.FontFamily = "Courier";  // QR code font look
            qrRun.FontSize = 10;
            qrRun.SetColor("000000");

            AddEmptyLine(doc, 1);

            // Add Digital Signature placeholder in the left bottom corner
            XWPFParagraph signatureParagraph = doc.CreateParagraph();
            XWPFRun signatureRun = signatureParagraph.CreateRun();
            signatureRun.SetText("[QUID-DIGITAL-SIGNATURE-BARCODE]");
            signatureParagraph.Alignment = ParagraphAlignment.LEFT;
            signatureRun.FontFamily = "Courier";
            signatureRun.FontSize = 10;
            signatureRun.SetColor("000000");

            // Save the document to memory stream
            doc.Write(ms);

            // Return the byte array of the document
            return ms.ToArray();
        }
    }

    // Utility function to add paragraphs with styling options
    private void AddParagraph(XWPFDocument doc, string text, bool isBold, int fontSize)
    {
        XWPFParagraph paragraph = doc.CreateParagraph();
        XWPFRun run = paragraph.CreateRun();
        run.SetText(text);
        run.FontSize = fontSize;
        run.IsBold = isBold;
        run.FontFamily = "Calibri";
    }

    // Utility function to add empty lines
    private void AddEmptyLine(XWPFDocument doc, int numberOfLines)
    {
        for (int i = 0; i < numberOfLines; i++)
        {
            XWPFParagraph paragraph = doc.CreateParagraph();
            XWPFRun run = paragraph.CreateRun();
            run.SetText("\n");
        }
    }
}
