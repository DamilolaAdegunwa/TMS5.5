using System;
using System.ServiceModel;

namespace Egoal.Net.Wcf
{
    public class WcfProxy<T> : IWcfProxy<T> where T : class, ICommunicationObject
    {
        private Lazy<T> client;
        private bool disposed;

        public WcfProxy(string endpointUrl)
        {
            client = new Lazy<T>(() => CreateChannel(endpointUrl));
        }

        private static T CreateChannel(string endpointUrl)
        {
            if (string.IsNullOrWhiteSpace(endpointUrl))
            {
                throw new ArgumentException("Endpoint cannot be empty.");
            }

            var binding = new BasicHttpBinding();
            binding.MaxBufferSize = int.MaxValue;
            binding.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.AllowCookies = true;

            EndpointAddress endpointAddress = new EndpointAddress(endpointUrl);
            var factory = new ChannelFactory<T>(binding, endpointAddress);

            return factory.CreateChannel();
        }

        public void Execute(Action<T> function)
        {
            function(client.Value);
        }

        public TResult Execute<TResult>(Func<T, TResult> function)
        {
            return function(client.Value);
        }

        /// <summary>
        /// Explicit disposal: manual call to dispose and suppression of further garbage collection calls.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Do not repeat dispose if called more than once.
            if (!disposed)
            {
                var closedSuccessfully = false;

                try
                {
                    // Attempt to close client connection only if it is not in a faulted state.
                    if (client.Value.State != CommunicationState.Faulted)
                    {
                        client.Value.Close();
                        closedSuccessfully = true;
                    }
                }
                finally
                {
                    // Force transition to closed if closing was unsuccessful.
                    if (!closedSuccessfully)
                    {
                        client.Value.Abort();
                    }
                }

                client = null;
                disposed = true;
            }
        }

        /// <summary>
        /// Implicit disposal: garbage collector calls finalizer and the dispose method through it.
        /// </summary>
        ~WcfProxy()
        {
            Dispose(false);
        }
    }
}
