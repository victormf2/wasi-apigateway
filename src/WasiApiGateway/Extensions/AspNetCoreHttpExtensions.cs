using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using WasiApiGateway.Application;
using Microsoft.Extensions.Primitives;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class AspNetCoreHttpExtensions
    {
        public static async Task<Request> AsApplicationRequest(this HttpRequest httpRequest, CancellationToken cancellationToken)
        {
            return new Request(
                Method: httpRequest.Method,
                Path: httpRequest.Path,
                Query: new Dictionary<string, StringValues>(httpRequest.Query),
                Headers: new Dictionary<string, StringValues>(httpRequest.Headers),
                Body: await httpRequest.Body.ToByteArrayAsync(cancellationToken)
            );
        }
    }
}