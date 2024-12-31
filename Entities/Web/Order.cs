using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STLServerlessNET.Entities.Web;

[Table("orders")]
[Index("OrderId", Name = "order_id")]
public partial class Order
{
    [Key]
    [Column("order_id", TypeName = "int(11)")]
    public int OrderId { get; set; }

    [Column("user_id", TypeName = "int(11)")]
    public int UserId { get; set; }

    [Column("order_type", TypeName = "enum('public','dealer','po','credit','template')")]
    public string OrderType { get; set; } = null!;

    [Column("first_name")]
    [StringLength(35)]
    public string FirstName { get; set; } = null!;

    [Column("last_name")]
    [StringLength(35)]
    public string LastName { get; set; } = null!;

    [Column("suffix")]
    [StringLength(70)]
    public string? Suffix { get; set; }

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("company")]
    [StringLength(35)]
    public string? Company { get; set; }

    [Column("phone")]
    [StringLength(25)]
    public string Phone { get; set; } = null!;

    [Column("phone_extension")]
    [StringLength(25)]
    public string PhoneExtension { get; set; } = null!;

    [Column("bill_street")]
    [StringLength(125)]
    public string BillStreet { get; set; } = null!;

    [Column("bill_apt")]
    [StringLength(125)]
    public string BillApt { get; set; } = null!;

    [Column("bill_apt_suffix")]
    [StringLength(125)]
    public string? BillAptSuffix { get; set; }

    [Column("bill_city")]
    [StringLength(80)]
    public string BillCity { get; set; } = null!;

    [Column("bill_state")]
    [StringLength(10)]
    public string BillState { get; set; } = null!;

    [Column("bill_zip")]
    [StringLength(15)]
    public string BillZip { get; set; } = null!;

    [Column("bill_country")]
    [StringLength(5)]
    public string BillCountry { get; set; } = null!;

    [Column("billing_id", TypeName = "int(11)")]
    public int BillingId { get; set; }

    [Column("attention")]
    [StringLength(35)]
    public string? Attention { get; set; }

    [Column("ship_first_name")]
    [StringLength(35)]
    public string? ShipFirstName { get; set; }

    [Column("ship_last_name")]
    [StringLength(35)]
    public string? ShipLastName { get; set; }

    [Column("ship_apt")]
    [StringLength(35)]
    public string? ShipApt { get; set; }

    [Column("ship_apt_suffix")]
    [StringLength(80)]
    public string? ShipAptSuffix { get; set; }

    [Column("ship_street")]
    [StringLength(125)]
    public string ShipStreet { get; set; } = null!;

    [Column("ship_city")]
    [StringLength(80)]
    public string ShipCity { get; set; } = null!;

    [Column("ship_state")]
    [StringLength(10)]
    public string ShipState { get; set; } = null!;

    [Column("ship_zip")]
    [StringLength(15)]
    public string ShipZip { get; set; } = null!;

    [Column("ship_country")]
    [StringLength(5)]
    public string ShipCountry { get; set; } = null!;

    [Column("shipping_id", TypeName = "int(11)")]
    public int ShippingId { get; set; }

    [Column("cc_type")]
    [StringLength(30)]
    public string? CcType { get; set; }

    [Column("cc_name")]
    [StringLength(80)]
    public string? CcName { get; set; }

    [Column("cc_num")]
    [StringLength(50)]
    public string? CcNum { get; set; }

    [Column("cc_exp")]
    [StringLength(10)]
    public string CcExp { get; set; } = null!;

    [Column("cc_code")]
    [StringLength(5)]
    public string? CcCode { get; set; }

    [Column("active", TypeName = "enum('Y','N')")]
    public string Active { get; set; } = null!;

    [Column("shipped", TypeName = "enum('Y','N')")]
    public string Shipped { get; set; } = null!;

    [Column("shipping_method", TypeName = "text")]
    public string? ShippingMethod { get; set; }

    [Column("shipping_price")]
    public float ShippingPrice { get; set; }

    [Column("sales_tax")]
    public float SalesTax { get; set; }

    [Column("charity_org")]
    [StringLength(120)]
    public string CharityOrg { get; set; } = null!;

    [Column("charity_amount")]
    public float CharityAmount { get; set; }

    [Column("sub_total")]
    public float SubTotal { get; set; }

    [Column("order_total")]
    public float OrderTotal { get; set; }

    [Column("complete", TypeName = "enum('Y','N')")]
    public string Complete { get; set; } = null!;

    [Column("submitted", TypeName = "enum('Y','N')")]
    public string Submitted { get; set; } = null!;

    [Column("notes", TypeName = "text")]
    public string? Notes { get; set; }

    [Column("hear_aboutus")]
    [StringLength(80)]
    public string? HearAboutus { get; set; }

    [Column("occupation")]
    [StringLength(80)]
    public string? Occupation { get; set; }

    [Column("fishbowl_order_id", TypeName = "int(11)")]
    public int? FishbowlOrderId { get; set; }

    [Column("signature_delivery")]
    public float SignatureDelivery { get; set; }

    [Column("coupon_code", TypeName = "text")]
    public string? CouponCode { get; set; }

    [Column("coupon_discount", TypeName = "text")]
    public string? CouponDiscount { get; set; }

    [Column("coupon_discount_amount")]
    public float? CouponDiscountAmount { get; set; }

    [Column("payment_method", TypeName = "enum('CreditCard','GiftCard','PayPal','PickUp','PO','Credit','GooglePay','ApplePay','AmazonPay')")]
    public string? PaymentMethod { get; set; }

    [Column("partial_payment", TypeName = "enum('Y','N')")]
    public string? PartialPayment { get; set; }

    [Column("giftcard_number")]
    [StringLength(50)]
    public string? GiftcardNumber { get; set; }

    [Column("giftcard_number2")]
    [StringLength(50)]
    public string? GiftcardNumber2 { get; set; }

    [Column("giftcard_number3")]
    [StringLength(50)]
    public string? GiftcardNumber3 { get; set; }

    [Column("giftcard_payment")]
    public float? GiftcardPayment { get; set; }

    [Column("giftcard_payment2")]
    public float? GiftcardPayment2 { get; set; }

    [Column("giftcard_payment3")]
    public float? GiftcardPayment3 { get; set; }

    [Column("paypal_payment")]
    public float? PaypalPayment { get; set; }

    [Column("creditcard_payment")]
    public float? CreditcardPayment { get; set; }

    [Column("googlepay_payment")]
    public float GooglepayPayment { get; set; }

    /// <summary>
    /// transactionID;AuthCode
    /// </summary>
    [Column("googlepay_trans_details")]
    [StringLength(100)]
    public string GooglepayTransDetails { get; set; } = null!;

    [Column("applepay_payment")]
    public float ApplepayPayment { get; set; }

    /// <summary>
    /// transactionID;AuthCode
    /// </summary>
    [Column("applepay_trans_details")]
    [StringLength(100)]
    public string ApplepayTransDetails { get; set; } = null!;

    [Column("amazon_order_id")]
    [StringLength(20)]
    public string? AmazonOrderId { get; set; }

    [Column("amazon_access_token", TypeName = "text")]
    public string AmazonAccessToken { get; set; } = null!;

    [Column("amazonpay_payment")]
    public float AmazonpayPayment { get; set; }

    [Column("amazonpay_trans_details")]
    [StringLength(50)]
    public string? AmazonpayTransDetails { get; set; }

    [Column("contains_custom_lightbar", TypeName = "tinyint(4)")]
    public sbyte? ContainsCustomLightbar { get; set; }

    [Column("contains_bundle", TypeName = "tinyint(4)")]
    public sbyte? ContainsBundle { get; set; }

    [Column("create_date", TypeName = "timestamp")]
    public DateTime? CreateDate { get; set; }

    [Column("update_date", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("payment_date", TypeName = "datetime")]
    public DateTime? PaymentDate { get; set; }

    [Column("points_earned", TypeName = "int(11)")]
    public int? PointsEarned { get; set; }

    [Column("points_claimed", TypeName = "int(11)")]
    public int? PointsClaimed { get; set; }

    [Column("reminder_sent", TypeName = "enum('Y','N','U')")]
    public string? ReminderSent { get; set; }

    [Column("reminder_second_sent", TypeName = "enum('Y','N','U')")]
    public string? ReminderSecondSent { get; set; }

    [Column("feedback_sent", TypeName = "enum('Y','N','U')")]
    public string? FeedbackSent { get; set; }

    [Column("interested_sent", TypeName = "enum('Y','N','U','NA')")]
    public string? InterestedSent { get; set; }

    [Column("phone_order", TypeName = "enum('Y','N')")]
    public string? PhoneOrder { get; set; }

    [Column("already_placed_order", TypeName = "enum('Y','N')")]
    public string? AlreadyPlacedOrder { get; set; }

    [Column("manual_email_sent", TypeName = "enum('Y','N')")]
    public string? ManualEmailSent { get; set; }

    [Column("referred_by", TypeName = "int(11)")]
    public int? ReferredBy { get; set; }

    [Column("referral_reward_points", TypeName = "int(11)")]
    public int? ReferralRewardPoints { get; set; }

    [Column("in_premises", TypeName = "tinyint(4)")]
    public sbyte? InPremises { get; set; }

    [Column("priority_processing", TypeName = "tinyint(4)")]
    public sbyte? PriorityProcessing { get; set; }

    [Column("partial_payment_reminder_sent", TypeName = "enum('Y','N')")]
    public string? PartialPaymentReminderSent { get; set; }

    [Column("is_credit_app", TypeName = "enum('Y','N')")]
    public string? IsCreditApp { get; set; }

    [Column("cavv")]
    [StringLength(50)]
    public string? Cavv { get; set; }

    [Column("eci_flag")]
    [StringLength(50)]
    public string? EciFlag { get; set; }

    [StringLength(5)]
    public string? Enrolled { get; set; }

    [Column("PAResStatus")]
    [StringLength(5)]
    public string? ParesStatus { get; set; }

    [StringLength(5)]
    public string? SignatureVerification { get; set; }

    [Column("xid")]
    [StringLength(5)]
    public string? Xid { get; set; }

    [Column("aav")]
    [StringLength(5)]
    public string? Aav { get; set; }

    [Column("loaded_from_order", TypeName = "int(11)")]
    public int? LoadedFromOrder { get; set; }

    [Column("ip_address")]
    [StringLength(50)]
    public string? IpAddress { get; set; }

    [Column("tracking_num", TypeName = "text")]
    public string? TrackingNum { get; set; }

    [Column("picked_up", TypeName = "text")]
    public string? PickedUp { get; set; }

    [Column("unpaid")]
    public float? Unpaid { get; set; }

    public User? User { get; set; }
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
}