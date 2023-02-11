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
    public class AddTests
    {
        [TestMethod]
        public void CanAddTagsFromList()
        {
            Tags tags = new Tags();
            tags = tags.Add(new List<string> { "tag1", "tag2", "tag3" });
            tags = tags.Add(new List<string> { "tag4", "tag5", "tag3" });
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanAddTagsFromString()
        {
            Tags tags = new Tags();
            tags = tags.Add("tag1, tag2, tag3");
            tags = tags.Add("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanAddTagsFromTags()
        {
            Tags tags = new Tags();
            tags = tags.Add(new Tags("tag1, tag2, tag3"));
            tags = tags.Add(new Tags("tag4, tag5, tag3"));
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }
    }
}
