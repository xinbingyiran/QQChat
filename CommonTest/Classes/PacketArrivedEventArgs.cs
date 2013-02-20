using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonTest.Classes
{
    class PacketArrivedEventArgs
    {
        public uint HeaderLength { get; set; }

        public string Protocol { get; set; }

        public string IPVersion { get; set; }

        public string OriginationAddress { get; set; }

        public string DestinationAddress { get; set; }

        public string OriginationPort { get; set; }

        public string DestinationPort { get; set; }

        public uint PacketLength { get; set; }

        public uint MessageLength { get; set; }

        public byte[] ReceiveBuffer { get; set; }

        public byte[] IPHeaderBuffer { get; set; }

        public byte[] MessageBuffer { get; set; }
    }
}
