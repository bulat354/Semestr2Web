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

        public DPTPClient AcceptClient()
        {
            var client = await _listener.AcceptTcpClientAsync();
            return new DPTPClient(client);
        }

        public async Task<DPTPClient?> AcceptClientAsync(CancellationToken token)
        {
            var client = await _listener.AcceptTcpClientAsync(token);
            return new DPTPClient(client);
        }

        public void Start() { _listener.Start(); }

        public void Stop() { _listener.Stop(); }
    }
}