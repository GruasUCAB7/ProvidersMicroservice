namespace ProvidersMS.src.Cranes.Application.Queries.GetAll.Types
{
    public record GetAllCranesQuery
    (
        int PerPage,
        int Page,
        string? IsActive
    );
}
