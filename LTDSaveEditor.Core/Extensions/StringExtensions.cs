using MurmurHash;
using System.Text;

namespace LTDSaveEditor.Core.Extensions;

public static class StringExtensions
{
    extension(string value)
    {
        public uint ToMurmur()
        {
            ReadOnlySpan<byte> inputSpan = Encoding.UTF8.GetBytes(value).AsSpan();

            return MurmurHash3.Hash32(ref inputSpan, 0);
        }
    }
}
