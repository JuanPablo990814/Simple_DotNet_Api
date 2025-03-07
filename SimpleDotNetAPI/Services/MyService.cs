namespace SimpleDotNetAPI.Services
{
    public class MyService : IMyService
    {
        private static readonly Random _random = new Random();
        private readonly int _serviceId;
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger)
        {
            _serviceId = _random.Next(100000, 999999); // Generate a random 6-digit number
            _logger = logger;
        }

        public void LogCreation(string message)
        {
            _logger.LogInformation($"{message} - Service ID: {_serviceId}");
        }
    }


    
}