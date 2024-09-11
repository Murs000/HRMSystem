namespace HRMSystem.Business.Services.Interfaces
{
    public interface IWordService
    {
        public Task<byte[]> GenerateDocument();
        public Task<bool> VerifyDocument(byte[] fileData);
    }
}