using ARTHS_Data;
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
    public class ConfigurationService : BaseService, IConfigurationService
    {
        private readonly IConfigurationRepository _configurationRepository;
        public ConfigurationService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _configurationRepository = unitOfWork.Configuration;
        }

        public async Task<ConfigurationViewModel> GetSetting()
        {

            var config = await _configurationRepository.GetAll().ProjectTo<ConfigurationViewModel>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
            config!.DailyOnlineBookings = await CalculateDailyOnlineBookings();
            return config;
        }


        public async Task<ConfigurationViewModel> UpdateSetting(UpdateConfigurationModel model)
        {
            if (model.TotalStaff == 0 || model.ServiceTime == 0 || model.WorkHours == 0)
            {
                throw new BadRequestException("Vui lòng nhập các giá trị total staff, service time, workHours khác 0");
            }
            var config = await _configurationRepository.GetAll().FirstOrDefaultAsync();
            if (config == null) { throw new BadRequestException(""); }

            config.TotalStaff = model.TotalStaff ?? config.TotalStaff;
            config.WorkHours = model.WorkHours ?? config.WorkHours;
            config.ServiceTime = model.ServiceTime ?? config.ServiceTime;
            config.NonBookingPercentage = model.NonBookingPercentage ?? config.NonBookingPercentage;
            config.ShippingMoney = model.ShippingMoney ?? config.ShippingMoney;

            _configurationRepository.Add(config);
            var result = await _unitOfWork.SaveChanges();
            return result > 0 ? await GetSetting() : null!;
        }

        public async Task<int> CalculateDailyOnlineBookings()
        {
            var config = await _configurationRepository.GetAll().FirstOrDefaultAsync();
            if (config == null) { throw new BadRequestException(""); }

            int motosPerStaff = config.WorkHours / config.ServiceTime;
            int totalMotosPerDay = motosPerStaff * config.TotalStaff;
            return totalMotosPerDay * (100 - config.NonBookingPercentage) / 100;
        }
    }
}
