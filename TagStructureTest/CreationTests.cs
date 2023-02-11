using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTags;

namespace TagStructureTest
{
    [TestClass]
    public class CreationTests
    {
        [TestMethod]
        public void CanCreateTagsFromList()
        {
            Tags tags = new Tags(new List<string> { "tag1", "tag2", "tag3" });
            tags = tags.Add(new List<string> { "tag4", "tag5", "tag3" });
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanCreateTagsFromString()
        {
            Tags tags = new Tags("tag1, tag2, tag3");
            tags = tags.Add("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanCreateTagsFromTags()
        {
            Tags tags = new Tags();
            tags = tags.Add(new Tags("tag1, tag2, tag3"));
            tags = tags.Add(new Tags("tag4, tag5, tag3"));
            Tags newTags = new Tags(tags);
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", newTags.ToString());
        }
    }
}
