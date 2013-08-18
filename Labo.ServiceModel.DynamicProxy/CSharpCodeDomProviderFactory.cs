using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace Labo.ServiceModel.DynamicProxy
{
    public sealed class CSharpCodeDomProviderFactory : ICodeDomProviderFactory
    {
        public CodeDomProvider CreateProvider()
        {
            return new CSharpCodeProvider();
        }
    }
}