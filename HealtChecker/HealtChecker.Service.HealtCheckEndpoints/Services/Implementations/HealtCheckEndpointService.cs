using HealtChecker.Service.HealtCheckEndpoints.Data.Entities;
using HealtChecker.Service.HealtCheckEndpoints.Data.Interfaces;
using HealtChecker.Service.HealtCheckEndpoints.Services.Interfaces;
using HealtChecker.Shared.Exceptions;
using HealtChecker.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealtChecker.Service.HealtCheckEndpoints.Services.Implementations
{
    public class HealtCheckEndpointService : IHealtCheckEndpointService
    {
        private IHealtCheckDbContext _healtCheckDbContext { get; init; }
        public HealtCheckEndpointService(IHealtCheckDbContext healtCheckDbContext)
        {
            _healtCheckDbContext = healtCheckDbContext;
        }
        public async Task<ServiceResult<Guid>> CreateHealtCheckEndpoint(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            HealtCheckEnpoint insertedModel = new HealtCheckEnpoint()
            {
                ConnectedUserId = healtCheckEndpointModel.ConnectedUserId,
                HealtCheckUrl = healtCheckEndpointModel.HealtCheckUrl,
                Name = healtCheckEndpointModel.Name,
                IntervalSeconds = healtCheckEndpointModel.IntervalSeconds,
                NextExecutionTime = DateTime.UtcNow.AddSeconds(healtCheckEndpointModel.IntervalSeconds),
                NotificationEmailAddress = healtCheckEndpointModel.NotificationEmailAddress,
                DownTimeAlertInterval = healtCheckEndpointModel.DownTimeAlertInterval
            };
            await _healtCheckDbContext.HealtCheckEnpoints.AddAsync(insertedModel);

            await _healtCheckDbContext.SaveChangesAsync(healtCheckEndpointModel.OperatedUserId);

            ServiceResult<Guid> result = new ServiceResult<Guid>();
            result.Data = insertedModel.Id;

            return result;
        }

        public async Task<ServiceResult<bool>> DeleteHealtCheckEndpoint(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            HealtCheckEnpoint storedEndpoint = await _healtCheckDbContext.HealtCheckEnpoints.FindAsync(healtCheckEndpointModel.Id);
            if (storedEndpoint == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    ServiceName = nameof(HealtCheckEndpointService),
                    MethodName = nameof(DeleteHealtCheckEndpoint)
                };
            }

            if (storedEndpoint.ConnectedUserId != healtCheckEndpointModel.OperatedUserId)
            {
                throw new SecurityException()
                {
                    MethodName = nameof(DeleteHealtCheckEndpoint),
                    ServiceName = nameof(HealtCheckEndpointService),
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    InvalidOperatorUserId = healtCheckEndpointModel.OperatedUserId
                };
            }

            _healtCheckDbContext.HealtCheckEnpoints.Remove(storedEndpoint);

            int affectedRows = _healtCheckDbContext.SaveChanges(healtCheckEndpointModel.OperatedUserId);

            ServiceResult<bool> result = new ServiceResult<bool>();
            result.Data = affectedRows > 0;

            return result;
        }

        public async Task<ServiceResult<HealtCheckEndpointModel>> GetHealtCheckEnpointById(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            HealtCheckEnpoint storedEndpoint = await _healtCheckDbContext.HealtCheckEnpoints
                .FindAsync(healtCheckEndpointModel.Id);
            if (storedEndpoint == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    ServiceName = nameof(HealtCheckEndpointService),
                    MethodName = nameof(GetHealtCheckEnpointById)
                };
            }

            if (storedEndpoint.ConnectedUserId != healtCheckEndpointModel.OperatedUserId)
            {
                throw new SecurityException()
                {
                    MethodName = nameof(GetHealtCheckEnpointById),
                    ServiceName = nameof(HealtCheckEndpointService),
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    InvalidOperatorUserId = healtCheckEndpointModel.OperatedUserId
                };
            }

            ServiceResult<HealtCheckEndpointModel> result = new ServiceResult<HealtCheckEndpointModel>();
            result.Data = EntityToModel(storedEndpoint);

            return result;

        }

        public async Task<ServiceResult<List<HealtCheckEndpointModel>>> GetHealtCheckEnpointsByUserId(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            List<HealtCheckEnpoint> healtCheckEndpoints = await _healtCheckDbContext.HealtCheckEnpoints
                .Where(x => x.ConnectedUserId == healtCheckEndpointModel.ConnectedUserId).ToListAsync();

            ServiceResult<List<HealtCheckEndpointModel>> result = new ServiceResult<List<HealtCheckEndpointModel>>();
            result.Data = healtCheckEndpoints.Select(x => EntityToModel(x)).ToList();

            return result;
        }

        public async Task<ServiceResult<bool>> UpdateHealtCheckEndpoint(HealtCheckEndpointModel healtCheckEndpointModel)
        {
            HealtCheckEnpoint storedEndpoint = await _healtCheckDbContext.HealtCheckEnpoints
                .FindAsync(healtCheckEndpointModel.Id);
            if (storedEndpoint == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    ServiceName = nameof(HealtCheckEndpointService),
                    MethodName = nameof(UpdateHealtCheckEndpoint)
                };
            }

            if (storedEndpoint.ConnectedUserId != healtCheckEndpointModel.OperatedUserId)
            {
                throw new SecurityException()
                {
                    MethodName = nameof(UpdateHealtCheckEndpoint),
                    ServiceName = nameof(HealtCheckEndpointService),
                    EntityId = healtCheckEndpointModel.Id,
                    EntityName = nameof(HealtCheckEnpoint),
                    InvalidOperatorUserId = healtCheckEndpointModel.OperatedUserId
                };
            }

            int differenceInterval = healtCheckEndpointModel.IntervalSeconds - storedEndpoint.IntervalSeconds;

            storedEndpoint.HealtCheckUrl = healtCheckEndpointModel.HealtCheckUrl;
            storedEndpoint.Name = healtCheckEndpointModel.Name;
            storedEndpoint.IntervalSeconds = healtCheckEndpointModel.IntervalSeconds;
            storedEndpoint.NextExecutionTime = storedEndpoint.NextExecutionTime.AddSeconds(differenceInterval);
            storedEndpoint.NotificationEmailAddress = healtCheckEndpointModel.NotificationEmailAddress;
            storedEndpoint.DownTimeAlertInterval = healtCheckEndpointModel.DownTimeAlertInterval;

            int affectedRows = _healtCheckDbContext.SaveChanges(healtCheckEndpointModel.OperatedUserId);

            ServiceResult<bool> result = new ServiceResult<bool>();
            result.Data = affectedRows > 0;

            return result;
        }

        public async Task<ServiceResult<List<HealtCheckEndpointModel>>> GetExecutableHealtCheckEndpoints(int recordCount)
        {
            List<HealtCheckEnpoint> healtCheckEndpoints = await _healtCheckDbContext.HealtCheckEnpoints
                .Where(x => x.NextExecutionTime <= DateTime.UtcNow).Take(recordCount).ToListAsync();

            foreach (var healtCheckEndpoint in healtCheckEndpoints)
            {
                healtCheckEndpoint.NextExecutionTime = DateTime.UtcNow.AddSeconds(healtCheckEndpoint.IntervalSeconds);
            }

            await _healtCheckDbContext.SaveChangesAsync(Guid.Empty);

            ServiceResult<List<HealtCheckEndpointModel>> result = new ServiceResult<List<HealtCheckEndpointModel>>();
            result.Data = healtCheckEndpoints.Select(x => EntityToModel(x)).ToList();

            return result;
        }


        private HealtCheckEndpointModel EntityToModel(HealtCheckEnpoint storedEndpoint)
        {
            return new HealtCheckEndpointModel()
            {
                ConnectedUserId = storedEndpoint.ConnectedUserId,
                HealtCheckUrl = storedEndpoint.HealtCheckUrl,
                Id = storedEndpoint.Id,
                Name = storedEndpoint.Name,
                OperatedUserId = storedEndpoint.UpdatedUserId ?? storedEndpoint.CreatedUserId,
                IntervalSeconds = storedEndpoint.IntervalSeconds,
                NotificationEmailAddress = storedEndpoint.NotificationEmailAddress,
                DownTimeAlertInterval = storedEndpoint.DownTimeAlertInterval
            };
        }

    }
}
