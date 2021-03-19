using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using WasiApiGateway.JsonConverters;

namespace WasiApiGateway.Application
{
    public class ApiHandlersContainer
    {
        private readonly Dictionary<(string path, string method), ApiHandler> _apiHandlersMappings = new();

        public void AddOrUpdateApiHandler(string path, string method, Stream wasiModuleStream)
        {
            _apiHandlersMappings[(NormalizePath(path), method.ToUpper())] = ApiHandler.FromStream(wasiModuleStream);
        }

        public IEnumerable<(string path, string method)> GetMappedRoutes()
        {
            return _apiHandlersMappings.Keys;
        }

        public ApiHandler GetApiHandler(string path, string method)
        {

            return _apiHandlersMappings.GetValueOrDefault((path, method));
        }

        public string NormalizePath(string path)
        {
            return path;
        }
    }

    public class ApiHandler
    {
        private static readonly JsonSerializerOptions requestJsonSerializerOptions;
        static ApiHandler() {
            requestJsonSerializerOptions= new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            requestJsonSerializerOptions.Converters.Add(new ByteArrayConverter());
        }

        private byte[] _wasiModuleContent;
        private ApiHandler(Stream wasiModuleStream)
        {
            _wasiModuleContent = wasiModuleStream.ToByteArray();
        }

        public static ApiHandler FromStream(Stream stream) => new ApiHandler(stream);
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        { 
            var message = JsonSerializer.Serialize(request, requestJsonSerializerOptions);
            var reply = await WasiModulesRunner.Run(_wasiModuleContent, message, cancellationToken);
            var response = JsonSerializer.Deserialize<Response>(reply, requestJsonSerializerOptions);
            return response;
        }
    }
}