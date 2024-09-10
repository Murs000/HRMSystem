using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using NPOI.XWPF.UserModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController(HttpClient httpClient) : ControllerBase
    {
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
            
            // Create digital signature
            string signature = CreateSignature(order);

            // Generate QR code
            var qrCode = await GenerateQRCode($"Admin Data; Signature: {signature}");

            // Replace placeholders in DOCX
            var fileData = ReplacePlaceholders(inputFile, order, qrCode);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            var fileName = "UpdatedVacationOrder.docx";

            return File(fileData, contentType, fileName);
        }

        private byte[] ReplacePlaceholders(string inputFile, VacationLeaveOrder order, byte[] qrCode)
        {
            var dictionary = new Dictionary<string, string>
            {
                { "QUID-ORDER-ID", order.OrderId },
                { "QUID-ORDER-DATE", order.OrderDate  },
                { "QUID-EMPLOYEE-NAME", order.EmployeeName  },
                { "QUID-EMPLOYEE-SURNAME", order.EmployeeSurname  },
                { "QUID-EMPLOYEE-POSITION", order.EmployeePosition  },
                { "QUID-VACATION-START", order.VacationStart  },
                { "QUID-VACATION-END", order.VacationEnd  },
                { "QUID-ADMIN-NAME", order.AdminName  },
                { "QUID-ADMIN-POSITION", order.AdminPosition  }
            };
            string placeHolderForQR = "[QUID-QR-CODE]";
            byte[] fileData;
            using (FileStream stream = new FileStream(inputFile, FileMode.Open, FileAccess.ReadWrite))
            {
                XWPFDocument document = new XWPFDocument(stream);

                foreach (var paragraph in document.Paragraphs)
                {
                    foreach (var run in paragraph.Runs)
                    {
                        foreach(var placeHolder in dictionary)
                        {
                            string text = run.ToString();
                            if (text.Contains(placeHolder.Key))
                            {
                                text = text.Replace(placeHolder.Key, placeHolder.Value);
                                run.SetText(text);
                            }
                            if (run.ToString().Contains(placeHolderForQR))
                            {
                                run.SetText(run.Text.Replace(placeHolderForQR, string.Empty)); // Remove the placeholder

                                // Add the image to the run
                                using var imageStream = new MemoryStream(qrCode);
                                var pictureType = (int)NPOI.XWPF.UserModel.PictureType.PNG;
                                var picture = run.AddPicture(imageStream, pictureType, $"{placeHolderForQR}.png", 100 * 914400 / 96, 100 * 914400 / 96); // Size in EMUs
                            }
                        }
                    }
                }

                using (MemoryStream outStream = new MemoryStream())
                {
                    document.Write(outStream);
                    outStream.Position = 0;
                    fileData = outStream.ToArray();
                }
            }
            return fileData;
        }

        private async Task<byte[]> GenerateQRCode(string signature)
        {
            var adminUserData = "Admin User Data Here"; // Example admin user data
            var qrCodeImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(adminUserData)}&size=300x300";
            var response = await httpClient.GetAsync(qrCodeImageUrl);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }

        private string CreateSignature(VacationLeaveOrder order)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes("ahha"));
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