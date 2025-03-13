using System.Text;
using System.Text.Json;
using EuroMotors.Application.Abstractions.Delivery;
using EuroMotors.Application.Delivery.GetWarehouse;
using Microsoft.Extensions.Options;

namespace EuroMotors.Infrastructure.Delivery;

internal sealed class DeliveryService(IOptions<DeliveryOptions> deliveryOptions, HttpClient httpClient) : IDeliveryService
{
    private readonly DeliveryOptions _options = deliveryOptions.Value;

    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    public async Task<List<Warehouse>> GetWarehousesAsync(string cityName, string query)
    {
        var requestData = new
        {
            apiKey = _options.ApiKey,
            modelName = "AddressGeneral",
            calledMethod = "getWarehouses",
            methodProperties = new { CityName = cityName, FindByString = query }
        };

        using var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(_options.ApiUrl, content);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        WarehousesResponse<Warehouse>? result = JsonSerializer.Deserialize<WarehousesResponse<Warehouse>>(responseBody, _jsonOptions);

        return result?.Data ?? [];
    }

}
