namespace ClassicLibraryAPI.Models {
    public class User {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Adress { get; set; }

        public User() {
            if (FirstName == null) {
                FirstName = "";
            }
            if (LastName == null) {
                LastName = "";
            }
            if (Email == null) {
                Email = "";
            }
            if (PhoneNumber == null) {
                PhoneNumber = "";
            }
            if (Adress == null) {
                Adress = "";
            }
        }
    }
}
