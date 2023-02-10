using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace TagStructureTest
{

    public struct Tags
    {
        //errado (segundo erro: melhor que os membros de uma struct sejam readonly/imutáveis)
        //private List<string> _taglist = new List<string>();

        //certo 
        //private readonly List<string> _taglist = new List<string>();

        //melhor ainda: troque por hashset
        private readonly HashSet<string> _taglist = new HashSet<string>();


        //atenção para os construtores: structs sem construtores não inicializam corretamente seus membros reference types
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
            //para segurança, invariantes etc
            if (t != null && t.Length > 0)
            {

                ////errado (terceiro erro: não mude / troque as referencias dos reference types internos (não crie / nobrescreva novos objetos com new / tolist etc)
                //var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct().ToList();
                //this._taglist = this._taglist.Union(tagsToAdd).Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToList();

                ////correção
                //var current = _taglist.ToArray();
                //var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct().ToArray();
                //_taglist.Clear();
                //_taglist.AddRange(current.Union(tagsToAdd).Select(x => x.Trim().ToLower()).Distinct().ToList());

                //melhor: use hashset que ele garante que só existe um de cada e faz a operação mudando o proprio hashset com UnionWith
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
            //para segurança, invariantes etc
            if (t != null && t.Length > 0)
            {
                ////errado  (terceiro erro: não mude / troque as referencias dos reference types internos (não crie / nobrescreva novos objetos com new / tolist etc)
                //var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct();
                //_taglist = _taglist.Except(tagsToRemove).Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToList();

                ////correção
                //var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct();
                //_taglist.RemoveAll(x => tagsToRemove.Contains(x));

                //melhor ainda: use hashset e subtraia as tags com ExceptWith
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
        //ou seja, getters ou métodos que retornem Tags sempre vão fazer uma cópia da sua estrutura inadvertidamente. E você estará trabalhando com outra variável. 


        //public Tags Tags; // ==> talvez pudesse resolver mas depende do seu modelo ...
        /*
         *  Getters são métodos embutidos, e um value type sempre é retornado de um 
         *  método como cópia (um novo) e não como referência (como os outros objetos), 
         *  por isso usando um getter ele cria outro Tags, que não é o que você estava 
         *  mexendo. Então é essencial que esse cara seja um field. Melhor adicionar 
         *  métodos para usar ele e não chamar ele através de um getter 
         *  (isso é encapsulamento). Encapsule todas as chamadas a dependencias de seus 
         *  objetos para não encadear chamadas as dependências expostas. Veja a lei de 
         *  demeter. https://pt.wikipedia.org/wiki/Lei_de_Demeter
         */

        //melhor ainda: 
        //private readonly Tags tags;
        //crie métodos para interagir com as tags do produto (adicionar/remover) sem retornar a variável Tags (para que não seja feita inadvertidadmente uma cópia da estrutura)
    }



    [TestClass]
    public class TagsTest
    {
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


    }
}