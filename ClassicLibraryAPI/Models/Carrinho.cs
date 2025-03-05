namespace ClassicLibraryAPI.Models {
    public class Carrinho {
        public List<ItemCarrinho> Itens { get; set; }

        public Carrinho() {
            Itens = new List<ItemCarrinho>();
        }

        public void AdicionarItem(Book livro, int quantidade) {
            var item = Itens.FirstOrDefault(i => i.Livro.LivroId == livro.LivroId);
            if (item == null) {
                Itens.Add(new ItemCarrinho(livro, quantidade));
            }
            else {
                item.Quantidade += quantidade;
            }
        }

        public decimal CalcularTotal() {
            return (decimal)Itens.Sum(i => i.Livro.Preco * i.Quantidade);
        }
    }
}
