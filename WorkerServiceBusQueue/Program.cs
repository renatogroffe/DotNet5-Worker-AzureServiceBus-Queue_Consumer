using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerServiceBusQueue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(
                "Testando o consumo de mensagens com Azure Service Bus + Filas");

            if (args.Length != 2)
            {
                Console.WriteLine(
                    "Informe 2 parâmetros: " +
                    "no primeiro a string de conexao com o Azure Service Bus, " +
                    "no segundo a Fila/Queue a ser utilizado no consumo das mensagens...");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ParametrosExecucao>(
                        new ParametrosExecucao()
                        {
                            ConnectionString = args[0],
                            Queue = args[1]
                        });
                    services.AddHostedService<Worker>();
                });
    }
}