using eCommerceApp.Application.DTOs.Cart;

namespace eCommerceApp.Application.Services.Interface.Cart
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<GetPaymentMethod>> GetPaymentMethods();
    }
}
