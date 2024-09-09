using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace HRMSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ImageController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("GenerateQRCode")]
        public async Task<IActionResult> GenerateQRCode()
        {
            var adminUserData = "Admin User Data Here"; // Example admin user data
            var qrCodeImageUrl = $"https://api.qrserver.com/v1/create-qr-code/?data={Uri.EscapeDataString(adminUserData)}&size=300x300";

            var response = await _httpClient.GetAsync(qrCodeImageUrl);
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return File(imageBytes, "image/png");
            }

            return StatusCode((int)response.StatusCode);
        }

        [HttpGet("GenerateBarcode")]
        public async Task<IActionResult> GenerateBarcode()
        {
            var digitalSignatureData = "Digital Signature Data Here"; // Example digital signature data
            var barcodeImageUrl = $"https://barcode.tec-it.com/barcode.ashx?data={Uri.EscapeDataString(digitalSignatureData)}&code=Code128";

            var response = await _httpClient.GetAsync(barcodeImageUrl);
            if (response.IsSuccessStatusCode)
            {
                var imageBytes = await response.Content.ReadAsByteArrayAsync();
                return File(imageBytes, "image/png");
            }

            return StatusCode((int)response.StatusCode);
        }
    }
}