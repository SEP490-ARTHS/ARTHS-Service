using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Post;
using ARTHS_Data.Models.Requests.Put;
using ARTHS_Data.Models.Views;
using ARTHS_Data.Repositories.Interfaces;
using ARTHS_Service.Interfaces;
using ARTHS_Utility.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ARTHS_Service.Implementations
{
    public class CartService : BaseService, ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _itemRepository;
        private readonly IMotobikeProductRepository _productRepository;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _cartRepository = unitOfWork.Cart;
            _itemRepository = unitOfWork.CartItem;
            _productRepository = unitOfWork.MotobikeProduct;
        }

        

        public async Task<CartViewModel> GetCartByCustomerId(Guid customerId)
        {
            return await _cartRepository.GetMany(cart => cart.CustomerId.Equals(customerId))
                .ProjectTo<CartViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy cart");
        }

        public async Task<CartViewModel> AddToCart(Guid customerId, List<CreateCartModel> list)
        {
            

            var cart = await _cartRepository.GetMany(cart => cart.CustomerId.Equals(customerId)).FirstOrDefaultAsync();
            if(cart == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin của cart");
            }

            var result = 0;
            using (var transaction = _unitOfWork.Transaction())
            {
                try
                {
                    foreach(var item in list)
                    {
                        var motobikeProduct = await _productRepository.GetMany(product => product.Id.Equals(item.ProductId)).FirstOrDefaultAsync();
                        if (motobikeProduct == null)
                        {
                            throw new NotFoundException("Không tìm thấy product");
                        }
                        await HandleAddCartItem(cart.Id, item);
                    }
                    result = await _unitOfWork.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            };
            return result > 0 ? await GetCartByCustomerId(customerId) : null!;

        }


        public async Task<CartViewModel> UpdateCart(Guid cartId, UpdateCartModel model)
        {
            var cart = await _cartRepository.GetMany(cart => cart.Id.Equals(cartId))
                .Include(item => item.CartItems)
                .FirstOrDefaultAsync();
            if(cart == null)
            {
                throw new NotFoundException("Không tìm thấy cart");
            }

            foreach(var oldItem in cart.CartItems.ToList())
            {
                bool itemExistInModel = false;
                foreach(var newItem in model.CartItems)
                {
                    if (oldItem.MotobikeProductId.Equals(newItem.ProductId))
                    {
                        if(newItem.Quantity > 10)
                        {
                            throw new BadRequestException("Một sản phẩm chỉ được tối đa 10 trong giỏ hàng");
                        }
                        oldItem.Quantity = newItem.Quantity;
                        itemExistInModel = true;
                        break;
                    }
                }
                if(!itemExistInModel)
                {
                    cart.CartItems.Remove(oldItem);
                }
            }
            _cartRepository.Update(cart);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetCartByCustomerId(cart.CustomerId) : null!;
        }

        

        private async Task HandleAddCartItem(Guid cartId, CreateCartModel model)
        {
            var existItemInCart = await _itemRepository.GetMany(item => item.CartId.Equals(cartId) 
                                                                && item.MotobikeProductId.Equals(model.ProductId))
                                                        .FirstOrDefaultAsync();
            if(existItemInCart != null)
            {
                if(existItemInCart.Quantity + model.Quantity > 10)
                {
                    throw new BadRequestException("Một sản phẩm chỉ được tối đa 10 trong giỏ hàng");
                }
                existItemInCart.Quantity += model.Quantity;
                _itemRepository.Update(existItemInCart);
            }
            else
            {
                if(model.Quantity > 10)
                {
                    throw new BadRequestException("Một sản phẩm chỉ được tối đa 10 trong giỏ hàng");
                }
                var cartItem = new CartItem
                {
                    CartId = cartId,
                    MotobikeProductId = model.ProductId,
                    Quantity = model.Quantity
                };
                _itemRepository.Add(cartItem);
            }
        }
    }
}
