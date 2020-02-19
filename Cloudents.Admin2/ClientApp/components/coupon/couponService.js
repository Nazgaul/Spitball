import { connectivityModule } from '../../services/connectivity.module';

function Coupon(objInit) {
    this.code = objInit.code;
    this.couponType = objInit.couponType;
    this.value = objInit.value;
    this.tutorId = objInit.tutorId;
    this.owner = objInit.owner;
    this.description = objInit.description;
    this.amountOfUsers = objInit.amountOfUsers;
}

function createCoupon(coupon) {
    return new Coupon(coupon)
}

function createCoupons(coupons) {
    return coupons.map(c => createCoupon(c))
}

function getCoupons() {
    return connectivityModule.http.get('AdminCoupon').then(createCoupons);
}

function createNewCoupon(couponObj) {
    return connectivityModule.http.post('AdminCoupon', couponObj);
}

export default {
    getCoupons,
    createNewCoupon
}