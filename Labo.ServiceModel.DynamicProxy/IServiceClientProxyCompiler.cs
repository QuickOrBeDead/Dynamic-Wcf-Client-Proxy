namespace Labo.ServiceModel.DynamicProxy
{
    public interface IServiceClientProxyCompiler
    {
        ServiceClientProxyCompileResult CompileProxy(ServiceMetadataInformation serviceMetadataInfo);
    }
}