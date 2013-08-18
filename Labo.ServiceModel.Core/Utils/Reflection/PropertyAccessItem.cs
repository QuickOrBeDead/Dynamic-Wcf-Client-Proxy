namespace Labo.ServiceModel.Core.Utils.Reflection
{
    internal sealed class PropertyAccessItem
    {
        public GetHandler Getter { get; set; }

        public SetHandler Setter { get; set; }
    }
}
