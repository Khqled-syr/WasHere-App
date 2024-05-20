using System.Net.Http;

namespace WasHere.Utils;

public class GetIPAddress
{
    
    public static async Task<string> GetPublicIpAddressAsync()
    {
        using var client = new HttpClient();
        try
        {
            // Use the Ipinfo.io API to fetch the public IP address
            return await client.GetStringAsync("https://ipinfo.io/ip");
        }
        catch (Exception ex)
        {
            // Handle exceptions, such as network errors
            return $"Error: {ex.Message}";
        }
    }
}