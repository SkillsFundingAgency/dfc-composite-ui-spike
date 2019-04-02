using Ncs.Prototype.Common;
using Ncs.Prototype.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ncs.Prototype.Web.ApplicationManagement.Services
{
    public class ApplicationService
    {
        private readonly IStorage _storage;
        private readonly CosmosSettings _cosmosSettings;

        public ApplicationService(IStorage storage, CosmosSettings cosmosSettings)
        {
            _storage = storage;
            _cosmosSettings = cosmosSettings;
        }

        public async Task Register(ApplicationEntity applicationEntity)
        {
            applicationEntity.IsRegistered = true;
            applicationEntity.Registered = DateTime.Now;
            applicationEntity.IsOnline = true;
            applicationEntity.IsHealthy = true;
            await _storage.Add(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, applicationEntity);
            await _storage.Update(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, applicationEntity.Name, applicationEntity);
        }

        public async Task Unregister(string name)
        {
            var applicationModel = await Get(name);
            if (applicationModel != null)
            {
                applicationModel.Modified = DateTime.Now;
                applicationModel.IsRegistered = false;
                await _storage.Update(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, name, applicationModel);
            }
        }

        public async Task Enable(string name)
        {
            var applicationModel = await Get(name);
            if (applicationModel != null)
            {
                applicationModel.Modified = DateTime.Now;
                applicationModel.IsOnline = true;
                await _storage.Update(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, name, applicationModel);
            }
        }

        public async Task Disable(string name)
        {
            var applicationModel = await Get(name);
            if (applicationModel != null)
            {
                applicationModel.Modified = DateTime.Now;
                applicationModel.IsOnline = false;
                await _storage.Update(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, name, applicationModel);
            }
        }

        public async Task<List<ApplicationEntity>> GetAllApplications()
        {
            return await _storage.Search<ApplicationEntity>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, null);
        }

        public async Task<List<ApplicationEntity>> GetOnlineApplications()
        {
            return await _storage.Search<ApplicationEntity>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, x => x.IsOnline && x.IsRegistered);
        }

        public async Task<List<ApplicationEntity>> GetOfflineApplications()
        {
            return await _storage.Search<ApplicationEntity>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, x => !x.IsOnline && x.IsRegistered);
        }

        public async Task<List<ApplicationEntity>> GetRegisteredApplication()
        {
            return await _storage.Search<ApplicationEntity>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, x => x.IsRegistered);
        }

        private async Task<ApplicationEntity> Get(string name)
        {
            return await _storage.Get<ApplicationEntity>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, name);
        }
    }
}
