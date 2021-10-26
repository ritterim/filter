using System;

namespace Filter.Tests.Common.Testing.Configuration
{
    public class RimDevTestsElasticsearchConfiguration
    {
        /// <summary>Port number (host.port) for the Elasticsearch instance.  It's kept separate
        /// in the C# configuration to make things easier in cases where the build script is
        /// spinning up a Docker instance.
        /// </summary>
        public int? Port { get; set; } = 9200;
        
        /// <summary>Port number (transport.port) for node communication within the Elasticsearch
        /// instance. This is no longer used by the client libraries when talking to Elasticsearch
        /// but is still required to be specified on the Docker configuration.
        /// </summary>
        public int? TransportPort { get; set; } = 9300;
        
        /// <summary>The base URI for talking to the Elasticsearch instance in tests.
        /// This gets combined with Port (if specified) to form the final URI. 
        /// </summary>
        public Uri BaseUri { get; set; } = new Uri("http://localhost/");

        /// <summary>URI to use when tests access the Elasticsearch instance.</summary>
        public Uri Uri {
            get
            {
                if (Port is null || Port == BaseUri.Port) return BaseUri;
                var uriBuilder = new UriBuilder(BaseUri) { Port = Port.Value };
                return uriBuilder.Uri;
            }
        }
    }
}