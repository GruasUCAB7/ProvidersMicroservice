namespace ProvidersMS.src.Cranes.Application.Commands.UpdateCrane.Types
{
    public record UpdateDriverResponse
    (
        string Id,
        string DNI,
        bool IsActiveLicensed,
        List<string> ImagesDocuments,
        string CraneAssigned
    );
}
