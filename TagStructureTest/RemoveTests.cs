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
    public class RemoveTests
    {
        [TestMethod]
        public void CanRemoveTagsFromList()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags = tags.Remove(new List<string> { "tag1", "tag5" });
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }

        [TestMethod]
        public void CanRemoveTagsFromString()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags = tags.Remove("tag1, tag5");
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }

        [TestMethod]
        public void CanRemoveTagsFromTags()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags = tags.Remove(new Tags("tag1, tag5"));
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }
    }
}
