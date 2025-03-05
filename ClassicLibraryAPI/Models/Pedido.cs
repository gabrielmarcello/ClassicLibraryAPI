namespace ClassicLibraryAPI.Models {
    public class Pedido {
        public int Id { get; set; }
        public User Cliente { get; set; }
        public List<ItemCarrinho> Itens { get; set; }
        public decimal Total { get; set; }
        public string StatusPagamento { get; set; }

        public Pedido(int id, User cliente, List<ItemCarrinho> itens, decimal total) {
            Id = id;
            Cliente = cliente;
            Itens = itens;
            Total = total;
            StatusPagamento = "Pendente";
        }
    }
}
