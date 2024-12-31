using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace STLServerlessNET.Entities.Web;

[Table("product")]
[Index("Active", "ProductType", Name = "active_product_type")]
[Index("LightbarType", Name = "lightbar_type")]
[Index("OffroadCategory", Name = "offroad_category")]
[Index("Slug", Name = "slug")]
[Index("SlugNew", Name = "slug_new")]
public partial class Product
{
    [Key]
    [Column("prod_id", TypeName = "int(11)")]
    public int ProdId { get; set; }

    [Column("replacement_cat_id", TypeName = "int(11)")]
    public int? ReplacementCatId { get; set; }

    [Column("prod_name", TypeName = "tinytext")]
    public string ProdName { get; set; } = null!;

    [Column("custom_h2", TypeName = "tinytext")]
    public string? CustomH2 { get; set; }

    [Column("prod_short_description", TypeName = "tinytext")]
    public string? ProdShortDescription { get; set; }

    [Column("prod_description", TypeName = "text")]
    public string? ProdDescription { get; set; }

    [Column("prod_features", TypeName = "text")]
    public string? ProdFeatures { get; set; }

    [Column("prod_sku")]
    [StringLength(25)]
    public string? ProdSku { get; set; }

    [Column("prod_url")]
    [StringLength(225)]
    public string? ProdUrl { get; set; }

    [Column("stock_count", TypeName = "int(11)")]
    public int? StockCount { get; set; }

    [Column("stock_sold", TypeName = "int(11)")]
    public int? StockSold { get; set; }

    [Column("list_price_global")]
    public float? ListPriceGlobal { get; set; }

    [Column("list_price")]
    public float? ListPrice { get; set; }

    [Column("sale_price")]
    public float SalePrice { get; set; }

    [Column("cent_discount")]
    public float CentDiscount { get; set; }

    [Column("active", TypeName = "enum('Y','N')")]
    public string Active { get; set; } = null!;

    [Column("taxable", TypeName = "enum('Y','N')")]
    public string Taxable { get; set; } = null!;

    [Column("weight")]
    public float? Weight { get; set; }

    [Column("height")]
    public float? Height { get; set; }

    [Column("width")]
    public float? Width { get; set; }

    [Column("length")]
    public float? Length { get; set; }

    [Column("brochure", TypeName = "text")]
    public string? Brochure { get; set; }

    [Column("brochure_thumbnail")]
    [StringLength(175)]
    public string? BrochureThumbnail { get; set; }

    [Column("instructions")]
    [StringLength(175)]
    public string? Instructions { get; set; }

    [Column("bracket_chart")]
    [StringLength(175)]
    public string? BracketChart { get; set; }

    [Column("flash_pattern")]
    [StringLength(175)]
    public string? FlashPattern { get; set; }

    [Column("specifications_image")]
    [StringLength(175)]
    public string? SpecificationsImage { get; set; }

    [Column("visor_instructions")]
    [StringLength(175)]
    public string? VisorInstructions { get; set; }

    [Column("product_type", TypeName = "enum('apparel','public','havis','reconditioned','clearance','giftcard','package','vehicles')")]
    public string? ProductType { get; set; }

    [Column("show_admin", TypeName = "enum('Y','N')")]
    public string? ShowAdmin { get; set; }

    [Column("create_date", TypeName = "datetime")]
    public DateTime CreateDate { get; set; }

    [Column("video_mini_thumbnail")]
    [StringLength(255)]
    public string? VideoMiniThumbnail { get; set; }

    [Column("video_main_thumbnail")]
    [StringLength(255)]
    public string? VideoMainThumbnail { get; set; }

    [Column("video_youtube_widget", TypeName = "text")]
    public string? VideoYoutubeWidget { get; set; }

    [Column("update_date", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("dealer_list_price")]
    public float? DealerListPrice { get; set; }

    [Column("dealer_sale_price")]
    public float? DealerSalePrice { get; set; }

    [Column("dealer_international_sale_price")]
    public float? DealerInternationalSalePrice { get; set; }

    [Column("dealer_international_list_price")]
    public float? DealerInternationalListPrice { get; set; }

    [Column("dealerActive", TypeName = "enum('Y','N')")]
    public string? DealerActive { get; set; }

    [Column("patent_num", TypeName = "text")]
    public string? PatentNum { get; set; }

    [Column("sae_certification_num")]
    [StringLength(50)]
    public string? SaeCertificationNum { get; set; }

    [Column("rohs_certification_num")]
    [StringLength(50)]
    public string? RohsCertificationNum { get; set; }

    [Column("lightbar_type", TypeName = "enum('NO_LIGHTBAR','4_HEAD','4_HEAD_MULTICOLOR','6_HEAD','6_HEAD_MULTI','6_HEAD_MULTICOLOR','8_HEAD','8_HEAD_MULTI','8_HEAD_MULTICOLOR','ECLIPSE','ECLIPSE_MINI','MINI_BASIC_RAPTAR','MINI_BASIC_OCTO','MINI_PRIME_18','MINI_KFORCE_18','MINI_PRIME_BASIC_27','MINI_PRIME_27','MINI_KFORCE_27','MINI_CUST_18','FULL_LIGHTBAR_18','FULL_LIGHTBAR_KFORCE_18','FULL_LIGHTBAR_22','FULL_LIGHTBAR_KFORCE_22','FULL_LIGHTBAR_26','FULL_LIGHTBAR_KFORCE_26','FULL_LIGHTBAR_30','FULL_LIGHTBAR_KFORCE_30','FULL_LIGHTBAR_34','FULL_LIGHTBAR_KFORCE_34','SPLIT_ECLIPSE','FULL_LIGHTBAR_ARIES_18','FULL_LIGHTBAR_ARIES_MULTI_18','FULL_LIGHTBAR_ARIES_22','FULL_LIGHTBAR_ARIES_MULTI_22','FULL_LIGHTBAR_ARIES_26','FULL_LIGHTBAR_ARIES_MULTI_26','MICRO_BASIC_12','FULL_LIGHTBAR_MICRO_26','FULL_LIGHTBAR_MICRO_30','ARIES_MINI_18','ARIES_MINI_27','FULL_LIGHTBAR_PRIME_TOW_26','FULL_LIGHTBAR_KFORCE_TOW_26','FULL_LIGHTBAR_ARIES_TOW_26','FULL_LIGHTBAR_MICRO_TOW_30','SPLIT_ECLIPSE_X','MICRO_BASIC_21','REAR_SPLIT_RAPTOR','CeptorX_4_HEAD','CeptorX_4_STD_HEAD','CeptorX_6_HEAD','CeptorX_6_STD_HEAD','STD_RAPTOR','FULL_LIGHTBAR_STD_22','FULL_LIGHTBAR_KFORCE_TOW_63','SPLIT_ECLIPSE_MULTICOLOR','SPLIT_ECLIPSE_X_MULTICOLOR','STD_SPLIT_ECLIPSE_MULTICOLOR','MINI_PRIME_18_MULTICOLOR','MICRO_BASIC_12_MULTICOLOR','MINI_PRIME_27_MULTICOLOR','MICRO_BASIC_21_MULTICOLOR','FULL_LIGHTBAR_18_MULTICOLOR','FULL_LIGHTBAR_22_MULTICOLOR','FULL_LIGHTBAR_26_MULTICOLOR','FULL_LIGHTBAR_STD_22_MULTICOLOR','FULL_LIGHTBAR_MICRO_26_MULTICOLOR')")]
    public string? LightbarType { get; set; }

    [Column("lightbar_price")]
    [Precision(10, 2)]
    public decimal? LightbarPrice { get; set; }

    [Column("dealer_lightbar_price")]
    [StringLength(50)]
    public string? DealerLightbarPrice { get; set; }

    [Column("group", TypeName = "enum('G1','G2','G4','G6','G10','G15','G20')")]
    public string? Group { get; set; }

    [Column("tab_id", TypeName = "int(11)")]
    public int? TabId { get; set; }

    [Column("series_id", TypeName = "int(11)")]
    public int? SeriesId { get; set; }

    [Column("flat_shipping_fee")]
    public float? FlatShippingFee { get; set; }

    [Column("video_thumbnail_image")]
    [StringLength(350)]
    public string? VideoThumbnailImage { get; set; }

    [Column("interactive_thumbnail_image")]
    [StringLength(350)]
    public string? InteractiveThumbnailImage { get; set; }

    [Column("interactive_player", TypeName = "text")]
    public string? InteractivePlayer { get; set; }

    [Column("total_quantity", TypeName = "int(20)")]
    public int? TotalQuantity { get; set; }

    [Column("attribute")]
    [StringLength(100)]
    public string? Attribute { get; set; }

    [Column("sort_order", TypeName = "int(11)")]
    public int? SortOrder { get; set; }

    [Column("slug")]
    [StringLength(100)]
    public string? Slug { get; set; }

    [Column("meta_title")]
    [StringLength(60)]
    public string? MetaTitle { get; set; }

    [Column("meta_keywords", TypeName = "text")]
    public string? MetaKeywords { get; set; }

    [Column("meta_description", TypeName = "text")]
    public string? MetaDescription { get; set; }

    [Column("og_title")]
    [StringLength(60)]
    public string? OgTitle { get; set; }

    [Column("og_keywords", TypeName = "text")]
    public string? OgKeywords { get; set; }

    [Column("og_description", TypeName = "text")]
    public string? OgDescription { get; set; }

    [Column("og_image")]
    [StringLength(255)]
    public string? OgImage { get; set; }

    [Column("slug_new")]
    [StringLength(100)]
    public string? SlugNew { get; set; }

    [Column("category_for_url", TypeName = "int(11)")]
    public int? CategoryForUrl { get; set; }

    [Column("amazon_up_charge")]
    public float? AmazonUpCharge { get; set; }

    [Column("amazon_display", TypeName = "enum('Y','N')")]
    public string? AmazonDisplay { get; set; }

    [Column("ebay_up_charge")]
    public float? EbayUpCharge { get; set; }

    [Column("ebay_display", TypeName = "enum('Y','N')")]
    public string? EbayDisplay { get; set; }

    [Column("show_in_stock", TypeName = "enum('Y','N')")]
    public string? ShowInStock { get; set; }

    [Column("video_thumbnail_2")]
    [StringLength(150)]
    public string? VideoThumbnail2 { get; set; }

    [Column("hover_image")]
    [StringLength(150)]
    public string? HoverImage { get; set; }

    [Column("fishbowl_rule_id", TypeName = "int(11)")]
    public int FishbowlRuleId { get; set; }

    [Column("cree_flag")]
    [StringLength(3)]
    public string CreeFlag { get; set; } = null!;

    [Column("warranty_flag")]
    [StringLength(3)]
    public string WarrantyFlag { get; set; } = null!;

    [Column("warranty_2_flag")]
    [StringLength(3)]
    public string Warranty2Flag { get; set; } = null!;

    [Column("moneyback_flag")]
    [StringLength(3)]
    public string MoneybackFlag { get; set; } = null!;

    [Column("freeshipping_flag")]
    [StringLength(3)]
    public string FreeshippingFlag { get; set; } = null!;

    [Column("freeshipping_over_flag")]
    [StringLength(3)]
    public string FreeshippingOverFlag { get; set; } = null!;

    [Column("shipping_per_bar")]
    [StringLength(3)]
    public string ShippingPerBar { get; set; } = null!;

    [Column("aplus_customer_support")]
    [StringLength(3)]
    public string AplusCustomerSupport { get; set; } = null!;

    [Column("hour_48_processing")]
    [StringLength(3)]
    public string Hour48Processing { get; set; } = null!;

    [Column("n_packs", TypeName = "enum('None','Bundles','2 pack','3 pack','4 pack','6 pack','8 pack','10 pack')")]
    public string NPacks { get; set; } = null!;

    [Column("featured_product", TypeName = "enum('Y','N')")]
    public string FeaturedProduct { get; set; } = null!;

    [Column("skip_dealer_discount", TypeName = "enum('Y','N')")]
    public string SkipDealerDiscount { get; set; } = null!;

    [Column("offroad_category")]
    [StringLength(250)]
    public string? OffroadCategory { get; set; }

    [Column("gif_heading")]
    [StringLength(100)]
    public string? GifHeading { get; set; }

    [Column("gif_image")]
    [StringLength(250)]
    public string? GifImage { get; set; }

    [Column("whats_included", TypeName = "text")]
    public string? WhatsIncluded { get; set; }

    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
