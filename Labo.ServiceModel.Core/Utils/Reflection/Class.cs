using System.Collections.Generic;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public sealed class Class : Instance
    {
        private IList<Property> m_Properties;
        public IList<Property> Properties
        {
            get
            {
                return m_Properties ?? (m_Properties = new List<Property>());
            }
        }
    }
}