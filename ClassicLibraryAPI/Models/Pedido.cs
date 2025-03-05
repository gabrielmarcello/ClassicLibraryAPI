namespace ClassicLibraryAPI.Models {
    public class Pedido {
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public List<ItemCarrinho> Itens { get; set; }
        public decimal Total { get; set; }
        public string StatusPagamento { get; set; }

        public Pedido(int idPedido, int idCliente, List<ItemCarrinho> itens) {
            IdPedido = idPedido;
            IdCliente = idCliente;
            Itens = itens;
            Total = CalcularTotalPedido();
            StatusPagamento = "Pendente";
        }

        private decimal CalcularTotalPedido() {
            decimal total = 0;
            foreach (var item in Itens) {
                total += (decimal)item.Livro.Preco;
            }
            return total;
        }
    }
}
