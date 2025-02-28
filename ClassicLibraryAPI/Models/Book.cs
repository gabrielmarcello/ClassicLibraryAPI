namespace ClassicLibraryAPI.Models {
    public class Book {
        public int LivroId { get; set; }
        public float Preco { get; set; }
        public string Titulo { get; set; }
        public string Idioma { get; set; }
        public string NomeCategoria { get; set; }
        public string NomeAutor { get; set; }
        public string Tipo { get; set; }
        public int Numero_Paginas { get; set; }
        public string Editora { get; set; }
        public DateOnly DataPublicacao { get; set; }
        public string Dimensao { get; set; }
        public string Imagem { get; set; }
        public int Quantidade_Estoque { get; set; }
        public bool Disponibilidade { get; set; }
        public string Condicao { get; set; }

    }
}
