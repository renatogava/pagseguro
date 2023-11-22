using Microsoft.Extensions.Caching.Memory;
using pagSeguro.Api.Entities;
using pagSeguro.Api.Helpers;
using pagSeguro.Api.Services.Models;

namespace pagSeguro.Api.Services
{
    public class SettingService : ISettingService
    {
        private DataContext _context;
        private readonly IMemoryCache _cache;

        public SettingService(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public bool Add(SettingRequest request)
        {
            var result = false;

            var setting = _context.Settings.FirstOrDefault(c => c.Name == request.Name);

            if (setting == null)
            {
                setting = new Setting();
                setting.Id = request.Id;
                setting.Name = request.Name;
                setting.Value = request.Value;

                _context.Settings.Add(setting);

                _context.SaveChanges();

                result = true;
            }

            return result;
        }

        public List<SettingResponse> AddList(List<SettingRequest> request)
        {
            var response = new List<SettingResponse>();

            if (request != null && request.Count > 0)
            {
                foreach (var item in request)
                {
                    var setting = _context.Settings.FirstOrDefault(c => c.Name == item.Name);
                    if (setting == null)
                    {
                        setting = new Setting();
                        setting.Id = item.Id;
                        setting.Name = item.Name;
                        setting.Value = item.Value;

                        response.Add(new SettingResponse
                        {
                            Id = setting.Id,
                            Name = setting.Name,
                            Value = setting.Value
                        });

                        _context.Settings.Add(setting);
                        _context.SaveChanges();
                    }
                }
            }

            return response;

        }

        public bool Delete(Setting request)
        {
            var result = false;
            var setting = _context.Settings.FirstOrDefault(c => c.Id == request.Id && c.Name == request.Name);

            if (setting != null)
            {
                _context.Settings.Remove(setting);
                _context.SaveChanges();
                result = true;
            }

            return result;

        }

        public SettingResponse GetById(SettingRequest request)
        {
            var response = new SettingResponse();
            var setting = _context.Settings.FirstOrDefault(c => c.Id == request.Id);

            if (setting != null)
            {
                response.success = true;
                response.Id = setting.Id;
                response.Name = setting.Name;
                response.Value = setting.Value;
            }

            return response;
        }

        public SettingResponse GetByName(SettingRequest request)
        {
            var response = new SettingResponse();
            var setting = _context.Settings.FirstOrDefault(c => c.Name == request.Name);

            if (setting != null)
            {
                response.success = true;
                response.Id = setting.Id;
                response.Name = setting.Name;
                response.Value = setting.Value;
            }

            return response;
        }

        public string GetByName(string name)
        {
            var cacheEntry = _cache.GetOrCreate(name, entry =>
            {
                var setting = _context.Settings.FirstOrDefault(c => c.Name == name);

                if (setting != null)
                {
                    return setting.Value;
                }
                else
                {
                    return string.Empty;
                }
            });

            return cacheEntry;
        }

        public async Task<SettingResponse> Update(SettingRequest request)
        {
            var response = new SettingResponse();
            var setting = new Setting();

            if (request.Id > 0)
            {
                setting = _context.Settings.FirstOrDefault(c => c.Id == request.Id);
            }
            else if (!string.IsNullOrEmpty(request.Name))
            {
                setting = _context.Settings.FirstOrDefault(c => c.Name == request.Name);
            }

            if (setting != null && setting.Id > 0)
            {

                setting.Name = request.Name;
                setting.Value = request.Value;

                _context.Settings.Update(setting);

                await _context.SaveChangesAsync();

                response.success = true;
                response.Id = setting.Id;
                response.Name = setting.Name;
                response.Value = setting.Value;
            }

            return response;
        }

        public List<Setting> GetAll()
        {
            return _context.Settings.ToList();
        }
    }
}
