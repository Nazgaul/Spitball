export const Coupon = {
    Default: function(objInit) {
        this.code = objInit.code;
        this.couponType = objInit.couponType;
        this.value = objInit.value;
        this.createTime = objInit.createTime;
        this.expiration = objInit.expiration;
    }
}