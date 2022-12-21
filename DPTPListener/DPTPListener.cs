using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace DPTPLibrary
{
    public class DPTPListener
    {
        private TcpListener _listener;
        
        public DPTPListener(string localaddr, int port)
        {
            _listener = new TcpListener(IPAddress.Parse(localaddr), port);
        }

        public DPTPListener(IPAddress localaddr, int port)
        {
            _listener = new TcpListener(localaddr, port);
        }

        public async Task<DPTPClient> AcceptClientAsync()
        {
            var client = await _listener.AcceptTcpClientAsync();
            return new DPTPClient(client);
        }

        public void Start() { _listener.Start(); }

        public void Stop() { _listener.Stop(); }
    }
}