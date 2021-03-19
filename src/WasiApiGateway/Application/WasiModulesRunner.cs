using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.IO;
using Wasmtime;
using System.Threading.Tasks;
using System.Threading;

namespace WasiApiGateway.Application
{
    public class WasiModulesRunner
    {
        public static async Task<string> Run(byte[] moduleContent, string message, CancellationToken cancellationToken)
        {
            var engineBuilder = new EngineBuilder();
            
            using var engine = engineBuilder.Build();
            using var module = Module.FromBytes(engine, "module", moduleContent);
            using var host = new Host(engine);

            var stdinFilePath = Path.GetTempFileName();
            await File.WriteAllTextAsync(stdinFilePath, message, cancellationToken);

            var stdoutFilePath = Path.GetTempFileName();

            var stderrFilePath = Path.GetTempFileName();

            host.DefineWasi("wasi_snapshot_preview1", new WasiConfiguration()
                .WithStandardInput(stdinFilePath)
                .WithStandardOutput(stdoutFilePath)
                .WithStandardError(stderrFilePath));
            
            using dynamic instance = host.Instantiate(module);

            
            string reply = await await Task.WhenAny(
                Task.Run(async () =>
                {
                    instance.run();

                    string errorReply = await File.ReadAllTextAsync(stderrFilePath, cancellationToken);

                    if (errorReply is not "")
                    {
                        throw new ModuleRunningException(errorReply);
                    }

                    string successReply = await File.ReadAllTextAsync(stdoutFilePath, cancellationToken);
                    return successReply;
                }),
                WaitCancellation<string>(cancellationToken)
            );

            return reply;
        }

        private static async Task<T> WaitCancellation<T>(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public class ModuleRunningException : Exception
    {
        public ModuleRunningException(string message = null, Exception innerExecption = null) : base(message, innerExecption)
        {
            
        }
    }
}