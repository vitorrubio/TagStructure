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
    public class EqualityTEsts
    {
        [TestMethod]
        public void TagsWithSameContentsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            Assert.IsTrue(tags1 == tags2);
        }

        [TestMethod]
        public void SameTagsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            Assert.IsTrue(tags1 == tags2);
        }

        [TestMethod]
        public void TagsWithSameContentsShouldHaveSameHashcode()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual(tags1.GetHashCode(), tags2.GetHashCode());

        }

        [TestMethod]
        public void SameTagsShouldHaveSameHashcode()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.AreEqual(tags1.GetHashCode(), tags2.GetHashCode());
        }

        [TestMethod]
        public void TagsShouldBeEqualsToString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1.Equals("tag1,tag2,tag3,tag4,tag5"));
        }

        [TestMethod]
        public void ATagsVarShouldBeEqualsToASameContentString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1.ToString());
        }
    }
}
