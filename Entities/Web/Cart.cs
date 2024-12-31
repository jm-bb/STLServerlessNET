using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Entities.Web;

[Table("cart")]
[Index("OrderId", Name = "order_id")]
[Index("ProdId", Name = "prod_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class Cart
{
    [Key]
    [Column("cart_id", TypeName = "int(11)")]
    public int CartId { get; set; }

    [Column("order_id", TypeName = "int(11)")]
    public int OrderId { get; set; }

    [Column("prod_id", TypeName = "int(11)")]
    public int ProdId { get; set; }

    [Column("prod_sku")]
    [StringLength(20)]
    public string ProdSku { get; set; } = null!;

    [Column("quantity", TypeName = "int(11)")]
    public int Quantity { get; set; }

    [Column("purchase_price")]
    public float? PurchasePrice { get; set; }

    [Column("saved_price")]
    public float? SavedPrice { get; set; }

    [Column("attributes", TypeName = "text")]
    public string? Attributes { get; set; }

    [Column("line_item_discounts")]
    [StringLength(250)]
    public string? LineItemDiscounts { get; set; }

    [Column("line_item_discounted_amount")]
    public float? LineItemDiscountedAmount { get; set; }

    [Column("aplus_warranty", TypeName = "enum('Y','N')")]
    public string AplusWarranty { get; set; } = null!;

    [Column("create_date", TypeName = "timestamp")]
    public DateTime? CreateDate { get; set; }

    [Column("fishbowl_rule_id", TypeName = "int(11)")]
    public int FishbowlRuleId { get; set; }

    public Order Order { get; set; }
    public Product Product { get; set; }
}
