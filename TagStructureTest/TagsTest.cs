using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace TagStructureTest
{

    public struct Tags
    {

        //melhor ainda: troque por hashset
        private readonly HashSet<string> _taglist = new HashSet<string>();


        //aten��o para os construtores: structs sem construtores n�o inicializam corretamente seus membros reference types
        public Tags()
        {
        }

        public Tags(IEnumerable<string>? tags) : this(tags?.ToArray())
        {

        }

        public Tags(Tags? tags) : this(tags?.GetTags())
        {

        }

        public Tags(params string[]? t)
        {
            if (t != null)
            {
                this._taglist.UnionWith(t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()));
            }
        }

        public override string ToString()
        {
            return string.Join(",", _taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
        }


        public void AddTags(IEnumerable<string>? t)
        {
            this.AddTags(t?.ToArray());
        }

        public void AddTags(Tags? t)
        {
            this.AddTags(t?.GetTags());
        }



        public void AddTags(params string[]? t)
        {
            //para seguran�a, invariantes etc
            if (t != null && t.Length > 0)
            {
                //melhor: use hashset que ele garante que s� existe um de cada e faz a opera��o mudando o proprio hashset com UnionWith
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                _taglist.UnionWith(tagsToAdd);

            }


        }


        public void RemoveTags(IEnumerable<string>? t)
        {
            this.RemoveTags(t?.ToArray());
        }


        public void RemoveTags(Tags? tags)
        {
            this.RemoveTags(tags?.GetTags());
        }



        public void RemoveTags(params string[]? t)
        {
            //para seguran�a, invariantes etc
            if (t != null && t.Length > 0)
            {
                //melhor: use hashset e subtraia as tags com ExceptWith
                var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                _taglist.ExceptWith(tagsToRemove);

            }

        }



        public string[] GetTags()
        {
            return this._taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToArray();
        }
    }


    public class Product
    {
        public Product()
        {
            Name = string.Empty;
            Tags = new Tags();
        }


        public string Name { get; set; }
        public Tags Tags { get; set; } //aqui o primeiro erro. (que explica o seu principal problema de referencia):
        //ou seja, getters ou m�todos que retornem Tags sempre v�o fazer uma c�pia da sua estrutura inadvertidamente. E voc� estar� trabalhando com outra vari�vel. 


        //public Tags Tags; // ==> talvez pudesse resolver mas depende do seu modelo ...
        /*
         *  Getters s�o m�todos embutidos, e um value type sempre � retornado de um 
         *  m�todo como c�pia (um novo) e n�o como refer�ncia (como os outros objetos), 
         *  por isso usando um getter ele cria outro Tags, que n�o � o que voc� estava 
         *  mexendo. Ent�o � essencial que esse cara seja um field. Melhor adicionar 
         *  m�todos para usar ele e n�o chamar ele atrav�s de um getter 
         *  (isso � encapsulamento). Encapsule todas as chamadas a dependencias de seus 
         *  objetos para n�o encadear chamadas as depend�ncias expostas. Veja a lei de 
         *  demeter. https://pt.wikipedia.org/wiki/Lei_de_Demeter
         */

        //melhor ainda: 
        //private readonly Tags tags;
        //crie m�todos para interagir com as tags do produto (adicionar/remover) sem retornar a vari�vel Tags (para que n�o seja feita inadvertidadmente uma c�pia da estrutura)
    }



    [TestClass]
    public class TagsTest
    {

        #region testes b�sicos passando na itera��o1

        [TestMethod]
        public void TagsMustHaveCombinationOfUniqueTags()
        {

            Tags tags = new Tags();
            tags.AddTags("tag1, tag2, tag3");
            tags.AddTags("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }


        [TestMethod]
        public void CanRemoveTags()
        {

            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags.RemoveTags("tag1");
            tags.RemoveTags(new Tags("tag5"));
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());


        }


        [TestMethod]
        public void ProductMustHaveCombinationOfUniqueTags()
        {


            Product prod = new Product();
            prod.Tags.AddTags("tag1, tag2, tag3");
            prod.Tags.AddTags("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", prod.Tags.ToString());



        }


        [TestMethod]
        public void CanRemoveTagsFromProduct()
        {

            Product prod = new Product();
            prod.Tags.AddTags("tag1,tag2,tag3,tag4,tag5");
            prod.Tags.RemoveTags("tag1");
            prod.Tags.RemoveTags(new Tags("tag5"));
            Assert.AreEqual("tag2,tag3,tag4", prod.Tags.ToString());


        }

        #endregion


        #region testes novos criados na iteracao 1 (constructor, add, remove)
        [TestMethod]
        public void CanCreateTagsFromList()
        {
            Tags tags = new Tags(new List<string> { "tag1", "tag2", "tag3" });
            tags.AddTags(new List<string> { "tag4", "tag5", "tag3" });
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanCreateTagsFromString()
        {
            Tags tags = new Tags("tag1, tag2, tag3");
            tags.AddTags("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanCreateTagsFromTags()
        {
            Tags tags = new Tags();
            tags.AddTags(new Tags("tag1, tag2, tag3"));
            tags.AddTags(new Tags("tag4, tag5, tag3"));
            Tags newTags = new Tags(tags);
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", newTags.ToString());
        }

        [TestMethod]
        public void CanAddTagsFromList()
        {
            Tags tags = new Tags();
            tags.AddTags(new List<string> { "tag1", "tag2", "tag3" });
            tags.AddTags(new List<string> { "tag4", "tag5", "tag3" });
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanAddTagsFromString()
        {
            Tags tags = new Tags();
            tags.AddTags("tag1, tag2, tag3");
            tags.AddTags("tag4, tag5, tag3");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }

        [TestMethod]
        public void CanAddTagsFromTags()
        {
            Tags tags = new Tags();
            tags.AddTags(new Tags("tag1, tag2, tag3"));
            tags.AddTags(new Tags("tag4, tag5, tag3"));
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags.ToString());
        }


        [TestMethod]
        public void CanRemoveTagsFromList()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags.RemoveTags(new List<string> { "tag1", "tag5"});
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }

        [TestMethod]
        public void CanRemoveTagsFromString()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags.RemoveTags("tag1, tag5");
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }

        [TestMethod]
        public void CanRemoveTagsFromTags()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags.RemoveTags(new Tags("tag1, tag5"));
            Assert.AreEqual("tag2,tag3,tag4", tags.ToString());
        }

        #endregion


        #region testes de igualdade 

        [TestMethod]
        public void TagsWithSameContentsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            //Assert.IsTrue(tags1 == tags2); //n�o compila
        }

        [TestMethod]
        public void SameTagsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            //Assert.IsTrue(tags1 == tags2); //n�o compila
        }



        [TestMethod]
        public void TagsWithSameContentsShouldHaveSameHashcodeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual(tags1.GetHashCode(), tags2.GetHashCode());

        }

        [TestMethod]
        public void SameTagsShouldHaveSameHashcodeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.AreEqual(tags1.GetHashCode(), tags2.GetHashCode());
        }


        [TestMethod]
        public void TagsShouldBeEqualsToString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);
            Assert.IsTrue(tags1.Equals("tag1,tag2,tag3,tag4,tag5"));
            //Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); //n�o compila
        }

        #endregion

    }
}