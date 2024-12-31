namespace STLServerlessNET.Models;

public class Coupon
{
    //coupon_id, coupon_code, coupon_message, coupon_type, coupon_discount, apply_to, product_id
    private int _couponId;
    private string _couponName;
    private string _couponDesc;
    private string _couponType;
    private string _couponDiscount;
    private string _applyTo;
    private string _prodId;
    private string _catId;
    public Coupon()
    {

    }

    public Coupon(int coupon_id, string coupon_code, string coupon_message, string coupon_type, string coupon_discount, string apply_to, string product_id, string catId)
    {
        _couponId = coupon_id;
        _couponName = coupon_code;
        _couponDesc = coupon_message;
        _couponType = coupon_type;
        _couponDiscount = coupon_discount;
        _applyTo = apply_to;
        _prodId = product_id;
        _catId = catId;
    }

    public int coupon_id
    {
        get { return _couponId; }
        set { _couponId = value; }
    }

    public string coupon_code
    {
        get { return _couponName; }
        set { _couponName = value; }
    }

    public string coupon_message
    {
        get { return _couponDesc; }
        set { _couponDesc = value; }
    }

    public string coupon_type
    {
        get { return _couponType; }
        set { _couponType = value; }
    }

    public string coupon_discount
    {
        get { return _couponDiscount; }
        set { _couponDiscount = value; }
    }

    public string apply_to
    {
        get { return _applyTo; }
        set { _applyTo = value; }
    }

    public string product_id
    {
        get { return _prodId; }
        set { _prodId = value; }
    }

    public string cat_id
    {
        get { return _catId; }
        set { _catId = value; }
    }
}