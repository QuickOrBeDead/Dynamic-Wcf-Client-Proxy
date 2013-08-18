using System.Collections.ObjectModel;
using System.ServiceModel.Description;

namespace Labo.ServiceModel.DynamicProxy
{
    public sealed class ServiceClientProxyFactoryGenerator : IServiceClientProxyFactoryGenerator
    {
        private readonly IServiceClientProxyCompiler m_ServiceClientProxyCompiler;
        private readonly IServiceMetadataDownloader m_ServiceMetadataDownloader;
        private readonly IServiceMetadataImporter m_ServiceMetadataImporter;

        public ServiceClientProxyFactoryGenerator(IServiceMetadataDownloader serviceMetadataDownloader, 
                                                  IServiceMetadataImporter serviceMetadataImporter, 
                                                  IServiceClientProxyCompiler serviceClientProxyCompiler)
        {
            m_ServiceMetadataDownloader = serviceMetadataDownloader;
            m_ServiceMetadataImporter = serviceMetadataImporter;
            m_ServiceClientProxyCompiler = serviceClientProxyCompiler;
        }

        public ServiceClientProxyFactory GenerateProxyFactory(string serviceUrl)
        {
            Collection<MetadataSection> metadataSections = m_ServiceMetadataDownloader.DownloadMetadata(serviceUrl);
            ServiceMetadataInformation metadataInformation = m_ServiceMetadataImporter.ImportMetadata(metadataSections, MetadataImporterSerializerFormatMode.DataContractSerializer);
            ServiceClientProxyCompileResult clientProxyCompileResult = m_ServiceClientProxyCompiler.CompileProxy(metadataInformation);
            return new ServiceClientProxyFactory(clientProxyCompileResult);
        }
    }
}