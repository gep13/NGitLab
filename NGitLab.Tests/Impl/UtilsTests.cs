using System.Runtime.Serialization;
using NUnit.Framework;

namespace NGitLab.Tests.Impl
{
    public class UtilsTests
    {
        private enum TestEnum1
        {
            someValue,
            [EnumMember(Value = "a-different-name")]
            SomeOtherValue,
        }

        [Test]
        public void Test_AddParameter_with_enum_value()
        {
            Assert.AreEqual("https://example.com/resource?parameter1=someValue", NGitLab.Impl.Utils.AddParameter("https://example.com/resource", "parameter1", TestEnum1.someValue));
            Assert.AreEqual("https://example.com/resource?parameter1=a-different-name", NGitLab.Impl.Utils.AddParameter("https://example.com/resource", "parameter1", TestEnum1.SomeOtherValue));
        }
    }
}
