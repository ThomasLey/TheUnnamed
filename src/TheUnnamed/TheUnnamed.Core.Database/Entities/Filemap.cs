using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheUnnamed.Core.Database.Entities;

public class Filemap
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid Uuid { get; set; } = Guid.Empty;

    public Filemap? Parent { get; set; }

    public string Title { get; set; } = null!;
}