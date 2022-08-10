using System;
using System.Buffers;
using System.IO;
using System.Linq;
using MessagePack;

namespace Prota
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class ProtaSerializeAttribute : MessagePackObjectAttribute
    {
        public ProtaSerializeAttribute(bool keyAsPropertyName = true) : base(keyAsPropertyName) { }
    }
    
    // Simple wrap to MessagePack.
    // Serialize/Deserialize have only as simple interface.
    public static class ProtaSerializer
    {
        static MessagePackSerializerOptions options;
        
        [ThreadStatic]
        static byte[] _serializeTempBuffer;
        static byte[] serializeTempBuffer => _serializeTempBuffer != null ? _serializeTempBuffer : _serializeTempBuffer = new byte[2 * 1024];       // 2k cache.
        
        [ThreadStatic]
        static byte[] _deserializeTempBuffer;
        static byte[] deserializeTempBuffer => _deserializeTempBuffer != null ? _deserializeTempBuffer : _deserializeTempBuffer = new byte[2 * 1024];       // 2k cache.
        
        
        // Serialize and write data to arr.
        public static int Serialize<T>(this T target, byte[] arr, int start = 0) => target.Serialize(new ArraySegment<byte>(arr, start, arr.Length - start));
        
        // Serialize and writer data to arr.
        public static int Serialize<T>(this T target, ArraySegment<byte> arr)
        {
            var writer = new MessagePackWriter(SequencePool.Shared, serializeTempBuffer);
            MessagePackSerializer.Serialize(ref writer, target, options);
            return (int)writer.FlushAndGetArray(arr.Array, arr.Offset);
        }
        
        public static T Deserialize<T>(this byte[] arr) => Deserialize<T>(arr, 0, arr.Length);
        public static T Deserialize<T>(this byte[] arr, out int bytesRead) => Deserialize<T>(arr, 0, arr.Length, out bytesRead);
        public static T Deserialize<T>(this byte[] arr, int start, int length) => Deserialize<T>(new ArraySegment<byte>(arr, start, length));
        public static T Deserialize<T>(this byte[] arr, int start, int length, out int bytesRead) => Deserialize<T>(new ArraySegment<byte>(arr, start, length), out bytesRead);
        public static T Deserialize<T>(this ArraySegment<byte> arr) => Deserialize<T>(arr, out _);
        public static T Deserialize<T>(this ArraySegment<byte> arr, out int bytesRead) => MessagePackSerializer.Deserialize<T>((ReadOnlyMemory<byte>)arr, options, out bytesRead);
        public static T Deserialize<T>(this Span<byte> arr) => Deserialize<T>(arr, out _);
        public static T Deserialize<T>(this Span<byte> arr, out int bytesRead)
        {
            arr.CopyTo(deserializeTempBuffer);
            return MessagePackSerializer.Deserialize<T>(deserializeTempBuffer, options, out bytesRead);
        }
        
        
        
        static ProtaSerializer()
        {
            options = MessagePackSerializerOptions.Standard
                .WithCompression(MessagePackCompression.Lz4BlockArray)
                .WithOmitAssemblyVersion(true);
        }
        
        
        
        [MessagePackObject]
        public struct StructForUnitTest
        {
            [Key(0)]
            public int a;
            [Key(1)]
            public int b;
        }
        
        [MessagePackObject]
        public struct StructForUnitTestSec
        {
            [Key(0)]
            public int b;
            [Key(1)]
            public int a;
        }
        
        public static void UnitTest()
        {
            var v = new StructForUnitTest();
            v.a = 11;
            v.b = 22;
            
            byte[] arr = new byte[128];
            var segment = arr.AsSegment();
            var size = v.Serialize(arr);
            
            Console.WriteLine($"count: { size }");
            Console.WriteLine($"data: { string.Join(" ", segment.Take(size).Select(x => x.ToString("X2"))) }");
            
            var g = segment.Deserialize<StructForUnitTestSec>();
            (g.a == v.b).Assert();
            (g.b == v.a).Assert();
            
            Console.WriteLine("done!");
            
        }
    }
}