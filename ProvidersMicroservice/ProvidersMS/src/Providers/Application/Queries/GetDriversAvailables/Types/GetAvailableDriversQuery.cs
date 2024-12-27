namespace ProvidersMS.src.Providers.Application.Queries.GetDriversAvailables.Types
{
    public record GetAvailableDriversQuery
    (
        int PerPage,
        int Page,
        string? IsAvailable
    );
}
