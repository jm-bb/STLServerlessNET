namespace STLServerlessNET.Models
{
    public class Coupon
    {
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
}