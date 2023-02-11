using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagStructureTest
{
    [TestClass]
    public class ConversionTests
    {
        [TestMethod]
        public void TagsShouldBeConvertibleToString()
        {
            string tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags);
        }

        [TestMethod]
        public void StringShouldBeConvertibleToTags()
        {
            Tags tags = "tag1,tag2,tag3,tag4,tag5";
            Assert.AreEqual(new Tags("tag1,tag2,tag3,tag4,tag5"), tags);
        }
    }
}
