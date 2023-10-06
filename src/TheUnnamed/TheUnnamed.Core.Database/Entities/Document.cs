using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TheUnnamed.Core.Database.Entities;

public class Document
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid Uuid { get; set; } = Guid.Empty;
    public string Title { get; set; } = null!;
    public string? Version { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Filemap Filemap { get; set; } = null!;

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User Owner { get; set; } = null!;

    public string Hash { get; set; } = null!;
    public string ContentType { get; set; } = null!;

    public ChangeInfo ChangeInfo { get; set; } = null!;
}