using Dominio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTags;
using System.Numerics;

namespace TagStructureTest
{
    [TestClass]
    public class TagsTest
    {

        #region testes básicos passando na iteração1

        [TestMethod]
        public void TagsMustHaveCombinationOfUniqueTags()
        {
            Tags tags = new Tags();
            tags = tags.Add("tag1, tag2, tag3");
            tags = tags.Add("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }


        [TestMethod]
        public void CanRemoveTags()
        {

            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags = tags.Remove("tag1");
            tags = tags.Remove(new Tags("tag5"));
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());


        }


        [TestMethod]
        public void ProductMustHaveCombinationOfUniqueTags()
        {


            Produto prod = new Produto();
            prod.Tags = prod.Tags.Add("tag1, tag2, tag3");
            prod.Tags = prod.Tags.Add("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", prod.Tags.ToString());



        }


        [TestMethod]
        public void CanRemoveTagsFromProduct()
        {

            Produto prod = new Produto();
            prod.Tags = prod.Tags.Add("tag1,tag2,tag3,tag4,tag5");
            prod.Tags = prod.Tags.Remove("tag1");
            prod.Tags = prod.Tags.Remove(new Tags("tag5"));
            Assert.AreEqual("tag2,tag3,tag4", prod.Tags.ToString());


        }

        #endregion

    }
}