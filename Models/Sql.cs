struct SqlQueries
{
    public const string EligibleOrder = @"select t1.order_id, t1.order_type, t1.user_id, t1.first_name, t1.last_name, t2.fishbowl_id, t1.email, t1.company, t1.phone, t1.bill_street, t1.bill_apt, t1.bill_apt_suffix, t1.bill_city, t1.bill_state,
                                         t1.bill_zip, t1.bill_country, t1.ship_first_name, t1.ship_last_name, t1.ship_street, t1.ship_apt, t1.ship_apt_suffix, t1.ship_city, t1.ship_state, t1.ship_zip, t1.ship_country, t1.cc_type,
                                         t1.cc_name, t1.active, t1.shipped, t1.shipping_method, t1.shipping_price, t1.sales_tax, t1.sub_total, t1.order_total, t1.complete, t1.fishbowl_order_id, t1.signature_delivery, t1.coupon_code, 
                                         t1.coupon_discount, t1.referred_by, t1.priority_processing, t1.contains_custom_lightbar, t1.points_earned, t1.points_claimed, t1.create_date, t1.attention, t1.payment_method, t1.giftcard_number, t1.giftcard_payment,
                                         t1.giftcard_number2, t1.giftcard_payment2, t1.giftcard_number3, t1.giftcard_payment3, t1.ship_apt, t2.tax_exempt, t1.Enrolled, t1.PAResStatus, t1.SignatureVerification, t1.loaded_from_order, t1.suffix from orders t1 left join
                                         users t2 on t2.user_id=t1.user_id where t1.order_id in (_ORDER_ID_) and t1.order_type='public' and t1.active='y' and t1.order_id not in (select distinct order_id from lightbar_selections where order_id is not null);";

    public const string EligibleCustomer = @"select u.user_id, u.user_type, u.user_email, u.user_password, u.user_password_last_1, u.user_password_last_2, u.password_last_updated, u.first_name, u.last_name, u.suffix, u.company, u.phone, u.phone_extension,
                                            IF(u.billing_id=0, u.bill_street, ub.bill_street) as bill_street, IF(u.billing_id=0, u.bill_apt, ub.bill_apt) as bill_apt, IF(u.billing_id=0, u.bill_apt_suffix, ub.bill_apt_suffix) as bill_apt_suffix,
                                            IF(u.billing_id=0, u.bill_city, ub.bill_city) as bill_city, IF(u.billing_id=0,u.bill_state, ub.bill_state) as bill_state, IF(u.billing_id=0,u.bill_zip, ub.bill_zip) as bill_zip, IF(u.billing_id=0, u.bill_country,
                                            ub.bill_country) as bill_country, u.billing_id, u.attention, IF(u.shipping_id=0, u.ship_apt, us.ship_apt) as ship_apt, IF(u.shipping_id=0, u.ship_apt_suffix, us.ship_apt_suffix) as ship_apt_suffix,
                                            IF(u.shipping_id=0, u.ship_street, us.ship_street) as ship_street, IF(u.shipping_id=0, u.ship_city, us.ship_city) as ship_city, IF(u.shipping_id=0, u.ship_state, us.ship_state) as ship_state, IF(u.shipping_id=0,
                                            u.ship_zip, us.ship_zip) as ship_zip, IF(u.shipping_id=0, u.ship_country, us.ship_country) as ship_country, u.shipping_id, u.active, u.tax_exempt, u.fishbowl_id, u.failed_attempts, u.unclaimed, u.auto_generated,
                                            u.last_failed_attempt_at, u.create_date, u.update_date, u.dealer_app_submitted, u.converted_from_user from users u left join user_billing ub on ub.id = u.billing_id left join user_shipping us on
                                            us.id = u.shipping_id where u.user_id in (_USER_ID_) and u.auto_generated=0 AND u.active='Y' and (u.billing_id > 0 OR LENGTH(u.bill_street) > 0) order by u.user_id;";

    public const string Carriers = @"select * from carrier;";
}