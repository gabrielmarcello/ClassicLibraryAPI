namespace ClassicLibraryAPI.Dtos {
    public class UserForRegistrationDTO {
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Adress { get; set; }

        public UserForRegistrationDTO() {
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
