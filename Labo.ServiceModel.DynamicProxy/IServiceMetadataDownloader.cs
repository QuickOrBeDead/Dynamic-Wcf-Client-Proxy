using System.Collections.ObjectModel;
using System.ServiceModel.Description;

namespace Labo.ServiceModel.DynamicProxy
{
    public interface IServiceMetadataDownloader
    {
        Collection<MetadataSection> DownloadMetadata(string wsdlUrl);
    }
}