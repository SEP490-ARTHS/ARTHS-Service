using ARTHS_Data;
using ARTHS_Data.Entities;
using ARTHS_Data.Models.Requests.Filters;
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
    public class VehicleService : BaseService, IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _vehicleRepository = unitOfWork.Vehicle;
        }

        public async Task<VehicleViewModel> GetVehicle(Guid id)
        {
            return await _vehicleRepository.GetMany(vehicle => vehicle.Id.Equals(id))
                            .ProjectTo<VehicleViewModel>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync() ?? null!;

        }

        public async Task<List<VehicleViewModel>> GetVehicles(VehicleFilterModel filter)
        {
            var query = _vehicleRepository.GetAll();
            if (filter.VehicleName != null)
            {
                query = query.Where(vehicle => vehicle.VehicleName.Contains(filter.VehicleName));

            }

            return await query
                .ProjectTo<VehicleViewModel>(_mapper.ConfigurationProvider)
                .OrderBy(vehicle => vehicle.VehicleName)
                .ToListAsync();
        }

        public async Task<VehicleViewModel> CreateVehicle(CreateVehicleRequest request)
        {
            try
            {

                if (_vehicleRepository.Any(v => v.VehicleName.Equals(request.VehicleName)))
                {
                    throw new ConflictException("Hãng xe đã tồn tại!");
                }

                var vehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    VehicleName = request.VehicleName,
                };

                _vehicleRepository.Add(vehicle);

                var result = await _unitOfWork.SaveChanges().ConfigureAwait(false);

                if (result > 0)
                {
                    return await GetVehicle(vehicle.Id);
                }

                throw new Exception("Tạo thất bại!");
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented.
                throw new Exception("An error occurred while creating the vehicle.", ex);
            }
        }


        public async Task<VehicleViewModel> UpdateVehicle(Guid Id, UpdateVehicleRequest request)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetMany(v => v.Id.Equals(Id)).FirstOrDefaultAsync();

                if (vehicle == null)
                {
                    throw new NotFoundException("không tìm thấy");
                }

                if (_vehicleRepository.Any(v => v.VehicleName.Equals(request.VehicleName)))
                {
                    throw new ConflictException("Tên phương tiện đã tồn tại");
                }

                vehicle.VehicleName = request.VehicleName;
                vehicle.VehicleName = request.VehicleName;

                _vehicleRepository.Update(vehicle);

                var result = await _unitOfWork.SaveChanges();

                if (result > 0)
                {
                    return await GetVehicle(Id);
                }

                throw new Exception("thay đổi thất bại");
            }
            catch (Exception ex)
            {
                // Log the exception here if logging is implemented.
                throw new Exception("An error occurred while updating the vehicle.", ex);
            }
        }


        public async Task<VehicleViewModel> DeleteVehicle(Guid Id)
        {
            var vehicle = await _vehicleRepository.GetMany(vehicle => vehicle.Id.Equals(Id)).FirstOrDefaultAsync();
            if (vehicle != null)
            {
                _vehicleRepository.Remove(vehicle);

                var result = await _unitOfWork.SaveChanges();
                if (result > 0)
                {
                    return new VehicleViewModel { };
                }
                throw new Exception("xóa không thành công");
            }
            throw new NotFoundException("không tìm thấy");
        }
    }
}
