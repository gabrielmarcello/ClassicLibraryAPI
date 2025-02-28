namespace ClassicLibraryAPI.Models {
    public class Book {
        public int Id { get; set; }
        public float Price { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Idiom { get; set; }
        public string Autor { get; set; }
        public string Type { get; set; }
        public int PagesNumber { get; set; }
        public string Publisher { get; set; }
        public DateOnly PublishDate { get; set; }
        public string Dimentions { get; set; }
        public string Image { get; set; }
        public int StockAmount { get; set; }
        public bool Availability { get; set; }
        public string Condition { get; set; }

    }
}
