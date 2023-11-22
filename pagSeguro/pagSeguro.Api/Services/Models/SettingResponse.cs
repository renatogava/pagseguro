namespace pagSeguro.Api.Services.Models
{
    public class SettingResponse
    {
        public bool success { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int StoreId { get; set; }
    }
}
