namespace ProvidersMS.src.Cranes.Application.Exceptions
{
    public class CraneAlreadyExistException(string plate) : ApplicationException($"Crane with plate {plate} already exist")
    {
    }
}
