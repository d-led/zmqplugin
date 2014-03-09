using MsgPack.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zmqpluginuser
{
    static class Extensions
    {
        public static byte[] ToByteArray(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string ToStringFromByteArray(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] PackMsg<T>(T t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.PackSingleObject(t);
        }

        public static byte[] Pack<T>(this T t)
        {
            return PackMsg(t);
        }

        public static T UnpackMsg<T>(byte[] t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.UnpackSingleObject(t);
        }

        public static T Unpack<T>(this byte[] t)
        {
            var serializer = MessagePackSerializer.Create<T>();
            return serializer.UnpackSingleObject(t);
        }

        public static byte[] ToASCII(this string s)
        {
            return System.Text.Encoding.ASCII.GetBytes(s);
        }

        /// <summary>
        /// http://stackoverflow.com/a/969327/847349
        /// </summary>
        public static TimeSpan Time(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
