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
    public class FeedbackService : BaseService, IFeedbackService
    {
        private readonly IFeedbackProductRepository _feedbackProductRepository;
        private readonly IMotobikeProductRepository _motobikeProductRepository;
        private readonly IFeedbackStaffRepository _feedbackStaffRepository;
        private readonly IStaffRepository _staffRepository;

        public FeedbackService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _feedbackProductRepository = unitOfWork.FeedbackProduct;
            _motobikeProductRepository = unitOfWork.MotobikeProduct;
            _feedbackStaffRepository = unitOfWork.FeedbackStaff;
            _staffRepository = unitOfWork.Staff;
        }

        public async Task<FeedbackProductViewModel> GetFeedbackProduct(Guid Id)
        {
            return await _feedbackProductRepository.GetMany(feedback => feedback.Id.Equals(Id))
                .ProjectTo<FeedbackProductViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy feedback");
        }

        public async Task<FeedbackStaffViewModel> GetFeedbackStaff(Guid Id)
        {
            return await _feedbackStaffRepository.GetMany(feedback => feedback.Id.Equals(Id))
                .ProjectTo<FeedbackStaffViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync() ?? throw new NotFoundException("Không tìm thấy feedback");
        }

        public async Task<FeedbackStaffViewModel> CreateFeedbackStaff(CreateFeedbackStaffModel model)
        {
            var staff = await _staffRepository.GetMany(staff => staff.AccountId.Equals(model.StaffId))
                .Include(staff => staff.FeedbackStaffs)
                .FirstOrDefaultAsync();
            if (staff == null) throw new NotFoundException("Không tìm thấy staff");
            if(staff.FeedbackStaffs.Any(feeback => feeback.CustomerId.Equals(model.CustomerId)))
            {
                throw new ConflictException("Mỗi customer chỉ được tạo một feedback");
            }
            var feedbackId = Guid.NewGuid();
            var feedback = new FeedbackStaff
            {
                Id = feedbackId,
                StaffId = model.StaffId,
                CustomerId = model.CustomerId,
                Title = model.Title,
                Content = model.Content
            };

            _feedbackStaffRepository.Add(feedback);
            return await _unitOfWork.SaveChanges() > 0 ? await GetFeedbackStaff(feedbackId) : null!;
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
            return result > 0 ? await GetFeedbackProduct(feedbackId) : null!;
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
            return result > 0 ? await GetFeedbackProduct(feedbackId) : null!;
        }
    }
}
