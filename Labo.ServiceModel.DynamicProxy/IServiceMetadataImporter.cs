using System.Collections.ObjectModel;
using System.ServiceModel.Description;

namespace Labo.ServiceModel.DynamicProxy
{
    public interface IServiceMetadataImporter
    {
        ServiceMetadataInformation ImportMetadata(Collection<MetadataSection> metadataCollection, MetadataImporterSerializerFormatMode formatMode);
    }
}