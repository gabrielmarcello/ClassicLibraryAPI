namespace ClassicLibraryAPI.Interfaces {
    public interface ICryptographyService {

        byte[] GetPasswordHash(string password, byte[] passwordSalt);

    }
}
