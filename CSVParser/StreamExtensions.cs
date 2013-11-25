using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSVParser
{
    public static class StreamExtensions
    {
        public static int Peek(this System.IO.Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new Exception();
            }

            var returnValue =  stream.ReadByte();
            stream.Position -= 1;
            return returnValue;
        }

        public static string ReadLine(this System.IO.Stream stream)
        {
            if (!stream.CanRead)
            {
                throw new Exception();
            }

            var buffer = new StringBuilder();

            var character = stream.ReadByte();
            
            while (character != -1)
            {
                
                if (character != 13)
                {
                    buffer.Append(Convert.ToChar(character));
                }
                else
                {
                    var nextChar = stream.ReadByte();
                    if (nextChar != 10)
                    {
                        stream.Position -= 1;
                    }
                    return buffer.ToString();
                }
                character = stream.ReadByte();
            } 

            return buffer.ToString();

        }
    }
}
