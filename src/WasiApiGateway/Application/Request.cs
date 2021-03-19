using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace WasiApiGateway.Application
{
    public record Request(
        string Method,
        string Path,
        IReadOnlyDictionary<string, StringValues> Query,
        IReadOnlyDictionary<string, StringValues> Headers,
        byte[] Body
    );

    public record Response(
        IReadOnlyDictionary<string, string[]> Headers,
        byte[] Body
    );
}