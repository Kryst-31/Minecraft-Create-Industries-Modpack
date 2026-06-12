using System.Net.Http;
using System.Threading.Tasks;

namespace installer.Services;

public class DownloadService
{
    private readonly HttpClient _http = new HttpClient();

    public async Task<byte[]> DownloadAsync(string url)
    {
        return await _http.GetByteArrayAsync(url);
    }
}
