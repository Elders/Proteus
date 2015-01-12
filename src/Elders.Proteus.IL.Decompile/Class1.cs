using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Elders.Proteus.IL.Decompile
{
    public interface IDynamicProxy
    {
        void Store(object holder);
        object Restore();
    }
    public class CustomeClass
    {
        public int aww { get; set; }
    }
    public class Implementer : IDynamicProxy
    {
        public CustomeClass holder;
        public void Store(CustomeClass holder)
        {
            this.holder = holder;
        }

        public object Restore()
        {
            throw new NotImplementedException();
        }
        public int aww
        {
            set
            {
                holder.aww = value;
            }
        }
        Func<object, object> delegatessss;
        public void Store(object holder)
        {
            delegatessss.Invoke(holder);
        }
    }
}
