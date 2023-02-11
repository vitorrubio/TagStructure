using SharpTags;

namespace Dominio
{
    public class Produto
    {
        public Produto()
        {
            Nome = string.Empty;
            Tags = new Tags();
        }


        public string Nome { get; set; }
        public Tags Tags { get; set; } 
    }
}