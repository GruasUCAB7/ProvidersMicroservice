namespace ProvidersMS.src.Cranes.Application.Queries.GetAll.Types
{
    public record GetAllDriversQuery
    (
        int PerPage,
        int Page,
        string? IsActiveLicensed
    );
}
