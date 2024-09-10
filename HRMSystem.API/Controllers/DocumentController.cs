using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NPOI.OpenXmlFormats.Dml;
using NPOI.POIFS.NIO;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public DocumentController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateDocumentAndQRCode()
        {
            string inputFile = "/Users/mursal/Projects/HRMSystem/FileTemplates/VacationOrder.docx";
            var order = new VacationLeaveOrder
            {
                OrderId = "98765",
                OrderDate = "2024-09-15",
                EmployeeName = "Alice",
                EmployeeSurname = "Johnson",
                EmployeePosition = "Project Manager",
                VacationStart = "2024-10-01",
                VacationEnd = "2024-10-14",
                AdminName = "Bob Smith",
                AdminPosition = "Office Manager",
                QrCode = "QR0987654321"
            };

            // Read DOCX file into a byte array
            byte[] fileData = System.IO.File.ReadAllBytes(inputFile);

            // Replace placeholders in DOCX
            fileData = ReplacePlaceholders(fileData, order);

            // Create digital signature
            string signature = CreateSignature(fileData,order.AdminName);

            // Generate QR code
            byte[] qrCode = await GenerateQRCode(signature, order.AdminName);

            // Set QR code in the document
            fileData = SetQRCode(fileData, qrCode);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = "UpdatedVacationOrder.docx";

            return File(fileData, contentType, fileName);
        }
        [HttpPost("verify")]
        public async Task<IActionResult> VerifyDocumentAndQRCode(IFormFile file)
        {
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream); // Asynchronously copy the file content to memory
                fileData = memoryStream.ToArray(); // Convert the memory stream to a byte array
            }

            var qrImage = ExtractImage(fileData);
            var result = await DecodeQRCode(qrImage);
            var responce = DeserialiResponce(result);

            string signature = CreateSignature(fileData,responce.AdminName);

            if(signature == responce.DigitalSignature)
            {
                return Ok("Verify passed");
            }

            return BadRequest("Not Verified");
        }
        private QRResponce DeserialiResponce(string apiResponce)
        {
            // Parse the JSON response
            JArray parsedResponse = JArray.Parse(apiResponce);

            // Extract the data field
            string data = parsedResponse[0]["symbol"][0]["data"].ToString();

            // Split the data into "Admin" and the digital signature
            string[] parts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
            string admin = parts[0]; // Admin
            string digitalSignature = parts[1]; // Digital signature

            return new QRResponce(admin, digitalSignature);
        }
        public record QRResponce(string AdminName, string DigitalSignature);
        private static async Task<string> DecodeQRCode(byte[] qrCodeBytes)
        {
            using (var client = new HttpClient())
            {
                // API endpoint for goqr.me service
                string url = "https://api.qrserver.com/v1/read-qr-code/";

                // Convert the byte array to base64
                var base64Image = Convert.ToBase64String(qrCodeBytes);

                /// Create form data with the image
                var content = new MultipartFormDataContent();
                var imageContent = new ByteArrayContent(qrCodeBytes);
                imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
                content.Add(imageContent, "file", "qrcode.png");

                // Send POST request
                var response = await client.PostAsync(url, content);

                // Get the result
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return jsonResponse;
                }
                else
                {
                    return "Error decoding QR code";
                }
            }
        }

        private byte[] ExtractImage(byte[] fileData)
        {
            byte[] image = null;
            using (var documentStream = new MemoryStream(fileData))
            {
                XWPFDocument document = new XWPFDocument(documentStream);
        
                foreach (var pictureData in document.AllPictures)
                {
                    var imageStream = new MemoryStream(pictureData.Data);
                    image = imageStream.ToArray();
                }
            }
            return image;
        }
        private byte[] SetQRCode(byte[] fileData, byte[] qrCode)
        {
            string placeHolderForQR = "[QUID-QR-CODE]";

            using (var documentStream = new MemoryStream(fileData))
            {
                XWPFDocument document = new XWPFDocument(documentStream);

                foreach (var paragraph in document.Paragraphs)
                {
                    foreach (var run in paragraph.Runs)
                    {
                        if (run.ToString().Contains(placeHolderForQR))
                        {
                            run.SetText(run.Text.Replace(placeHolderForQR, string.Empty)); // Remove the placeholder

                            // Add the image to the run
                            using var imageStream = new MemoryStream(qrCode);
                            var pictureType = (int)NPOI.XWPF.UserModel.PictureType.PNG;
                            run.AddPicture(imageStream, pictureType, $"{placeHolderForQR}.png", 100 * 914400 / 96, 100 * 914400 / 96); // Size in EMUs
                        }
                    }
                }

                using (var outStream = new MemoryStream())
                {
                    document.Write(outStream);
                    return outStream.ToArray();
                }
            }
        }

        private byte[] ReplacePlaceholders(byte[] fileData, VacationLeaveOrder order)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "[QUID-ORDER-ID]", order.OrderId },
                { "[QUID-ORDER-DATE]", order.OrderDate  },
                { "[QUID-EMPLOYEE-NAME]", order.EmployeeName  },
                { "[QUID-EMPLOYEE-SURNAME]", order.EmployeeSurname  },
                { "[QUID-EMPLOYEE-POSITION]", order.EmployeePosition  },
                { "[QUID-VACATION-START]", order.VacationStart  },
                { "[QUID-VACATION-END]", order.VacationEnd  },
                { "[QUID-ADMIN-NAME]", order.AdminName  },
                { "[QUID-ADMIN-POSITION]", order.AdminPosition  }
            };

            using (var documentStream = new MemoryStream(fileData))
            {
                XWPFDocument document = new XWPFDocument(documentStream);

                foreach (var paragraph in document.Paragraphs)
                {
                    foreach (var run in paragraph.Runs)
                    {
                        foreach (var placeHolder in dictionary)
                        {
                            string text = run.ToString();
                            if (text.Contains(placeHolder.Key))
                            {
                                text = text.Replace(placeHolder.Key, placeHolder.Value);
                                run.SetText(text);
                            }
                        }
                    }
                }

                using (var outStream = new MemoryStream())
                {
                    document.Write(outStream);
                    return outStream.ToArray();
                }
            }
        }

        private async Task<byte[]> GenerateQRCode(string signature, string adminUser)
        {
            var qrCodeImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(adminUser + "\n" + signature)}&size=300x300";
            var response = await _httpClient.GetAsync(qrCodeImageUrl);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }

        private string CreateSignature(byte[] fileData,string adminName)
        {
            string allText = string.Empty;

            using (var documentStream = new MemoryStream(fileData))
            {
                XWPFDocument document = new XWPFDocument(documentStream);

                foreach (var paragraph in document.Paragraphs)
                {
                    allText += paragraph.Text;
                }

                allText = allText.Replace("[QUID-QR-CODE]", string.Empty);
            }

            allText += adminName;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(allText));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public class VacationLeaveOrder
        {
            public string OrderId { get; set; }
            public string OrderDate { get; set; }
            public string EmployeeName { get; set; }
            public string EmployeeSurname { get; set; }
            public string EmployeePosition { get; set; }
            public string VacationStart { get; set; }
            public string VacationEnd { get; set; }
            public string AdminName { get; set; }
            public string AdminPosition { get; set; }
            public string QrCode { get; set; }
        }
    }
}