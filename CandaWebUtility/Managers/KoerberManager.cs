
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class KoerberLibReturnObject
{
    public bool Success { get; set; } = false;
    public string Message { get; set; } = "";

    public string SerializedAuthenticationTicket { get; set; } = "";

}

public class AuthOdataPayload
{
    public AuthOdataPayload(string userName, string password)
    {
        this.userLogOnName = userName;
        this.userPassword = password;
    }
    public string applicationId { get; set; } = "Warehouse Edge";
    public string tenant { get; set; } = "";
    public string userLogOnName { get; set; } = "";
    public string userPassword { get; set; } = ""; //plain text
    public string connectionType { get; set; } = "Integration";
}

public class KoerberLib
{

    public static async Task<KoerberLibReturnObject> LogOn(string userName, string password)
    {
        var retVal = new KoerberLibReturnObject();
        try
        {
            string odataURL = ConfigurationManager.ConnectionStrings["KoerberOdataURL"].ConnectionString;
            if (string.IsNullOrEmpty(odataURL))
            {
                retVal.Message = "There was a problem in the config file.  The Web.config is missing connection string: \"KoerberOdataURL\"";
                return retVal;
            }
            string jsonData = JsonConvert.SerializeObject(new AuthOdataPayload(userName, password));
                
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, odataURL + "/LogOn");
                request.Headers.Add("Accept", "application/json");

                request.Content = new StringContent(jsonData, null, "application/json");
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    retVal.Success = true;
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(jsonResponse);
                    string serializedTicket = data["SerializedAuthenticationTicket"]?.ToString();
                    retVal.SerializedAuthenticationTicket = serializedTicket;
                }
                else
                {
                    retVal.Success = false;
                    retVal.Message = "The login credentials were invalid.";
                }
            }
        }
        catch(Exception ex)
        {
            retVal.Success = false;
            retVal.Message = "Server error: " + ex.Message;
        }
        return retVal;
    }

   
}