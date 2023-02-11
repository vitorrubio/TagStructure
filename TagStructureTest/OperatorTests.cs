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
    public class OperatorTests
    {
        [TestMethod]
        public void TagsShouldBeEqualsToStringUsingEqualityOperators()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5");
        }

        [TestMethod]
        public void TagsShouldBeNotEqualsToStringUsingEqualityOperators()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1 != "tag1,tag2,tag4,tag5");
        }

        [TestMethod]
        public void EqualityOperatorSameVarTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.IsTrue(tags1 == tags2);
        }

        [TestMethod]
        public void EqualityOperatorSameContentsTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1 == tags2);
        }

        [TestMethod]
        public void InequalityOperatorSameVarTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            tags1 = tags2.Add("Teste");
            Assert.IsTrue(tags1 != tags2);
        }

        [TestMethod]
        public void IneEqualityOperatorSameContentsTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag6");
            Assert.IsTrue(tags1 != tags2);
        }

        [TestMethod]
        public void CanAddTags()
        {
            Tags tags = new Tags("tag1, tag2, tag3");
            tags += new Tags("tag4, tag5, tag3");
            tags += "tag9, tag5, tag0";
            Assert.AreEqual("tag0,tag1,tag2,tag3,tag4,tag5,tag9", tags.ToString());
        }

        [TestMethod]
        public void CanSubtractTags()
        {
            Tags tags = new Tags("tag0,tag1,tag2,tag3,tag4,tag5,tag9");
            tags -= new Tags("tag3");
            Assert.AreEqual("tag0,tag1,tag2,tag4,tag5,tag9", tags.ToString());
        }
    }
}
