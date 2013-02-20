using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonTest.Classes;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace CommonTest
{
    public partial class Form1 : Form
    {

        private ConcurrentBag<PortFollow> _ports;

        private XBRawSocket _socket;

        public Timer _timer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _socket = new XBRawSocket();
            _socket.PacketArrival += socket_PacketArrival;
            _timer = new Timer();
            _timer.Interval = 5000;
            _timer.Tick += _timer_Tick;
            _timer.Start();
            _ports = new ConcurrentBag<PortFollow>();
        }

        private void socket_PacketArrival(object sender, PacketArrivedEventArgs args)
        {
            DealMessage(args);
        }

        //private delegate void DealMessageDele(PacketArrivedEventArgs args);

        private void DealMessage(PacketArrivedEventArgs args)
        {
            //if(InvokeRequired)
            //{
            //    BeginInvoke(new DealMessageDele(DealMessage),args);
            //    return;
            //}
            if (args.OriginationAddress == _socket.ComputerIP.ToString())
            {
                string port = args.OriginationPort;
                string pro = args.Protocol;
                var client = _ports.FirstOrDefault(ele => ele.OpenPorts == port && ele.ProType == pro);
                if (client == null)
                {
                    client = new PortFollow() { OpenPorts = port, ProType = pro };
                    _ports.Add(client);
                }
                client.upload += (int)args.PacketLength;
            }
            else
            {
                string port = args.DestinationPort;
                string pro = args.Protocol;
                var client = _ports.FirstOrDefault(ele => ele.OpenPorts == port && ele.ProType == pro);
                if (client == null)
                {
                    client = new PortFollow() { OpenPorts = port, ProType = pro };
                    _ports.Add(client);
                }
                client.download += (int)args.PacketLength;
            }
        }

        protected void _timer_Tick(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            int inter = _timer.Interval / 1000;
            foreach (var client in _ports)
            {
                client.uploadper = client.upload / inter;
                client.uploadAll += client.upload;
                client.upload = 0;
                client.downloadper = client.download / inter;
                client.downloadAll += client.download;
                client.download = 0;
                richTextBox1.AppendText(
                    string.Format(
                    "{0}:{1}  up {2} / {3}  down {4} / {5}{6}",
                    client.ProType,
                    client.OpenPorts,
                    client.uploadper,
                    client.uploadAll,
                    client.downloadper,
                    client.downloadAll,
                    Environment.NewLine
                    ));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_socket.KeepRunning)
            {
                _socket.KeepRunning = false;
                _socket.Shutdown();
            }
            else
            {
                _socket.KeepRunning = true;
                _socket.CreateAndBindSocket();
                _socket.Run();
            }
        }
    }
}
