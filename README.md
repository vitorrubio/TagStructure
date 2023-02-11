# Struct para gerenciar Tags no C#

## Objetivo
Ontem eu tentei fazer um tipo no C# para servir como tags para os produtos / serviços da minha empresa, e que pudesse ser usada em qualquer classe que precisasse te tags e que pudesse ser lido/gravado no banco de dados usando EF como se fosse uma string normal.
Queria que atendesse aos seguintes requisitos:
1. mantivesse uma lista de strings únicas e em minúscula
2. fosse conversível para string (com o ToString mas implementamos também overload de conversão implícita) retornando a lista de tags únicas em minúsculas, separadas por vírgula e ordenadas.
3. fosse conversível DE string
4. Se comportasse como string de todas as formas
5. Se parecesse com um tipo nativo do .Net
6. Fosse no meu domínio um value object
7. Fosse compatível com o EF


## Disclaimers

Um disclaimer aqui: essa classe não vai contar as tags para medir relevância ou fazer tag cloud. 

Não use esse código em um projeto em produção (pelo menos não ainda, não em um grande)

Quero deixar claro que meu primeiro código está muito longe de estar correto, na verdade está um lixo, não o use. Tem várias coisas absolutamente erradas e é um bom exemplo de como mesmo devs experientes podem cometer grandes erros em coisas simples. 


---

## História

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


### Iteração 5
Adicionei overlod para os operadores de conversão implícita string => tags e tags => string.

```
        public static implicit operator string(Tags t) => t.ToString();

        public static implicit operator Tags(string s) => new Tags(s);
```
E Deu pau boniiiiito.
> Muito cuidado ao brincar com overload de operadores.

Primeiro de tudo, o que são conversões implícitas? São conversões sem perda de dados, de tipos compatíveis, que podem ser feitas diretamente sem colocar o tipo na frente. 
Por exemplo:
```
byte a = 5;
int b = a;
```
Essa é uma conversão implícita. Você pode jogar um byte em um int, porque ele cabe, e ele também é um "tipo do mesmo tipo". Ele é um número inteiro. Só que menor. Ele pode ser convertido direto, é o mesmo tipo de dado. 
Tá, mas e uma conversão explícita? É o tipo de conversão que você tem que forçar com o nome do outro tipo na frente, porque os tipos são quase incompatíveis, ou compatíveis até certo ponto, e você vai perder dados. Por exemplo na conversão abaixo:
```
double a = 5.5;
int b = (int)a;
```
Nessa conversão eu tenho que forçar o tipo double a "caber" no int e pra isso eu perco informação, eu perco o .5.


No nosso caso, eu quero que Tags seja conversível para string. Assim de uma forma que se eu jogar uma string em uma tag ele crie a tag automaticamente sem dar new, e se eu jogar uma tag em uma string ele converta automaticamente sem eu ter que chamar o .ToString().

Muito ousado? 

O que eu quero é fazer isso ser legal e compilável:

```
Tags a = "vitor,teste";
string b = a;
```

E porque está dando vários erros do tipo 
> Error	CS0121	The call is ambiguous between the following methods or properties: 'Tags.RemoveTags(Tags?)' and 'Tags.RemoveTags(params string[]?)'	TagStructureTest	C:\Users\vitor\OneDrive\Desktop\Labs\TagStructureTest\TagStructureTest\TagsTest.cs	32	N/A

Onde está a ambiguidade? Em todos os métodos que eu aceito Tags como parâmetro de entrada mas que também tem um overload que aceite string está dando ambiguidade. Por quê? Porque o compilador não sabe se `RemoveTags(new Tags("tag5"));`é pra chamar o RemoveTags de string ou o RemoveTags de Tags convertento pra string .... já que agora Tags é legalmente conversível pra string ... 

Complicado? Muito. 

Vamos retirar todos os métodos que aceitam Tags como argumento, e seus testes. Vamos também renomear AddTags para Add e RemoveTags para Remove.

Magicamente removendo esses métodos ele compila, e agora temos menos métodos para dar manutenção, testar e documentar.

Rodamos os testes e todos os testes rodaram exceto o `InequalityOperatorSameVarTest`, que já esperávamos, pois Tags ainda não é imutável, e o teste
```
        [TestMethod]
        public void TagsShouldBeEqualsToString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1.Equals("tag1,tag2,tag3,tag4,tag5"));
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);
            //Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); //não compila
        }
```
Falha na linha `Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);`

Estrano isso, mas vamos descomentar  linha Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5") e ver se ela passa, vamos também olhar a documentação do Assert.AreEqual e debugar. Vamos também dividir isso aí em 3 testes.

```
        [TestMethod]
        public void TagsShouldBeEqualsToString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1.Equals("tag1,tag2,tag3,tag4,tag5"));
        }

        [TestMethod]
        public void TagsShouldBeEqualsToStringUsingEqualityOperators()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); 
        }

        [TestMethod]
        public void ATagsVarShouldBeEqualsToASameContentString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);
        }
```

Compila e `Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1);` continua falhando. 

Inspecionando o método Assert.AreEqual encontramos a resposta:
```
        //
        // Summary:
        //     Tests whether the specified objects are equal and throws an exception if the
        //     two objects are not equal. *Different numeric types are treated as unequal even
        //     if the logical values are equal. 42L is not equal to 42.*
        //
        // Parameters:
        //   expected:
        //     The first object to compare. This is the object the tests expects.
        //
        //   actual:
        //     The second object to compare. This is the object produced by the code under test.
        //
        // Exceptions:
        //   T:Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException:
        //     Thrown if expected is not equal to actual.
        public static void AreEqual(object expected, object actual)
```
Ou seja, Assert.AreEqual considera o mesmo tipo e mesmo valor. Ele falha pra um int e um byte por exemplo. Isso está na documentação.
- [Método Assert.AreEqual](https://learn.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.assert.areequal?redirectedfrom=MSDN&view=visualstudiosdk-2022#overloads)

Então vamos trocar essa linha para  `Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1.ToString());` porque nós queremos ver se o valor das duas strings geradas é o mesmo. 

Funcionou. Agora vamos para a iteração 6 onde deixamos isso tudo verdadeiramente imutável. 


### Iteração 6

Lembrando que cada interação minha é um ajuste ou refactoring para fazer funcionar um dos testes (e as vezes testes adicionais) e que eu estou colocando cada uma em uma branch. 
Eu não colocaria cada um em uma branch num projeto da vida real mas com certeza teria um commit bem explicado para cada um. 
Vamos começar fazendo o seguinte: Mudar o HashSet<string> pra um ImmutableHashSet<string>. Isso faz com que não possamos mais usar os métodos UnionWith e ExceptWith. Então teriamos que criar novos em vez de mudar seu conteúdo. Mas lemre-se que não devemos criar novos reference types dentro de um field de um calue type. 
Então teremos que fazer com que os métodos Add e Remove retornem um novo Tags com seu conteúdo já preparado (o resultado da fusão dos hashsets). E teremos que mudar a classe Produto também. 
Muita coisa terá que mudar. 
O parameterless constructor (construtor padrão) tem que criar um ImmutableHashSet<string> vazio. O construtor que aceita params string[] deve criar um ImmutableHashSet<string> com essas strings ou vazio. 
Os métodos add e remove devem aproveitar o método new pra criar um novo. O jeito de criar um ImmutableHashSet<string> é meio diferente, você verá. 

Todos os tags.Add e tags.Remove tiveram que mudar para tags = tags.Add e tags = tags.Remove.

```
using System.Collections.Immutable;

namespace SharpTags
{
    public struct Tags
    {
        private readonly ImmutableHashSet<string> _taglist;

        public Tags() : this(null)
        {

        }

        public Tags(IEnumerable<string>? t) : this(t?.ToArray())
        {
           
        }

        public Tags(params string[]? t)
        {
            if (t != null && t.Count() > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower()).ToArray();
                _taglist = ImmutableHashSet.Create<string>(tagsToAdd);
            }
            else
            {
                _taglist = ImmutableHashSet.Create<string>();
            }
        }

        public override string ToString()
        {
            return string.Join(",", _taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct());
        }


        public Tags Add(IEnumerable<string>? t)
        {
            return this.Add(t?.ToArray());
        }

        public Tags Add(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToAdd = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                return new Tags(_taglist.Union(tagsToAdd));
            }

            return new Tags(_taglist);
        }


        public Tags Remove(IEnumerable<string>? t)
        {
            return this.Remove(t?.ToArray());
        }






        public Tags Remove(params string[]? t)
        {
            if (t != null && t.Length > 0)
            {
                var tagsToRemove = t.Where(x => !string.IsNullOrWhiteSpace(x)).SelectMany(x => x!.Split(",")).Select(x => x.Trim().ToLower());
                return new Tags(_taglist.Except(tagsToRemove));
            }
            return new Tags(_taglist);
        }


        public string[] GetTags()
        {
            return this._taglist.Select(x => x.Trim().ToLower()).OrderBy(x => x).Distinct().ToArray();
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object? obj)
        {

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



        public static bool operator ==(Tags esquerda, Tags direita)
        {
            return esquerda.Equals(direita);
        }

        public static bool operator !=(Tags esquerda, Tags direita) => !(esquerda == direita);


        public static implicit operator string(Tags t) => t.ToString();

        public static implicit operator Tags(string s) => new Tags(s);
    }
}
```

Alteramos e rodamos os testes e funcionou

```
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


        #region testes novos criados na iteracao 1 (constructor, add, remove)
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


        [TestMethod]
        public void CanRemoveTagsFromList()
        {
            Tags tags = new Tags("tag1,tag2,tag3,tag4,tag5");
            tags = tags.Remove(new List<string> { "tag1", "tag5"});
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

        #endregion


        #region testes de igualdade 

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
        public void TagsShouldBeEqualsToStringUsingEqualityOperators()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.IsTrue(tags1 == "tag1,tag2,tag3,tag4,tag5"); 
        }

        [TestMethod]
        public void ATagsVarShouldBeEqualsToASameContentString()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Assert.AreEqual("tag1,tag2,tag3,tag4,tag5", tags1.ToString());
        }


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
            tags1 = tags2.Add("Teste");
            Assert.IsTrue(tags1 != tags2); 
        }

        [TestMethod]
        public void IneEqualityOperatorSameContentsTest()
        {
            Tags tags1 = new Tags("tag1,tag2,tag3,tag4,tag5");
            Tags tags2 = new Tags("tag1,tag2,tag3,tag4,tag6");
            Assert.IsTrue(tags1 != tags2); 
        }

        #endregion

    }
}
```

Mas ainda falta testar bem esses dois implicit operator. 
Além disso, e se a gente transformasse cada operação de tags=tags.Add pra tags+= ? E tags.Remove pra tags-= ?
Também precisamos fazer uma limpeza e dividir esses testes talvez em 5 arquivos: Testes de criação, Add, Remove e Gerais. O Arquivo de testes está ficando muito grande.
Além disso está faltando um teste para o método GetTags (que retorna um array de string[]). Esse método não foi nem utilizado e não sei se GetTags é um nome apropriado pra ele. ToArray parece mais apropriado mas eu gostaria de evitar para não dar a impressão de que estamos implementando IEnumerable.
Como nosso conteúdo é imutável, poderíamos guardar ele direto em uma string em vez de um ImmutableHashSet. Assim as consultas a ele, comparações e o ToString poderiam ficar mais performáticos.
Mas se fizermos isso ficaria mais difícil implementar IEnumerable (para navegar entre as tags). Mas será que queremos fazer isso?
Também precisamos decidir se vamos implementar IComparable e IEquatable.