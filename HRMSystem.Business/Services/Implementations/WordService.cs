using HRMSystem.Business.Services.Interfaces;
using NPOI.XWPF.UserModel;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using HRMSystem.Business.Settings;
using Microsoft.Extensions.Options;

namespace HRMSystem.Business.Services.Implementations;

public class WordService : IWordService
{
    private readonly ISignatureService _signatureService;
    private readonly FileSettings _fileSettings;

    public WordService(ISignatureService signatureService,IOptions<FileSettings> fileSettings)
    {
        _signatureService = signatureService;
        _fileSettings = fileSettings.Value;
    }

    public async Task<byte[]> GenerateDocument()
    {
        string inputFile = _fileSettings.Path;
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
            AdminPosition = "Office Manager"
        };

        // Read DOCX file into a byte array
        byte[] fileData = await File.ReadAllBytesAsync(inputFile);

        // Replace placeholders in DOCX
        fileData = ReplacePlaceholders(fileData, order);

        // Create digital signature using SignatureService
        string signature = _signatureService.CreateSignature(fileData, order.AdminName);

        // Generate QR code using SignatureService
        byte[] qrCode = await _signatureService.GenerateQRCode(signature, order.AdminName);

        // Set QR code in the document
        fileData = SetQRCode(fileData, qrCode);

        return fileData;
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
            var document = new XWPFDocument(documentStream);

            foreach (var paragraph in document.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    foreach (var placeholder in dictionary)
                    {
                        string text = run.ToString();
                        if (text.Contains(placeholder.Key))
                        {
                            text = text.Replace(placeholder.Key, placeholder.Value);
                            run.SetText(text, 0);
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

    private byte[] SetQRCode(byte[] fileData, byte[] qrCode)
    {
        string placeHolderForQR = "[QUID-QR-CODE]";

        using (var documentStream = new MemoryStream(fileData))
        {
            var document = new XWPFDocument(documentStream);

            foreach (var paragraph in document.Paragraphs)
            {
                foreach (var run in paragraph.Runs)
                {
                    if (run.ToString().Contains(placeHolderForQR))
                    {
                        run.SetText(run.Text.Replace(placeHolderForQR, string.Empty), 0); // Remove the placeholder

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

    public async Task<bool> VerifyDocument(byte[] fileData)
    {
        var qrImage = ExtractImage(fileData);
        var result = await _signatureService.DecodeQRCode(qrImage);
        if(result == null)
        {
            return false;
        }

        string signature = _signatureService.CreateSignature(fileData, result.AdminName);

        return signature == result.DigitalSignature;
    }

    private byte[] ExtractImage(byte[] fileData)
    {
        byte[] image = null;
        using (var documentStream = new MemoryStream(fileData))
        {
            var document = new XWPFDocument(documentStream);

            foreach (var pictureData in document.AllPictures)
            {
                var imageStream = new MemoryStream(pictureData.Data);
                image = imageStream.ToArray();
            }
        }
        return image;
    }

    private class VacationLeaveOrder
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
    }
}