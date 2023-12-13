using Microsoft.EntityFrameworkCore;

namespace TheUnnamed.Core.Database.Entities;

[Owned]
public class ChangeInfo
{
    public User ModifiedBy { get; init; } = null!;
    public DateTime ModifiedAt { get; init; } = DateTime.MinValue;
}