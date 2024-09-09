using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace HRMSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _documentPath;

        public DocumentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _documentPath = "/Users/mursal/Projects/HRMSystem/FileTemplates/VacationOrder.docx";
        }

        [HttpGet("ProcessDocument")]
        public async Task<IActionResult> ProcessDocument()
        {
            if (!System.IO.File.Exists(_documentPath))
                return NotFound("Document not found");

            // Load the Word document using NPOI
            using var stream = new FileStream(_documentPath, FileMode.Open, FileAccess.ReadWrite);
            var document = new XWPFDocument(stream);

            // Replace placeholders with images
            await SetBarcodeAsync(document);
            await SetQRCode(document);

            // Save the changes to a memory stream
            using var outputStream = new MemoryStream();
            document.Write(outputStream);
            outputStream.Position = 0; // Ensure the stream position is reset

            return File(outputStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ProcessedDocument.docx");
        }

        private async Task SetBarcodeAsync(XWPFDocument document)
        {
            string placeholder = "[QUID-DIGITAL-SIGNATURE-BARCODE]";
            byte[] imageBytes = await GenerateBarcode();

            foreach (var paragraph in document.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    if (run.ToString().Contains(placeholder))
                    {
                        run.SetText(run.Text.Replace(placeholder, string.Empty)); // Remove the placeholder

                        // Add the image to the run
                        using var imageStream = new MemoryStream(imageBytes);
                        var pictureType = (int)NPOI.XWPF.UserModel.PictureType.PNG;
                        var picture = run.AddPicture(imageStream, pictureType, $"{placeholder}.png", 664 * 914400 / 96, 100 * 914400 / 96); // Size in EMUs
                    }
                }
            }
        }
        private async Task SetQRCode(XWPFDocument document)
        {
            string placeholder = "[QUID-QR-CODE]";
            byte[] imageBytes = await GenerateQRCode();

            foreach (var paragraph in document.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    if (run.ToString().Contains(placeholder))
                    {
                        run.SetText(run.Text.Replace(placeholder, string.Empty)); // Remove the placeholder

                        // Add the image to the run
                        using var imageStream = new MemoryStream(imageBytes);
                        var pictureType = (int)NPOI.XWPF.UserModel.PictureType.PNG;
                        var picture = run.AddPicture(imageStream, pictureType, $"{placeholder}.png", 100 * 914400 / 96, 100 * 914400 / 96); // Size in EMUs
                    }
                }
            }
        }

        private async Task<byte[]> GenerateQRCode()
        {
            var adminUserData = "Admin User Data Here"; // Example admin user data
            var qrCodeImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(adminUserData)}&size=300x300";
            var response = await _httpClient.GetAsync(qrCodeImageUrl);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }

        private async Task<byte[]> GenerateBarcode()
        {
            var digitalSignatureData = "Digital Signature Data Here"; // Example digital signature data
            var barcodeImageUrl = $"https://barcode.tec-it.com/barcode.ashx?data={Uri.EscapeDataString(digitalSignatureData)}&code=Code128";
            var response = await _httpClient.GetAsync(barcodeImageUrl);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }
    }
}