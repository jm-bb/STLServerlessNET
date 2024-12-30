using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("zones")]
public class Zone
{
    [Key]
    [Column("zone_id")]
    public int ZoneId { get; set; }

    [Column("country_id")]
    public int CountryId { get; set; }

    [Column("code")]
    public string Code { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("status")]
    public int Status { get; set; }
}
