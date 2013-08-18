using System.Collections.Generic;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public sealed class Method
    {
        public string Name { get; set; }

        private IList<Parameter> m_Parameters;
        public IList<Parameter> Parameters
        {
            get
            {
                return m_Parameters ?? (m_Parameters = new List<Parameter>());
            }
        }

        public Instance ReturnValue { get; set; }
    }
}