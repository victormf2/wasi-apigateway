using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            return stream.ToByteArrayAsync(CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static async Task<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream, cancellationToken);
            byte[] bytes = memoryStream.ToArray();
            return bytes;
        }
    }
}