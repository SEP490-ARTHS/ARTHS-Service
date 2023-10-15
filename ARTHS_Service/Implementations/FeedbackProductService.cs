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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTHS_Service.Implementations
{
    public class FeedbackProductService : BaseService, IFeedbackProductService
    {
        private readonly IFeedbackProductRepository _feedbackProductRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;

        public FeedbackProductService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _feedbackProductRepository = unitOfWork.FeedbackProduct;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
        }

        public async Task<FeedbackProductViewModel> GetFeedback(Guid Id)
        {
            return await _feedbackProductRepository.GetMany(feedback => feedback.Id.Equals(Id))
                .ProjectTo<FeedbackProductViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy feedback");
        }

        public async Task<FeedbackProductViewModel> CreateProductFeedback(Guid customerId, CreateFeedbackProductModel model)
        {
            var product = await _motobikeProductRepository.GetMany(product => product.Id.Equals(model.MotobikeProductId))
                .Include(product => product.FeedbackProducts).FirstOrDefaultAsync();
            if (product == null) throw new NotFoundException("Không tìm thấy product");
            if (product.FeedbackProducts.Any(feebback => feebback.CustomerId.Equals(customerId)))
            {
                throw new ConflictException("Mỗi customer chỉ được tạo một feedback");
            }

            var feedbackId = Guid.NewGuid();
            var feedback = new FeedbackProduct
            {
                Id = feedbackId,
                CustomerId = customerId,
                MotobikeProductId = model.MotobikeProductId,
                Rate = model.Rate,
                Content = model.Content,
            };

            _feedbackProductRepository.Add(feedback);

            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetFeedback(feedbackId) : null!;
        }

        public async Task<FeedbackProductViewModel> UpdateProductFeedback(Guid customerId, Guid feedbackId, UpdateFeedbackProductModel model)
        {
            var feedback = await _feedbackProductRepository.GetMany(feedback => feedback.Id.Equals(feedbackId)).FirstOrDefaultAsync();
            if (feedback == null) throw new NotFoundException("Không tìm thấy feedback");
            if (!feedback.CustomerId.Equals(customerId)) throw new ConflictException("Bạn không có quyền chỉnh sửa feedback này.");

            feedback.Rate = model.Rate ?? feedback.Rate;
            feedback.Content = model.Content ?? feedback.Content;
            feedback.UpdateAt = DateTime.UtcNow;

            _feedbackProductRepository.Update(feedback);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetFeedback(feedbackId) : null!;
        }
    }
}
