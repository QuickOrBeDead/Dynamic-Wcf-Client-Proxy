namespace Labo.ServiceModel.DynamicProxy
{
    internal interface IServiceClientProxyFactoryGenerator
    {
        ServiceClientProxyFactory GenerateProxyFactory(string serviceUrl);
    }
}