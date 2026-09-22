using CarShop.Application.DTOs.Payment;
using CarShop.Application.Features.PaymentGateway;
using CarShop.Domain.Entities;
using System.Linq.Expressions;

namespace CarShop.Application.Mappers
{
    public static class PaymentGatewayMapper
    {
        public static Expression<Func<PaymentGateway, PaymentGatewayDto>> ToDtoExpression =>
            g => new PaymentGatewayDto
            {
                Id = g.Id,
                Name = g.Name,
                GatewayFamily = g.GatewayFamily,
                Slug = g.Slug,
                Type = g.Type,
                LogoUrl = g.LogoUrl,
                IsActive = g.IsActive,
                IsSandbox = g.IsSandbox,
                SupportedCurrencies = g.SupportedCurrencies,
                SortOrder = g.SortOrder,
                CreatedAt = g.CreatedAt,
                UpdatedAt = g.UpdatedAt
            };

        public static PaymentGatewayDto ToDto(PaymentGateway g) => new()
        {
            Id                  = g.Id,
            Name                = g.Name,
            GatewayFamily       = string.IsNullOrWhiteSpace(g.GatewayFamily)
                ? GatewayConfigSchema.GetFamilyKey(g.Slug)
                : g.GatewayFamily,
            Slug                = g.Slug,
            Type                = g.Type,
            LogoUrl             = g.LogoUrl,
            IsActive            = g.IsActive,
            IsSandbox           = g.IsSandbox,
            SupportedCurrencies = g.SupportedCurrencies,
            SortOrder           = g.SortOrder,
            CreatedAt           = g.CreatedAt,
            UpdatedAt           = g.UpdatedAt
        };
    }
}
