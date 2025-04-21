namespace API.Helpers;
public interface IRandomNumberGeneratorService
{
    byte[] GenerateBytes(int length);
}
