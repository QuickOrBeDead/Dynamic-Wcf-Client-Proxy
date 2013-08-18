using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web.Services.Discovery;
using System.Xml;
using System.Xml.Schema;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;

namespace Labo.ServiceModel.DynamicProxy
{
    public sealed class ServiceMetadataDownloader : IServiceMetadataDownloader
    {
        public Collection<MetadataSection> DownloadMetadata(string serviceUrl)
        {
            Uri serviceUri = new Uri(serviceUrl);

            Collection<MetadataSection> metadataSections;
            if (TryDownloadByMetadataExchangeClient(serviceUri, out metadataSections))
            {
                return metadataSections;
            }
            else
            {
                if (TryDownloadByMetadataExchangeClient(GetDefaultMexUri(serviceUri), out metadataSections))
                {
                    return metadataSections;
                }
            }

            bool supporstDiscoveryClientProtocol = serviceUri.Scheme == Uri.UriSchemeHttp || serviceUri.Scheme == Uri.UriSchemeHttps;
            if (supporstDiscoveryClientProtocol)
            {
                DiscoveryClientProtocol disco = new DiscoveryClientProtocol();
                disco.AllowAutoRedirect = true;
                disco.UseDefaultCredentials = true;
                disco.DiscoverAny(serviceUrl);
                disco.ResolveAll();

                Collection<MetadataSection> result = new Collection<MetadataSection>();
                if (disco.Documents.Values != null)
                {
                    foreach (object document in disco.Documents.Values)
                    {
                        AddDocumentToResults(document, result);
                    }
                }
                return result;
            }
            return null;
        }

        private static bool TryDownloadByMetadataExchangeClient(Uri serviceUri, out Collection<MetadataSection> metadataSections)
        {
            try
            {
                MetadataExchangeClient mexClient = CreateMetadataExchangeClient(serviceUri);
                mexClient.OperationTimeout = TimeSpan.FromMinutes(5.0);
                metadataSections = mexClient.GetMetadata().MetadataSections;
                return true;
            }
            catch
            {
                metadataSections = null;
                return false;
            }
        }

        private static Uri GetDefaultMexUri(Uri serviceUri)
        {
            if (serviceUri.AbsoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri(serviceUri, "./mex");
            }
            return new Uri(serviceUri.AbsoluteUri + "/mex");
        }

        private static MetadataExchangeClient CreateMetadataExchangeClient(Uri serviceUri)
        {
            string scheme = serviceUri.Scheme;
            MetadataExchangeClient result = null;
            if (string.Compare(scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) == 0)
            {
                WSHttpBinding wSHttpBinding = (WSHttpBinding)MetadataExchangeBindings.CreateMexHttpBinding();
                wSHttpBinding.MaxReceivedMessageSize = 67108864L;
                wSHttpBinding.ReaderQuotas.MaxNameTableCharCount = 1048576;
                result = new MetadataExchangeClient(wSHttpBinding);
            }
            else
            {
                if (string.Compare(scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    WSHttpBinding wSHttpBinding2 = (WSHttpBinding)MetadataExchangeBindings.CreateMexHttpsBinding();
                    wSHttpBinding2.MaxReceivedMessageSize = 67108864L;
                    wSHttpBinding2.ReaderQuotas.MaxNameTableCharCount = 1048576;
                    result = new MetadataExchangeClient(wSHttpBinding2);
                }
                else
                {
                    if (string.Compare(scheme, Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        CustomBinding tcpBinding = (CustomBinding)MetadataExchangeBindings.CreateMexTcpBinding();
                        tcpBinding.Elements.Find<TcpTransportBindingElement>().MaxReceivedMessageSize = 67108864L;
                        result = new MetadataExchangeClient(tcpBinding);
                    }
                    else if (string.Compare(scheme, Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        CustomBinding namedPipeBinding = (CustomBinding)MetadataExchangeBindings.CreateMexNamedPipeBinding();
                        namedPipeBinding.Elements.Find<NamedPipeTransportBindingElement>().MaxReceivedMessageSize = 67108864L;
                        result = new MetadataExchangeClient(namedPipeBinding);
                    }
                }
            }
            return result;
        }

        private static void AddDocumentToResults(object document, ICollection<MetadataSection> results)
        {
            ServiceDescription serviceDescription = document as ServiceDescription;
            XmlSchema xmlSchema = document as XmlSchema;
            XmlElement xmlElement = document as XmlElement;

            if (serviceDescription != null)
            {
                results.Add(MetadataSection.CreateFromServiceDescription(serviceDescription));
            }
            else if (xmlSchema != null)
            {
                results.Add(MetadataSection.CreateFromSchema(xmlSchema));
            }
            else if (xmlElement != null && (xmlElement.NamespaceURI == "http://schemas.xmlsoap.org/ws/2004/09/policy" || xmlElement.NamespaceURI == "http://www.w3.org/ns/ws-policy") && xmlElement.LocalName == "Policy")
            {
                results.Add(MetadataSection.CreateFromPolicy(xmlElement, null));
            }
            else
            {
                MetadataSection mexDoc = new MetadataSection();
                mexDoc.Metadata = document;
                results.Add(mexDoc);
            }
        }
    }
}
