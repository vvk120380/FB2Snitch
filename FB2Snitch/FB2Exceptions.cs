using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FB2Snitch
{


    [Serializable]
    public class FB2DBException : Exception
    {
        public FB2DBException() { }
        public FB2DBException(string message) : base(message) { }
        public FB2DBException(string message, Exception inner) : base(message, inner) { }
        protected FB2DBException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class FB2ZipException : Exception
    {
        public FB2ZipException() { }
        public FB2ZipException(string message) : base(message) { }
        public FB2ZipException(string message, Exception inner) : base(message, inner) { }
        protected FB2ZipException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class FB2MD5HashException : Exception
    {
        public FB2MD5HashException() { }
        public FB2MD5HashException(string message) : base(message) { }
        public FB2MD5HashException(string message, Exception inner) : base(message, inner) { }
        protected FB2MD5HashException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class FB2BLLException : Exception
    {
        public FB2BLLException() { }
        public FB2BLLException(string message) : base(message) { }
        public FB2BLLException(string message, Exception inner) : base(message, inner) { }
        protected FB2BLLException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    public class FB2BaseException : Exception
    {
        public FB2BaseException() { }
        public FB2BaseException(string message) : base(message) { }
        public FB2BaseException(string message, Exception inner) : base(message, inner) { }
        protected FB2BaseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


}
    
