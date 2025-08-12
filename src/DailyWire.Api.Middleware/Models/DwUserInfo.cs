using System.Text.Json.Serialization;

namespace DailyWire.Api.Middleware.Models;

public class DwUserInfo
{
    [JsonPropertyName("personID")]
    public string PersonId { get; set; } = string.Empty;
    
    [JsonPropertyName("recurlyAccountCode")]
    public string RecurlyAccountCode  { get; set; } = string.Empty;
    
    [JsonPropertyName("email")]
    public string Email  { get; set; } = string.Empty;
    
    [JsonPropertyName("firstName")]
    public string FirstName  { get; set; } = string.Empty;
    
    [JsonPropertyName("lastName")]
    public string LastName  { get; set; } = string.Empty;
    
    [JsonPropertyName("avatar")]
    public string Avatar { get; set; } = string.Empty;
    
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;
    
    [JsonPropertyName("accessLevel")]
    public string AccessLevel { get; set; } = string.Empty;
    
    [JsonPropertyName("accountCreatedAt")]
    public DateTime AccountCreatedAt { get; set; }
    
    [JsonPropertyName("planType")]
    public string PlanType { get; set; } = string.Empty;
    
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; set; } = string.Empty;
}