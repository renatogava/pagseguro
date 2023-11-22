using pagSeguro.Api.Entities;
using pagSeguro.Api.Services.Models;
using System;

namespace pagSeguro.Api.Services
{
    public interface ISettingService
    {
        SettingResponse GetById(SettingRequest request);
        SettingResponse GetByName(SettingRequest request);
        bool Add(SettingRequest request);
        bool Delete(Setting request);
        Task<SettingResponse> Update(SettingRequest request);
        string GetByName(string value);
        List<SettingResponse> AddList(List<SettingRequest> request);
        List<Setting> GetAll();
    }
}
