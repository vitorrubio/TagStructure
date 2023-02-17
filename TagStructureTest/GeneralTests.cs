using Dominio;
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
    public class GeneralTests
    {
        [TestMethod]
        public void DefaultConstructorTest()
        {
            Tags t = new Tags();
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { }, t.GetTags());
            Assert.AreEqual("", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(1, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "teste" }, t.GetTags());
            Assert.AreEqual("teste", t.ToString());
        }

        [TestMethod]
        public void BlankConstructorTest()
        {
            Tags t = new Tags(" ");
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { }, t.GetTags());
            Assert.AreEqual("", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(1, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "teste" }, t.GetTags());
            Assert.AreEqual("teste", t.ToString());
        }

        [TestMethod]
        public void EmptyConstructorTest()
        {
            Tags t = new Tags("");
            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { }, t.GetTags());
            Assert.AreEqual("", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(1, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "teste" }, t.GetTags());
            Assert.AreEqual("teste", t.ToString());
        }

        [TestMethod]
        public void NullConstructorTest()
        {
            string? s0 = null;

            Tags t = new Tags(s0!);

            Assert.IsNotNull(t);
            Assert.AreEqual(0, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { }, t.GetTags());
            Assert.AreEqual("", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(1, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "teste" }, t.GetTags());
            Assert.AreEqual("teste", t.ToString());
        }

        [TestMethod]
        public void StringConstructorTest()
        {
            Tags t = new Tags("Platinum, monitor, xyz");
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "monitor", "platinum", "xyz" }, t.GetTags());
            Assert.AreEqual("monitor,platinum,xyz", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(4, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] {  "monitor", "platinum", "teste", "xyz" }, t.GetTags());
            Assert.AreEqual("monitor,platinum,teste,xyz", t.ToString());
        }

        [TestMethod]
        public void TagsConstructorTest()
        {
            Tags q = new Tags("Platinum, monitor, xyz");
            Tags t = new Tags(q);
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "monitor", "platinum", "xyz" }, t.GetTags());
            Assert.AreEqual("monitor,platinum,xyz", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(4, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "monitor", "platinum", "teste", "xyz" }, t.GetTags());
            Assert.AreEqual("monitor,platinum,teste,xyz", t.ToString());
        }

        [TestMethod]
        public void StringArrayConstructorTest()
        {
            Tags t = new Tags(new string[] { "Gold", "xyz", "MONITOR" });
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "gold", "monitor", "xyz" }, t.GetTags());
            Assert.AreEqual("gold,monitor,xyz", t.ToString());


            t = t.Add(new Tags("teste"));
            Assert.AreEqual(4, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] {  "gold", "monitor", "teste", "xyz" }, t.GetTags());
            Assert.AreEqual("gold,monitor,teste,xyz", t.ToString());
        }

        [TestMethod]
        public void ListStringConstructorTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            Assert.IsNotNull(t);
            Assert.AreEqual(3, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "gold", "monitor", "xyz" }, t.GetTags());
            Assert.AreEqual("gold,monitor,xyz", t.ToString());

            t = t.Add(new Tags("teste"));
            Assert.AreEqual(4, t.GetTags().Length);
            CollectionAssert.AreEqual(new string[] { "gold", "monitor", "teste", "xyz" }, t.GetTags());
            Assert.AreEqual("gold,monitor,teste,xyz", t.ToString());
        }

        [TestMethod]
        public void ToString_Should_Return_Comma_Separated_Values()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            Assert.AreEqual("gold,monitor,xyz", t.ToString());
        }

        [TestMethod]
        public void Two_Tags_With_Same_Content_Should_Be_Equals()
        {
            Tags p = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            Tags q = new Tags(new List<string> { "MONITOR", "golD", "xyz" });

            Assert.AreEqual(p, q);
            Assert.IsTrue(p.Equals(q));
            Assert.IsTrue(q.Equals(p));

            Assert.IsTrue(p.Equals("gold,monitor,xyz"));
            Assert.IsTrue(q.Equals("gold,monitor,xyz"));

            Assert.IsTrue("gold,monitor,xyz".Equals(p));
            Assert.IsTrue("gold,monitor,xyz".Equals(q));
        }


        [TestMethod]
        public void AddTagTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            t = t.Add("Phishing ");
            var expected = "gold,monitor,phishing,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());

        }

        [TestMethod]
        public void AddTagsTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            t = t.Add("Phishing");
            var expected = "gold,monitor,phishing,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());

            t = t.Add("Uno ", "dOs ", "trEs ");
            expected = "dos,gold,monitor,phishing,tres,uno,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());

            t = t.Add(" One,   tWo ", "threE");
            expected = "dos,gold,monitor,one,phishing,three,tres,two,uno,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());
        }


        [TestMethod]
        public void AddTagsObjectTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            Tags q = new Tags(new List<string> { "golD", "Phishing" });
            t = t.Add(q);
            var expected = "gold,monitor,phishing,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());

            t = t.Add(new Tags(new List<string> { "Uno ", "dOs ", "trEs " }));
            expected = "dos,gold,monitor,phishing,tres,uno,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());

            t = t.Add(new Tags(new List<string> { " One,   tWo ", "threE" }));
            expected = "dos,gold,monitor,one,phishing,three,tres,two,uno,xyz";
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());
            Assert.AreEqual(expected, t.ToString());
        }

        [TestMethod]
        public void RemoveTagTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            t = t.Remove("golD ");
            CollectionAssert.AreEqual("monitor,xyz".Split(","), t.GetTags());
        }

        [TestMethod]
        public void RemoveTagsTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            t = t.Remove("golD , monitor");
            CollectionAssert.AreEqual("xyz".Split(","), t.GetTags());

            Tags q = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            q = q.Remove("  golD", "monitor  ");
            CollectionAssert.AreEqual("xyz".Split(","), q.GetTags());
        }

        [TestMethod]
        public void RemoveTagsObjectTest()
        {
            Tags t = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            t = t.Remove(new Tags(new List<string> { "golD , monitor" }));
            CollectionAssert.AreEqual("xyz".Split(","), t.GetTags());

            Tags q = new Tags(new List<string> { "Gold", "xyz", "MONITOR" });
            q = q.Remove(new Tags(new List<string> { "  golD", "monitor  " }));
            CollectionAssert.AreEqual("xyz".Split(","), q.GetTags());
        }


        [TestMethod]
        public void ImplicitStringToTagsTest()
        {
            string tags = "one , two , three";
            Tags t = tags;
            string expected = "one,three,two";
            Assert.AreEqual(expected, t.ToString());
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());

            Assert.IsTrue(t.Equals(t));
            Assert.IsTrue(expected.Equals(t));
            Assert.IsTrue(t.Equals(expected));

#pragma warning disable CS1718 // testing equality override operator
            Assert.IsTrue(t == t);
#pragma warning restore CS1718 // testing equality override operator
            Assert.IsTrue(expected == t);
            Assert.IsTrue(t == expected);
        }

        [TestMethod]
        public void ImplicitTagsToString()
        {
            Tags t = new("one , two , three");
            string expected = "one,three,two";
            string converted = t;

            Assert.AreEqual(expected, converted);
            Assert.AreEqual(converted, expected);

            Assert.IsTrue(expected == converted);
            Assert.IsTrue(converted == expected);
        }


        [TestMethod]
        public void ExplicitStringToTagsTest()
        {
            string tags = "one , two , three";
            Tags t = (Tags)tags;
            string expected = "one,three,two";
            Assert.AreEqual(expected, t.ToString());
            CollectionAssert.AreEqual(expected.Split(","), t.GetTags());

            Assert.IsTrue(t.Equals(t));
            Assert.IsTrue(expected.Equals(t));
            Assert.IsTrue(t.Equals(expected));

#pragma warning disable CS1718 // testing equality override operator
            Assert.IsTrue(t == t);
#pragma warning restore CS1718 // testing equality override operator
            Assert.IsTrue(expected == t);
            Assert.IsTrue(t == expected);
        }


        [TestMethod]
        public void ExplicitTagsToString()
        {
            Tags t = new("one , two , three");
            string expected = "one,three,two";
            string converted = (string)t;

            Assert.AreEqual(expected, converted);
            Assert.AreEqual(converted, expected);

            Assert.IsTrue(expected == converted);
            Assert.IsTrue(converted == expected);
        }




    }
}
