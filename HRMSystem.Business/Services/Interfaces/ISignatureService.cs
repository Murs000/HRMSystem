

using HRMSystem.Business.DTOs;

namespace HRMSystem.Business.Services.Interfaces
{
    public interface ISignatureService
    {
        public Task<QRResponce> DecodeQRCode(byte[] qrCodeBytes);
        public Task<byte[]> GenerateQRCode(string signature, string adminUser);
        public string CreateSignature(byte[] fileData,string adminName);
    }
}