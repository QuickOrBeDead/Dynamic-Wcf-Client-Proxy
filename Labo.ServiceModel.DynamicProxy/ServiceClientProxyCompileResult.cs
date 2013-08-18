using System;
using System.Reflection;

namespace Labo.ServiceModel.DynamicProxy
{
    [Serializable]
    public sealed class ServiceClientProxyCompileResult
    {
        public ServiceMetadataInformation ServiceMetadataInformation { get; private set; }

        public Assembly CompiledAssembly { get; private set; }

        public string Config { get; private set; }

        public ServiceClientProxyCompileResult(ServiceMetadataInformation serviceMetadataInformation, Assembly compiledAssembly, string config)
        {
            Config = config;
            CompiledAssembly = compiledAssembly;
            ServiceMetadataInformation = serviceMetadataInformation;
        }
    }
}