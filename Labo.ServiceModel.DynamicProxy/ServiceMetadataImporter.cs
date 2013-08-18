using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Design;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.Xml.Serialization;
using Binding = System.ServiceModel.Channels.Binding;

namespace Labo.ServiceModel.DynamicProxy
{
    public sealed class ServiceMetadataImporter : IServiceMetadataImporter
    {
        private readonly ICodeDomProviderFactory m_CodeDomProviderFactory;

        public ServiceMetadataImporter(ICodeDomProviderFactory codeDomProviderFactory)
        {
            m_CodeDomProviderFactory = codeDomProviderFactory;
        }

        public ServiceMetadataInformation ImportMetadata(Collection<MetadataSection> metadataCollection, MetadataImporterSerializerFormatMode formatMode)
        {
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeDomProvider codeDomProvider = m_CodeDomProviderFactory.CreateProvider();

            WsdlImporter importer = new WsdlImporter(new MetadataSet(metadataCollection));
            switch (formatMode)
            {
                case MetadataImporterSerializerFormatMode.DataContractSerializer:
                    AddStateForDataContractSerializerImport(importer, formatMode, codeCompileUnit, codeDomProvider);
                    break;
                case MetadataImporterSerializerFormatMode.XmlSerializer:
                    AddStateForXmlSerializerImport(importer, codeCompileUnit, codeDomProvider);
                    break;
                case MetadataImporterSerializerFormatMode.Auto:
                    AddStateForDataContractSerializerImport(importer, formatMode, codeCompileUnit, codeDomProvider);
                    AddStateForXmlSerializerImport(importer, codeCompileUnit, codeDomProvider);
                    break;
            }

            if (!importer.State.ContainsKey(typeof(WrappedOptions)))
            {
                importer.State.Add(typeof(WrappedOptions), new WrappedOptions
                {
                    WrappedFlag = false
                });
            }

            Collection<Binding> bindings = importer.ImportAllBindings();
            Collection<ContractDescription> contracts = importer.ImportAllContracts();
            ServiceEndpointCollection endpoints = importer.ImportAllEndpoints();
            Collection<MetadataConversionError> importErrors = importer.Errors;

            bool success = true;
            if (importErrors != null)
            {
                foreach (MetadataConversionError error in importErrors)
                {
                    if (!error.IsWarning)
                    {
                        success = false;
                        break;
                    }
                }
            }
            if (!success)
            {
                //TODO: Throw exception
            }
           return new ServiceMetadataInformation(codeCompileUnit, codeDomProvider)
               {
                   Bindings = bindings,
                   Contracts = contracts,
                   Endpoints = endpoints
               };
        }

        private static void AddStateForXmlSerializerImport(MetadataImporter importer, CodeCompileUnit codeCompileUnit, CodeDomProvider codeDomProvider)
        {

            XmlSerializerImportOptions importOptions = new XmlSerializerImportOptions(codeCompileUnit);
            importOptions.CodeProvider = codeDomProvider;
            importOptions.WebReferenceOptions = new WebReferenceOptions();
            importOptions.WebReferenceOptions.CodeGenerationOptions = CodeGenerationOptions.GenerateProperties | CodeGenerationOptions.GenerateOrder;
            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(typeof(TypedDataSetSchemaImporterExtension).AssemblyQualifiedName);
            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);

            importer.State.Add(typeof(XmlSerializerImportOptions), importOptions);
        }

        private static void AddStateForDataContractSerializerImport(WsdlImporter importer, MetadataImporterSerializerFormatMode formatMode, CodeCompileUnit codeCompileUnit, CodeDomProvider codeDomProvider)
        {
            XsdDataContractImporter xsdDataContractImporter = new XsdDataContractImporter(codeCompileUnit);
            xsdDataContractImporter.Options = CreateDataContractImportOptions(formatMode, codeDomProvider);
            importer.State.Add(typeof(XsdDataContractImporter), xsdDataContractImporter);

            for (int i = 0; i < importer.WsdlImportExtensions.Count; i++)
            {
                IWsdlImportExtension importExtension = importer.WsdlImportExtensions[i];
                DataContractSerializerMessageContractImporter dataContractSerializerMessageContractImporter = importExtension as DataContractSerializerMessageContractImporter;

                if (dataContractSerializerMessageContractImporter != null)
                {
                    dataContractSerializerMessageContractImporter.Enabled = formatMode != MetadataImporterSerializerFormatMode.XmlSerializer;
                }
            }
        }

        private static ImportOptions CreateDataContractImportOptions(MetadataImporterSerializerFormatMode formatMode, CodeDomProvider codeDomProvider)
        {
            ImportOptions importOptions = new ImportOptions();
            importOptions.GenerateSerializable = true;
            importOptions.GenerateInternal = false;
            importOptions.ImportXmlType = formatMode == MetadataImporterSerializerFormatMode.DataContractSerializer;
            importOptions.EnableDataBinding = false;
            importOptions.CodeProvider = codeDomProvider;
            return importOptions;
        }
    }
}