namespace Robotic.Forklift.Application.Dtos
{
    public record PageQuery(int Page = 1, int Size = 20, string? SortBy = null, string SortDir = "asc");

    public record PagedResult<T>(
                        int Page,
                        int Size,
                        int TotalItems,
                        IReadOnlyList<T> Items
                    );
}