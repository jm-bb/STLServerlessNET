using System.ComponentModel.DataAnnotations.Schema;

namespace STLServerlessNET.Entities.Service;

[Table("orders")]
public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public string OrderType { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Suffix { get; set; }

    public string Email { get; set; } = null!;

    public string? Company { get; set; }

    public string Phone { get; set; } = null!;

    public string PhoneExtension { get; set; } = null!;

    public string BillStreet { get; set; } = null!;

    public string BillApt { get; set; } = null!;

    public string? BillAptSuffix { get; set; }

    public string BillCity { get; set; } = null!;

    public string BillState { get; set; } = null!;

    public string BillZip { get; set; } = null!;

    public string BillCountry { get; set; } = null!;

    public int BillingId { get; set; }

    public string? Attention { get; set; }

    public string? ShipFirstName { get; set; }

    public string? ShipLastName { get; set; }

    public string? ShipApt { get; set; }

    public string? ShipAptSuffix { get; set; }

    public string ShipStreet { get; set; } = null!;

    public string ShipCity { get; set; } = null!;

    public string ShipState { get; set; } = null!;

    public string ShipZip { get; set; } = null!;

    public string ShipCountry { get; set; } = null!;

    public int ShippingId { get; set; }

    public string? CcType { get; set; }

    public string? CcName { get; set; }

    public string? CcNum { get; set; }

    public string CcExp { get; set; } = null!;

    public string? CcCode { get; set; }

    public string Active { get; set; } = null!;

    public string Shipped { get; set; } = null!;

    public string? ShippingMethod { get; set; }

    public float ShippingPrice { get; set; }

    public float SalesTax { get; set; }

    public string CharityOrg { get; set; } = null!;

    public float CharityAmount { get; set; }

    public float SubTotal { get; set; }

    public float OrderTotal { get; set; }

    public string Complete { get; set; } = null!;

    public string Submitted { get; set; } = null!;

    public string? Notes { get; set; }

    public string? HearAboutus { get; set; }

    public string? Occupation { get; set; }

    public int? FishbowlOrderId { get; set; }

    public float SignatureDelivery { get; set; }

    public string? CouponCode { get; set; }

    public string? CouponDiscount { get; set; }

    public float? CouponDiscountAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PartialPayment { get; set; }

    public string? GiftcardNumber { get; set; }

    public string? GiftcardNumber2 { get; set; }

    public string? GiftcardNumber3 { get; set; }

    public float? GiftcardPayment { get; set; }

    public float? GiftcardPayment2 { get; set; }

    public float? GiftcardPayment3 { get; set; }

    public float? PaypalPayment { get; set; }

    public float? CreditcardPayment { get; set; }

    public float GooglepayPayment { get; set; }

    /// <summary>
    /// transactionID;AuthCode
    /// </summary>
    public string GooglepayTransDetails { get; set; } = null!;

    public float ApplepayPayment { get; set; }

    /// <summary>
    /// transactionID;AuthCode
    /// </summary>
    public string ApplepayTransDetails { get; set; } = null!;

    public string? AmazonOrderId { get; set; }

    public string AmazonAccessToken { get; set; } = null!;

    public float AmazonpayPayment { get; set; }

    public string? AmazonpayTransDetails { get; set; }

    public sbyte? ContainsCustomLightbar { get; set; }

    public sbyte? ContainsBundle { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? PointsEarned { get; set; }

    public int? PointsClaimed { get; set; }

    public string? ReminderSent { get; set; }

    public string? ReminderSecondSent { get; set; }

    public string? FeedbackSent { get; set; }

    public string? InterestedSent { get; set; }

    public string? PhoneOrder { get; set; }

    public string? AlreadyPlacedOrder { get; set; }

    public string? ManualEmailSent { get; set; }

    public int? ReferredBy { get; set; }

    public int? ReferralRewardPoints { get; set; }

    public sbyte? InPremises { get; set; }

    public sbyte? PriorityProcessing { get; set; }

    public string? PartialPaymentReminderSent { get; set; }

    public string? IsCreditApp { get; set; }

    public string? Cavv { get; set; }

    public string? EciFlag { get; set; }

    public string? Enrolled { get; set; }

    public string? ParesStatus { get; set; }

    public string? SignatureVerification { get; set; }

    public string? Xid { get; set; }

    public string? Aav { get; set; }

    public int? LoadedFromOrder { get; set; }

    public string? IpAddress { get; set; }

    public string? TrackingNum { get; set; }

    public string? PickedUp { get; set; }

    public float? Unpaid { get; set; }

    public virtual User User { get; set; } = null!;
}
