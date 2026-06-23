using AutoMapper;
using FurniZone.BLL.ModelVM.Auth;
using FurniZone.BLL.ModelVM.Cart;
using FurniZone.BLL.ModelVM.Category;
using FurniZone.BLL.ModelVM.Order;
using FurniZone.BLL.ModelVM.Product;
using FurniZone.BLL.ModelVM.Review;
using FurniZone.BLL.ModelVM.User;
using FurniZone.BLL.ModelVM.Wishlist;
using FurniZone.DAL.Entities;

namespace FurniZone.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserResponse>();
            CreateMap<User, AuthResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            // Category mappings
            CreateMap<Category, CategoryResponse>();
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>();

            // Product mappings
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<Product, ProductDetailResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<CreateProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>();

            // Cart mappings
            CreateMap<CartItem, CartItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.Product.Price));

            CreateMap<Cart, CartResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartItems))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => 
                    src.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => 
                    src.CartItems.Sum(ci => ci.Quantity)));

            // Wishlist mappings
            CreateMap<WishlistItem, WishlistItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<Wishlist, WishlistResponse>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.WishlistItems))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.WishlistItems.Count));

            // Review mappings
            CreateMap<Review, ReviewResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<CreateReviewRequest, Review>();
            CreateMap<UpdateReviewRequest, Review>();

            // Order mappings
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

            CreateMap<Payment, PaymentResponse>();

            CreateMap<Order, OrderResponse>();
        }
    }
}
