# Struct para gerenciar Tags no C#

Ontem eu tentei fazer um tipo no C# para servir como tags para os produtos / servi�os da minha empresa, e que pudesse ser usada em qualquer classe que precisasse te tags e que pudesse ser lido/gravado no banco de dados usando EF como se fosse uma string normal.
Queria que atendesse aos seguintes requisitos:
1. mantivesse uma lista de strings �nicas e em min�scula
2. fosse convers�vel para string (com o ToString mas implementamos tamb�m overload de convers�o impl�cita) retornando a lista de tags �nicas em min�sculas, separadas por v�rgula e ordenadas.
3. fosse convers�vel DE string
4. Se comportasse como string de todas as formas
5. Se parecesse com um tipo nativo do .Net
6. Fosse no meu dom�nio um value object
7. Fosse compat�vel com o EF

Um disclaimer aqui: essa classe n�o vai contar as tags para medir relev�ncia ou fazer tag cloud. 

Quero deixar claro que meu primeiro c�digo est� muito longe de estar correto, na verdade est� um lixo, n�o o use. Tem v�rias coisas absolutamente erradas e � um bom exemplo de como mesmo devs experientes podem cometer grandes erros em coisas simples. 

O meu primeiro c�digo eu fiz a classe Product, a struct Tags (gostaria de insistir em struct por enquanto) e 4 testes unit�rios. 
Dois testes falharam e dois passaram, e eu fiquei intrigado com o motivo que levou dois deles a falharem e propus o desafio ontem no [replit](https://replit.com/)
Vou postar aqui o c�digo errado, os primeiros acertos que fiz e o reposit�rio no github com o c�digo correto. No reposit�rio do github teremos v�rias branches com os nomes iteracao1, iteracao2 e assim por diante para voc� poder ver a evolu��o no c�digo com o passar do tempo. Na Main/Master teremos a �ltima vers�o do c�digo. 

Abaixo o c�digo da itera��o ZERO. 

```
///ATEN��O: C�DIGO REDONDAMENTE ERRADO PARA FINS DID�TICOS
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TagStructureTest
{

    public struct Tags
    {
        private List<string> tags = new List<string>();

        public Tags() { }
        public Tags(params string[] t)
        {
            if (tags != null)
            {
                this.tags.AddRange(t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
            }
        }

        public override string ToString()
        {
            return string.Join(",", tags.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
        }

        public void AddTags(params string[] t)
        {
            var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct().ToList();
            this.tags = this.tags.Union(tagsToAdd).Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToList();


        }

        public void AddTags(Tags t)
        {
            this.AddTags(t.GetTags());
        }


        public void RemoveTags(params string[] t)
        {
            var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).Distinct();

          tags = tags.Except(tagsToRemove).Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToList();
        }

        public void RemoveTags(Tags tags)
        {
            this.RemoveTags(tags.GetTags());
        }

        public string[] GetTags()
        {
            return this.tags.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToArray();
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
        public Tags Tags { get; init; }
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

            Product prod = new Product { Tags = new Tags("tag1,tag2,tag3,tag4,tag5") };
            prod.Tags.RemoveTags("tag1");
            prod.Tags.RemoveTags(new Tags("tag5"));
            Assert.AreEqual("tag2,tag3,tag4", prod.Tags.ToString());


        }
    }
}
```


## Change Log

### Itera��o 1

Mudamos a List<string> interna para um HashSet<string> porque o hashset j� garante a unicidade das tags. Fizemos a renomea��o de algumas vair�veis, e mais 9 testes. 3 de cria��o, 3 de add e 3 de remove.
Deixamos a HashSet<string> como readonly, para n�o mudarmos sua inst�ncia, mas mesmo assim ela (e todo o restante), � mut�vel. 
Deixamos a ordena��o s� para a sa�da ToString. 
J� podemos criar Tags a partir de strings usando um dos constructores ou convert�-las para strings, mas ainda n�o podemos simplesmente atribuir um objeto tags a uma string, ou uma string ao Tags. Tamb�m n�o temos o que � recomend�vel pela microsoft: Override de Equals, GetHashCode, etc.
Igualdade entre tags com o mesmo conte�do, como se fossem um record, Equal(), GetHashCode(), ==, nada disso est� funcionando.



### Itera��o 2

Adicionamos mais esses testes ao que foi feito na itera��o 1 e todos passaram:

```
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
```

Criamos testes que falham com certeza, alguns deles nem compilam por isso a parte que n�o compila est� comentada para vermos os outros falharem:
```
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
```
