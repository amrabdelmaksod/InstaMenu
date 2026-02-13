namespace InstaMenu.Application.Index.DTOs
{
    public record IndexViewModel
    {
        // ===== Summary Cards =====
        public decimal TotalOrdersAmount { get; init; }        // مجموع الطلبات (ج.م)
        public int OrdersCount { get; init; }                  // عدد الطلبات
        public int MenuVisitsCount { get; init; }              // عدد زيارات القائمة

        // ===== Top Sections =====
        public IReadOnlyList<TopSectionDto> TopSections { get; init; }
            = new List<TopSectionDto>();

        // ===== Most Ordered Items =====
        public IReadOnlyList<TopItemDto> MostOrderedItems { get; init; }
            = new List<TopItemDto>();

        // ===== Most Visited Items =====
        public IReadOnlyList<TopItemDto> MostVisitedItems { get; init; }
            = new List<TopItemDto>();
    }
    public record TopSectionDto
    {
        public Guid SectionId { get; init; }
        public string Name { get; init; } = default!;
        public int VisitsCount { get; init; }
    }

    public record TopItemDto
    {
        public Guid ItemId { get; init; }
        public string Name { get; init; } = default!;
        public int Count { get; init; }     // عدد الطلبات أو الزيارات
    }
}
