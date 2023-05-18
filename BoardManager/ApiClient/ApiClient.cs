using RestSharp;

namespace BoardManager.ApiClient;

public class ApiClient : IApiClient
{
    public async Task<RestResponse> PostBoardUpdate(string input)
    {
        Console.WriteLine($"Sending fen string to GUI: {input}");

        var client = new RestClient("http://host.docker.internal:3002/");
        var request = new RestRequest("api/updateBoard", Method.Post);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddJsonBody(new { input });
        var response = await client.ExecutePostAsync(request);
        return response;

    }
}
