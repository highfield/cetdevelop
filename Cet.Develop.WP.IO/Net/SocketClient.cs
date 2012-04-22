using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;


namespace Cet.Develop.Windows.IO.Net
{
    /// <summary>
    /// Windows Phone tailored socket client class
    /// </summary>
    /// <remarks>
    /// Have a look here: http://msdn.microsoft.com/en-us/library/hh202858(VS.92).aspx
    /// Some little modification by M. Vernari
    /// </remarks>
    public class SocketClient
        : ICommClient, IDisposable
    {
        // Define a timeout in milliseconds for each asynchronous call. If a response is not received within this 
        // timeout period, the call is aborted.
        private const int TIMEOUT_MILLISECONDS = 5000;

        // The maximum size of the data buffer to use with the asynchronous socket methods
        private const int MaxBufferSize = 2048;

        // Cached Socket object that will be used by each call for the lifetime of this class
        private Socket _socket = null;

        // Signaling object used to notify when an asynchronous operation is completed
        private static ManualResetEvent _clientDone = new ManualResetEvent(false);



        public int Latency { get; set; }
        public string LastError { get; private set; }



        /// <summary>
        /// Entry-point for submitting a query to the remote device
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CommResponse Query(ClientCommData data)
        {
            //convert the request data as an ordinary byte array
            byte[] outgoing = data
                .OutgoingData
                .ToArray();

            int totalTimeout = this.Latency + data.Timeout;

            //retries loop
            for (int attempt = 0, retries = data.Retries; attempt < retries; attempt++)
            {
                //phyiscal writing
                this.Send(outgoing);

                //create a writer for accumulate the incoming data
                var incoming = new ByteArrayWriter();

                //read the incoming data from the physical port
                incoming.WriteBytes(
                    this.Receive(totalTimeout)
                    );

                //try to decode the stream
                data.IncomingData = incoming.ToReader();

                CommResponse result = data
                    .OwnerProtocol
                    .Codec
                    .ClientDecode(data);

                //exit whether any concrete result: either good or bad
                if (result.Status == CommResponse.Ack)
                {
                    return result;
                }
                else if (result.Status == CommResponse.Critical)
                {
                    return result;
                }
                else if (result.Status != CommResponse.Unknown)
                {
                    break;
                }

                //stop immediately if the host asked to abort

                //TODO

            }       //for

            //no attempt was successful
            return new CommResponse(
                data,
                CommResponse.Critical);
        }



        /// <summary>
        /// Attempt a TCP socket connection to the given host over the given port
        /// </summary>
        /// <param name="hostName">The name of the host</param>
        /// <param name="portNumber">The port number to connect</param>
        /// <returns>True when the connection succeeded</returns>
        public bool Connect(
            string hostName,
            int portNumber,
            int timeout = TIMEOUT_MILLISECONDS)
        {
            string result = "Operation Timeout";

            // Create DnsEndPoint. The hostName and port are passed in to this method.
            DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);

            // Create a stream-based, TCP socket using the InterNetwork Address Family. 
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Create a SocketAsyncEventArgs object to be used in the connection request
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;

            // Inline event handler for the Completed event.
            // Note: This event handler was implemented inline in order to make this method self-contained.
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                if (_socket.Connected)
                {
                    //success
                    result = string.Empty;
                }
                else
                {
                    // Retrieve the result of this request
                    result = e.SocketError.ToString();
                }

                // Signal that the request is complete, unblocking the UI thread
                _clientDone.Set();
            });

            // Sets the state of the event to nonsignaled, causing threads to block
            _clientDone.Reset();

            // Make an asynchronous Connect request over the socket
            _socket.ConnectAsync(socketEventArg);

            // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
            // If no response comes back within this time then proceed
            _clientDone.WaitOne(timeout);

            this.LastError = result;
            return string.IsNullOrEmpty(result);
        }



        /// <summary>
        /// Send the given data to the server using the established connection
        /// </summary>
        /// <param name="data">The data to send to the server</param>
        /// <returns>True when the sending succeeded</returns>
        public bool Send(
            IEnumerable<byte> data,
            int timeout = TIMEOUT_MILLISECONDS)
        {
            string response = "Operation Timeout";

            // We are re-using the _socket object that was initialized in the Connect method
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();

                // Set properties on context object
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;
                socketEventArg.UserToken = null;

                // Inline event handler for the Completed event.
                // Note: This event handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    response = e.SocketError.ToString();

                    // Unblock the thread
                    _clientDone.Set();
                });

                // Add the data to be sent into the buffer
                var buffer = data.ToArray();

                socketEventArg.SetBuffer(
                    buffer,
                    0,
                    buffer.Length);

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Send request over the socket
                _socket.SendAsync(socketEventArg);

                // Block the thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(timeout);
            }
            else
            {
                response = "Socket is not initialized";
            }

            this.LastError = response;
            return string.IsNullOrEmpty(response);
        }



        /// <summary>
        /// Receive data from the server using the established socket connection
        /// </summary>
        /// <returns>The data received from the server, when no error was thrown</returns>
        public byte[] Receive(int timeout = TIMEOUT_MILLISECONDS)
        {
            string response = "Operation Timeout";
            byte[] buffer = new byte[0];

            // We are receiving over an established socket connection
            if (_socket != null)
            {
                // Create SocketAsyncEventArgs context object
                SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
                socketEventArg.RemoteEndPoint = _socket.RemoteEndPoint;

                // Setup the buffer to receive the data
                socketEventArg.SetBuffer(
                    new Byte[MaxBufferSize],
                    0,
                    MaxBufferSize);

                // Inline event handler for the Completed event.
                // Note: This even handler was implemented inline in order to make this method self-contained.
                socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
                {
                    if (e.SocketError == SocketError.Success)
                    {
                        // Retrieve the data from the buffer
                        response = string.Empty;

                        buffer = new byte[e.BytesTransferred];
                        Buffer.BlockCopy(
                            e.Buffer,
                            e.Offset,
                            buffer,
                            0,
                            buffer.Length);
                    }
                    else
                    {
                        response = e.SocketError.ToString();
                    }

                    _clientDone.Set();
                });

                // Sets the state of the event to nonsignaled, causing threads to block
                _clientDone.Reset();

                // Make an asynchronous Receive request over the socket
                _socket.ReceiveAsync(socketEventArg);

                // Block the UI thread for a maximum of TIMEOUT_MILLISECONDS milliseconds.
                // If no response comes back within this time then proceed
                _clientDone.WaitOne(timeout);
            }
            else
            {
                response = "Socket is not initialized";
            }

            this.LastError = response;
            return buffer;
        }



        /// <summary>
        /// Closes the Socket connection and releases all associated resources
        /// </summary>
        public void Close()
        {
            if (_socket != null)
            {
                _socket.Close();
            }
        }



        #region IDisposable Members

        private bool _disposed = false;



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this.Close();
                    this._socket.Dispose();
                }

                this._disposed = true;
            }
        }



        ~SocketClient()
        {
            Dispose(false);
        }

        #endregion
    }
}
