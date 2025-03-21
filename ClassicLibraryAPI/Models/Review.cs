namespace ClassicLibraryAPI.Models {
    public class Review {

        public int ResenhaId { get; set; }
        public string Titulo { get; set; }
        public string Descricao {  get; set; }
        public int Avaliacao { get; set; }
        public DateTime DataResenha {  get; set; }
        public int UserId { get; set; }
        public int LivroId { get; set; }
    }
}
