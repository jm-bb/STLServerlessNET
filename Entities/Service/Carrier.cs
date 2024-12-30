using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("carrier")]
public partial class Carrier
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("activeFlag")]
    public ulong? ActiveFlag { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("readOnly")]
    public ulong? ReadOnly { get; set; }

    [Column("scac")]
    public string? Scac { get; set; }
}
