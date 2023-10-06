namespace TheUnnamed.Web.Blazor.Jobs
{
    public class RemoveDocumentsWithoutStream  :IHostedService, IDisposable
    {
        private readonly ILogger<RemoveDocumentsWithoutStream> _logger;
        private Timer? _timer = null;

        public RemoveDocumentsWithoutStream(ILogger<RemoveDocumentsWithoutStream> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(15));

            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            _logger.LogDebug("Timer service checking for documents");


        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
