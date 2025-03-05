namespace ClassicLibraryAPI.Models {
    public class ItemCarrinho {
        public Book Livro { get; set; }
        public int Quantidade { get; set; }

        public ItemCarrinho(Book livro, int quantidade) {
            Livro = livro;
            Quantidade = quantidade;
        }
    }
}
