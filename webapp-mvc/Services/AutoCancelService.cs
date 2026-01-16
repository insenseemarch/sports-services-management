using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using webapp_mvc.Models; // DatabaseHelper namespace

namespace webapp_mvc.Services
{
    public class AutoCancelService : BackgroundService
    {
        private readonly ILogger<AutoCancelService> _logger;
        private readonly DatabaseHelper _db;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        public AutoCancelService(ILogger<AutoCancelService> logger, DatabaseHelper db)
        {
            _logger = logger;
            _db = db;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Auto Cancel Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Auto Cancel Service checking for expired bookings");
                    // Execute SP
                    // Note: DatabaseHelper is usually scoped/transient, but here we inject it. 
                    // If errors occur due to DI scope, we might need IServiceScopeFactory.
                    // Assuming DatabaseHelper is Singleton or Transient safe.
                    
                    // Since ExecuteNonQuery is synchronous, we wrap it
                    // TODO: Create sp_TuDongHuyDonQuaHan stored procedure
                    // _db.ExecuteNonQuery("sp_TuDongHuyDonQuaHan");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Auto Cancel Service");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
