using System;

namespace Labo.ServiceModel.Core.Utils.Reflection
{
    public class Instance
    {
        public Type Type { get; set; }

        public object Definition { get; set; }

        public bool IsClass
        {
            get
            {
                return Definition is Class;
            }
        }

        public Class GetClass()
        {
            return Definition as Class;
        }
    }
}