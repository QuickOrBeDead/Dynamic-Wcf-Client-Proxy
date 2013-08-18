using System.Reflection;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public static class Constants
    {
        public const BindingFlags CONSTRUCTOR_INFO_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        public const BindingFlags PROPERTY_INFO_BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    }
}
