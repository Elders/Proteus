
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elders.Proteus.Surrogate;
using Machine.Specifications;

namespace Elders.Proteus.Tests.Proxy
{
    public class BaseClass
    {
        public BaseClass()
        {
            BaseProperty = "base value";
        }
        private string BaseProperty { get; set; }
    }
    public class ToProxy : BaseClass
    {
        public string TestProperty { get; set; }
    }
    [Subject(typeof(When_creating_a_proxy))]
    public class When_creating_a_proxy
    {
        Establish context = () =>
        {
            proxyType = ProxyBuilder.BuildProxy(typeof(ToProxy), Guid.NewGuid());
            instance = Activator.CreateInstance(proxyType) as IDynamicProxy;
            toProxy = new ToProxy();
            toProxy.TestProperty = "test value";

            //  toProxy.

        };
        Because of_deserialization = () => { instance.Store(toProxy); };
        static ToProxy toProxy;
        static IDynamicProxy instance;
        static Type proxyType;
         It should_not_be_null_the_proxy_should_work = () => { instance.GetType().GetProperties().Where(x => x.Name == "TestProperty").First().GetGetMethod().Invoke(instance, new object[] { }).ShouldEqual(toProxy.TestProperty); };

        //It should_base_classes_should_work = () => { instance.GetType().GetProperties().Where(x => x.Name == "BaseProperty").First().GetGetMethod().Invoke(instance, new object[] { }).ShouldEqual("base value"); };


    }
}
