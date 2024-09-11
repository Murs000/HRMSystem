using HRMSystem.Business.Services.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NPOI.XWPF.UserModel;
using Newtonsoft.Json.Linq;
using HRMSystem.Business.DTOs;

namespace HRMSystem.Business.Services.Implementations
{
    public partial class SignatureService : ISignatureService
    {
        private readonly HttpClient _httpClient;

        public SignatureService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QRResponce> DecodeQRCode(byte[] qrCodeBytes)
        {
            // API endpoint for goqr.me service
            string url = "https://api.qrserver.com/v1/read-qr-code/";

            // Convert the byte array to base64
            var base64Image = Convert.ToBase64String(qrCodeBytes);

            // Create form data with the image
            var content = new MultipartFormDataContent();
            var imageContent = new ByteArrayContent(qrCodeBytes);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
            content.Add(imageContent, "file", "qrcode.png");

            // Send POST request
            var response = await _httpClient.PostAsync(url, content);
            

            // Get the result
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return DeserializeResponse(jsonResponse);
            }
            else
            {
                return null;
            }
        }

        private QRResponce DeserializeResponse(string apiResponse)
        {
            // Parse the JSON response
            JArray parsedResponse = JArray.Parse(apiResponse);

            // Extract the data field
            string data = parsedResponse[0]["symbol"][0]["data"]?.ToString();

            // Ensure data is valid before proceeding
            if (string.IsNullOrEmpty(data)) return null;

            // Split the data into "Admin" and the digital signature
            string[] parts = data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string admin = parts[0]; // Admin
            string digitalSignature = parts.Length > 1 ? parts[1] : string.Empty; // Digital signature

            return new QRResponce(admin, digitalSignature);
        }

        public async Task<byte[]> GenerateQRCode(string signature, string adminUser)
        {
            var qrCodeImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(adminUser + "\n" + signature)}&size=300x300";
            var response = await _httpClient.GetAsync(qrCodeImageUrl);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsByteArrayAsync() : null;
        }

        public string CreateSignature(byte[] fileData, string adminName)
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
    }
}