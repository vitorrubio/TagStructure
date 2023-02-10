# Struct para gerenciar Tags no C#

Ontem eu tentei fazer um tipo no C# para servir como tags para os produtos / serviços da minha empresa, e que pudesse ser usada em qualquer classe que precisasse te tags e que pudesse ser lido/gravado no banco de dados usando EF como se fosse uma string normal.
Queria que atendesse aos seguintes requisitos:
1. mantivesse uma lista de strings únicas e em minúscula
2. fosse conversível para string (com o ToString mas implementamos também overload de conversão implícita) retornando a lista de tags únicas em minúsculas, separadas por vírgula e ordenadas.
3. fosse conversível DE string
4. Se comportasse como string de todas as formas
5. Se parecesse com um tipo nativo do .Net
6. Fosse no meu domínio um value object
7. Fosse compatível com o EF

Um disclaimer aqui: essa classe não vai contar as tags para medir relevância ou fazer tag cloud. 

Quero deixar claro que meu primeiro código está muito longe de estar correto, na verdade está um lixo, não o use. Tem várias coisas absolutamente erradas e é um bom exemplo de como mesmo devs experientes podem cometer grandes erros em coisas simples. 

O meu primeiro código eu fiz a classe Product, a struct Tags (gostaria de insistir em struct por enquanto) e 4 testes unitários. 
Dois testes falharam e dois passaram, e eu fiquei intrigado com o motivo que levou dois deles a falharem e propus o desafio ontem no [replit](https://replit.com/)
Vou postar aqui o código errado, os primeiros acertos que fiz e o repositório no github com o código correto. No repositório do github teremos várias branches com os nomes iteracao1, iteracao2 e assim por diante para você poder ver a evolução no código com o passar do tempo. Na Main/Master teremos a última versão do código. 

Abaixo o código da iteração ZERO. 

```
///ATENÇÃO: CÓDIGO REDONDAMENTE ERRADO PARA FINS DIDÁTICOS
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

### Iteração 1

Mudamos a List<string> interna para um HashSet<string> porque o hashset já garante a unicidade das tags. Fizemos a renomeação de algumas vairáveis, e mais 9 testes. 3 de criação, 3 de add e 3 de remove.
Deixamos a HashSet<string> como readonly, para não mudarmos sua instância, mas mesmo assim ela (e todo o restante), é mutável. 
Deixamos a ordenação só para a saída ToString. 
Já podemos criar Tags a partir de strings usando um dos constructores ou convertê-las para strings, mas ainda não podemos simplesmente atribuir um objeto tags a uma string, ou uma string ao Tags. Também não temos o que é recomendável pela microsoft: Override de Equals, GetHashCode, etc.
Igualdade entre tags com o mesmo conteúdo, como se fossem um record, Equal(), GetHashCode(), ==, nada disso está funcionando.



### Iteração 2

Adicionamos mais esses testes ao que foi feito na iteração 1 e todos passaram:

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

Criamos testes que falham com certeza, alguns deles nem compilam por isso a parte que não compila está comentada para vermos os outros falharem:
```
        [TestMethod]
        public void TagsWithSameContentsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            //Assert.IsTrue(tags1 == tags2); //não compila
        }

        [TestMethod]
        public void SameTagsShouldBeEquals()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = tags1;
            Assert.AreEqual(tags1, tags2);
            Assert.IsTrue(tags1.Equals(tags2));
            //Assert.IsTrue(tags1 == tags2); //não compila
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
            //Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); //não compila
        }
```

### Iteração 3
Os testes que criamos falharam porque não temos override do Equals, nem do GetHashCode, ou do operador == 

Também precisamos dar uma arrumada na casa, está tudo em um arquivo só porque fizemos no replit, mas está na hora de separar o projeto de teste do restante, a Tags para uma biblioteca e Product para uma suposta aplicação. 
Também estou passando o nome de todas as classes de domínio para o português para fins didáticos, e passando a Tags para uma biblioteca chamada SharpTags para simular uma biblioteca de terceiros (que é a maneira como outros a usariam)
Criamos a biblioteca SharpTags
Renomeamos TagStructureTest para SharpTagsTest
Ficamos com a estrutura:
```
TagStructureTest
 ├─> Dominio
 │   ├── Dominio.csproj
 │   └── Produto.cs
 ├─> SharpTags
 │   ├── SharpTags.csproj
 │   └── Tags.cs
 ├─> TagStructureTest
 │   ├── TagsTest.cs
 │   └── TagStructureTest.csproj
 ├── TagStructureTest.sln
 ```

 Adicionamos Override do Equals e do GetHashCode, que sempre devem ser implementados juntos. 
 O Override do GetHashCode eu simplesmente aproveitei que já temos um ToString e uso a hashcode que seria gerada para sua string. Não acredito que precisamos de algo melhor que isso por enquanto.
 Já o Equals, primeiro ele verifica se dois objetos são o mesmo objeto/instância e retorna true, caso contrário verifica se o objeto sendo comparado é null e retorna false, por último ele vê se as duas strings resultantes são iguais, retornando esse resultado.
 Não vamos entrar em detalhes sobre o GetHashCode, ele é um algritmo que gera um número inteiro único para um objeto e é usado para otimizar a performance ao armazenar esse objeto em hashes, como listas do tipo HashSet e Dictionary, fazendo com que sejam armazenados como se fosse em um vetor indexado numericamente (usando esse número gerado como índice) para evitar colisões e aumentar a performance em casoss de listas muito grandes. 
 A regra mais simples é: se dois objetos são iguais então seus hashes devem ser iguais. Se você fez o override de Equals é obrigado a fazer o override de GetHashCode.
 Se você estivesse trabalhando com entidades aqui para serem persistidas em banco de dados com nHibernate ou EF, você faria o GetHashCode ser o próprio Id, e faria o Equals ser baseado no próprio Id também. 

 - [Definição e Guidelines para implementar Equals](https://learn.microsoft.com/en-us/dotnet/api/system.object.equals?view=net-7.0)
 - [Implementação correta de GetHashCode](https://learn.microsoft.com/en-us/dotnet/api/system.object.gethashcode?view=net-7.0)
 - [Usar o Visual Studio para gerar o Equals e o GetHashCode](https://learn.microsoft.com/en-us/visualstudio/ide/reference/generate-equals-gethashcode-methods?view=vs-2022)
 - [Discussão interessante sobre GetHashCode no Stack Overflow](https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-overriding-gethashcode)

 Minha implementação:
 ```
         public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            if ((!(obj is Tags)) && (!(obj is string)))
            {
                return false;
            }

            return this.ToString().Equals(obj.ToString());
        }
 ```

 Quase todos passaram exceto os ainda comentados e o teste `Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);`


### Iteração 4

Para iniciar a iteração 4 vamos fazer o operator overloading dos sinais == e !=
> Operator overloading é um tipo de método que sobrecarrega ou motifica o comportamento de operadores. São úteis para quando precisamos que uma classe ou até uma struct se comporte como um tipo de dado especial até nos momentos em que usamos == ou !=. Por exemplo, nos objetos Produto os operadores == e != só comparam as referências e não o conteúdo. Nós alteraremos esse comportamento em Tags porque queremos que o conteúdo das tags seja considerado. 
Veja também
- [Operator Overloading](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/operator-overloading)
- [Operators](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#1410-operators)
- [Operator Overloading](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/expressions#1143-operator-overloading)
- [== Operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/equality-operators#equality-operator-)
- [!= Operator](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/equality-operators#inequality-operator-)

Algumas mudanças foram feitas porque estamos tratando de igualdade e override de operators em um Value Type e não em um Reference Type. 
Isso torna algumas coisas mais simples embora outras precisem de mais cuidados. 
O trecho de código abaixo podemos tirar:
```
            if (object.ReferenceEquals(this, obj))
            {
                return true;
            }
```
Com Value Types não precisamos lidar com Reference Equals nem com nulidade.

Os operadores == e != implementados. Veja que o != é facil, porque ele é a negação do ==.

```
        public static bool operator ==(Tags esquerda, Tags direita)
        {
            return esquerda.Equals(direita);
        }

        public static bool operator !=(Tags esquerda, Tags direita) => !(esquerda == direita);
```

Agora podemos descomentar as linhas:

```
Assert.IsTrue(tags1 == tags2);
```

mas a `Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); ` ainda não compila, segura ela comentada.
Podemos colocar esses métodos em outros testes também. 
O teste TagsShouldBeEqualsToString continua não passando, porque compara com string, ainda não implementamos isso. 

Acrescentei  mais 4 testes:
```
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
            tags2.AddTags("Teste");
            Assert.IsTrue(tags1 != tags2); 
        }

        [TestMethod]
        public void IneEqualityOperatorSameContentsTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag6");
            Assert.IsTrue(tags1 != tags2); 
        }
```

E não estranhamente o InequalityOperatorSameVarTest não passou. Porque? Porque ambos estão compartilhando a mesma  HashSet<string> _taglist, por referência, que está sendo mudada pelo método AddTags. Podemos mudar esse método para criar uma nova, mas isso fará novamente com que outros testes, principalmente de retorno de funções e getters retornando Tags (como no caso de Produto), falhem.

A solução é transformar a classe em imutável de vez. Isso vai envolver bastante energia por isso deixaremos para a iteração 5.