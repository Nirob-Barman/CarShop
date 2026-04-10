using CarShop.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace CarShop.Infrastructure.Services
{
    public class DataProtectionConfigEncryptor : IConfigEncryptor
    {
        private readonly IDataProtector _protector;

        public DataProtectionConfigEncryptor(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("PaymentGateway.Config");
        }

        public string Encrypt(string plainText) => _protector.Protect(plainText);

        public string Decrypt(string cipherText)
        {
            try { return _protector.Unprotect(cipherText); }
            catch { return "{}"; }
        }
    }
}
