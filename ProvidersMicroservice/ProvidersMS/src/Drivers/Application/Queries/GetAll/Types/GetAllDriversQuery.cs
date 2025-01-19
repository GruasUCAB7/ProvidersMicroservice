namespace ProvidersMS.src.Drivers.Application.Queries.GetAll.Types
{
    public record GetAllDriversQuery
    (
        int PerPage,
        int Page,
        string? IsActiveLicensed,
        string? IsAvailable
    );
}
