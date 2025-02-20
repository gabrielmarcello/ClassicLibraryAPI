namespace ClassicLibraryAPI.Dtos {
    public class UserForLoginConfirmationDTO {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public UserForLoginConfirmationDTO() {
            if (PasswordHash == null) {
                PasswordHash = new byte[0];
            }
            if (PasswordSalt == null) {
                PasswordSalt = new byte[0];
            }
        }
    }
}
