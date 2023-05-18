using RestSharp;

namespace BoardManager.ApiClient;

public interface IApiClient
{
    public Task<RestResponse> PostBoardUpdate(string boardFenState);
}
