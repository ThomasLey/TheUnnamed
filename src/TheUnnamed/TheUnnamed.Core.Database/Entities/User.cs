using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheUnnamed.Core.Database.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public Guid Uuid { get; set; } = Guid.Empty;
    public string UniqueName { get; set; } = null!;
    public string DisplayName { get; set; } = null!;

    public IList<Document> Documents { get; set; }
}