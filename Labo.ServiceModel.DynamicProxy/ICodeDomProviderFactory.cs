using System.CodeDom.Compiler;

namespace Labo.ServiceModel.DynamicProxy
{
    public interface ICodeDomProviderFactory
    {
        CodeDomProvider CreateProvider();
    }
}