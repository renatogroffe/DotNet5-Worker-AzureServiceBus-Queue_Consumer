using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;

namespace WorkerServiceBusQueue
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ParametrosExecucao _parametrosExecucao;
        private readonly QueueClient _client;

        public Worker(ILogger<Worker> logger,
            ParametrosExecucao parametrosExecucao)
        {
            _logger = logger;
            _parametrosExecucao = parametrosExecucao;
            _client = new (
                _parametrosExecucao.ConnectionString,
                _parametrosExecucao.Queue,
                ReceiveMode.ReceiveAndDelete);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    _logger.LogInformation(
                        "Iniciando o processamento de mensagens...");
                    _client.RegisterMessageHandler(
                        async (message, stoppingToken) =>
                        {
                            await ProcessarMensagem(message);
                        }
                        ,
                        new MessageHandlerOptions(
                            async (e) =>
                            {
                                await ProcessarErro(e);
                            }
                        )
                    );
                });
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _client.CloseAsync();
            _logger.LogInformation(
                "Conexao com o Azure Service Bus fechada!");
        }

        private Task ProcessarMensagem(Message message)
        {
            var conteudo = Encoding.UTF8.GetString(message.Body);
            _logger.LogInformation("[Nova mensagem recebida] " + conteudo);
            return Task.CompletedTask;
        }

        private Task ProcessarErro(ExceptionReceivedEventArgs e)
        {
            _logger.LogError("[Falha] " +
                e.Exception.GetType().FullName + " " +
                e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}