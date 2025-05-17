
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tensae.Generic.Repository;
using TenVids.Data.Access.Data;
using TenVids.Data.Access.IRepo;
using TenVids.Models;
using TenVids.Models.DTOs;
    
using TenVids.Utilities;

namespace TenVids.Repository
{
    public class VideoViewRepository : Repository<VideoViews>, IVideoViewRepository
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly TenVidsApplicationContext _context;

        public VideoViewRepository(TenVidsApplicationContext context, IConfiguration configuration)
            : base(context)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.ip2location.io")
            };
        }

        public async Task HandleVideoViewAsync(string userId, int videoId, string ipAddress)
        {
            var existingView = await _context.VideoViews.Where(x=>x.AppUserId == userId && x.VideoId == videoId)
                .OrderByDescending(x => x.LastVisit).FirstOrDefaultAsync();
            if (existingView == null)
            {
                await AddVideoViewAsync(userId, videoId, ipAddress);
            }
            else
            {
                DateTime now=DateTime.UtcNow;
                DateTime onehourafterlastvisit= existingView.LastVisit.AddHours(1);
                if (now > onehourafterlastvisit && now.Date==existingView.LastVisit.Date)
                {
                    existingView.LastVisit=DateTime.UtcNow;
                    existingView.NumberOfVisit++;
                }
                if(existingView.LastVisit.Date<now.Date)
                {
                    await AddVideoViewAsync(userId, videoId, ipAddress);
                }
            }
        }

        private async Task AddVideoViewAsync(string userId, int videoId, string ipAddress)
        {
            var geoData = await GetIP2LocationResultAsync(ipAddress);

            var newView = new VideoViews
            {
                VideoId = videoId,
                AppUserId = userId,
                IpAddress = ipAddress,
                Country = geoData.Country_Name ?? "Unknown",
                City = geoData.City_Name ?? "Unknown",
               PostalCode = geoData.Zip_Code?? "Unknown",
                Is_Proxy = geoData.Is_Proxy 
            };

          Add(newView);
        }

        private async Task<IP2LocationResultDto> GetIP2LocationResultAsync(string ipAddress)
        {
            try
            {
                if (SD.LocalIpAddresses.Contains(ipAddress))
                {
                    return new IP2LocationResultDto();
                }
                else
                {
                    var result = await _httpClient.GetFromJsonAsync<IP2LocationResultDto>($"?Key={_configuration["IP2LocationAPIKey"]}&ip={ipAddress}&format=json");
                    return result;
                }

           
            }
            catch
            {
                return new IP2LocationResultDto();
            }
        }
    }
}
