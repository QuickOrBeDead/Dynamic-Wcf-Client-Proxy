using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Xml;

namespace Labo.ServiceModel.DynamicProxy
{
    public sealed class ServiceClientProxyCompiler : IServiceClientProxyCompiler
    {
        public ServiceClientProxyCompileResult CompileProxy(ServiceMetadataInformation serviceMetadataInfo)
        {
            string tempConfigFileName = CreateTempConfigFile();
            CodeCompileUnit codeCompileUnit = serviceMetadataInfo.CodeCompileUnit;
            ServiceContractGenerator contractGenerator = new ServiceContractGenerator(codeCompileUnit, CreateConfig(new FileInfo(tempConfigFileName)));
            contractGenerator.Options |= ServiceContractGenerationOptions.ClientClass;

            for (int i = 0; i < serviceMetadataInfo.Contracts.Count; i++)
            {
                ContractDescription contract = serviceMetadataInfo.Contracts[i];
                contractGenerator.GenerateServiceContractType(contract);
            }

            bool success = true;
            Collection<MetadataConversionError> contractGenErrors = contractGenerator.Errors;
            if (contractGenErrors != null)
            {
                foreach (MetadataConversionError error in contractGenErrors)
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

            CodeDomProvider codeDomProvider = serviceMetadataInfo.CodeDomProvider;
            string proxyCode = CreateProxyCode(codeDomProvider, codeCompileUnit);

            CompilerParameters compilerParameters = new CompilerParameters();

            AddAssemblyReference(typeof(ServiceContractAttribute).Assembly, compilerParameters.ReferencedAssemblies);
            AddAssemblyReference(typeof(System.Web.Services.Description.ServiceDescription).Assembly, compilerParameters.ReferencedAssemblies);
            AddAssemblyReference(typeof(DataContractAttribute).Assembly, compilerParameters.ReferencedAssemblies);
            AddAssemblyReference(typeof(XmlElement).Assembly, compilerParameters.ReferencedAssemblies);
            AddAssemblyReference(typeof(Uri).Assembly, compilerParameters.ReferencedAssemblies);
            AddAssemblyReference(typeof(DataSet).Assembly, compilerParameters.ReferencedAssemblies);

            CompilerResults results = codeDomProvider.CompileAssemblyFromSource(compilerParameters, proxyCode);

            CompilerErrorCollection compileErrors = results.Errors;
            Assembly compiledAssembly = Assembly.LoadFile(results.PathToAssembly);
            return new ServiceClientProxyCompileResult(serviceMetadataInfo, compiledAssembly, GenerateConfig(contractGenerator, serviceMetadataInfo.Endpoints, tempConfigFileName));
        }

        private static string CreateTempConfigFile()
        {
            string tempConfigFileName = Path.GetTempFileName();
            File.WriteAllText(tempConfigFileName, "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<configuration>\r\n</configuration>");
            return tempConfigFileName;
        }

        private static string CreateProxyCode(CodeDomProvider codeDomProvider, CodeCompileUnit codeCompileUnit)
        {
            string proxyCode;
            using (StringWriter writer = new StringWriter())
            {
                CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();
                codeGenOptions.BracingStyle = "C";
                codeDomProvider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, codeGenOptions);
                writer.Flush();

                proxyCode = writer.ToString();
            }
            return proxyCode;
        }

        private static void AddAssemblyReference(Assembly referencedAssembly, StringCollection referencedAssemblies)
        {
            string path = Path.GetFullPath(referencedAssembly.Location);
            string name = Path.GetFileName(path);
            if (!(referencedAssemblies.Contains(name) || referencedAssemblies.Contains(path)))
            {
                referencedAssemblies.Add(path);
            }
        }

        private static Configuration CreateConfig(FileSystemInfo configFileInfo)
        {
            Configuration machineConfig = ConfigurationManager.OpenMachineConfiguration();
            ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
            exeConfigurationFileMap.ExeConfigFilename = configFileInfo.FullName;
            exeConfigurationFileMap.MachineConfigFilename = machineConfig.FilePath;
            Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
            configuration.NamespaceDeclared = true;
            return configuration;
        }

        private static string GenerateConfig(ServiceContractGenerator contractGenerator, IReadOnlyList<ServiceEndpoint> endpoints, string configFilePath)
        {
            for (int i = 0; i < endpoints.Count; i++)
            {
                ServiceEndpoint current = endpoints[i];
                ChannelEndpointElement channelEndpointElement;
                contractGenerator.GenerateServiceEndpoint(current, out channelEndpointElement);
            }

            Configuration configuration = contractGenerator.Configuration;
            configuration.NamespaceDeclared = false;
            configuration.Save();
            return File.ReadAllText(configFilePath);
        }
    }
}