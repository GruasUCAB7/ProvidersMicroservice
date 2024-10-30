namespace ProvidersMS.src.Cranes.Application.Exceptions
{
    public class CraneAlreadyExistException : ApplicationException
    {
        public CraneAlreadyExistException(string plate) 
            : base($"Crane with plate {plate} already exist") 
        { 
        }
    }
}
