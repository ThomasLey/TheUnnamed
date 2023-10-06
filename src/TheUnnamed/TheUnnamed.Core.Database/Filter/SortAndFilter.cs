namespace TheUnnamed.Core.Database.Filter;

public class SortAndFilter
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public IEnumerable<Sort> Sort { get; set; }
    public Filter Filter { get; set; }
}