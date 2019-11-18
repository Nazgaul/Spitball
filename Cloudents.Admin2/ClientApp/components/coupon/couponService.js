import { connectivityModule } from '../../services/connectivity.module';

function createNewCoupon(couponObj) {
    return connectivityModule.http.post('AdminCoupon', couponObj);
}

export default {
    createNewCoupon
}