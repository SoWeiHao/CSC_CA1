using Newtonsoft.Json;

public class UpdateSubscriptionItemRequest
{
    [JsonProperty("id")]
    public string Id { get; set; }


    [JsonProperty("priceid")]
    public string PriceId { get; set; }

}