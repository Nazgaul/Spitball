import { connectivityModule } from "./connectivity.module";

function applyCoupon(couponObj) {
    return connectivityModule.http.post('/Account/coupon', couponObj);
}

export default {
    applyCoupon,
}