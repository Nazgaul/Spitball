import axios from 'axios'

const couponInstance = axios.create({
    baseURL: '/api/coupon'
})

const state = {
    couponError: false,
    couponDialog: false,
};

const getters = {
    getCouponError: state => state.couponError,
    //getCouponDialog: state => state.couponDialog,
};

const mutations = {
    setCouponError: (state, error) => state.couponError = error,
    setCouponDialog(state, val){ 
        state.couponDialog = val;
        state.couponError = false;
    }
};

const actions = {
    createCoupon(context, params){
        return couponInstance.post('', params)
    },
    updateCoupon({commit, getters}, params){
        return couponInstance.post('/apply', params).then(({data}) => {
            let tutorUser = getters.getCoursePrice || getters.getRoomTutor.tutorUser?.tutorPrice
            tutorUser.amount = data.price;
            
            commit('setCouponError', false);
            commit('setCouponDialog', false);

        }).catch(ex => {
            commit('setCouponError', true);
            console.error(ex);
            return Promise.reject()
        });
    },
    getUserCoupons() {
        return couponInstance.get().then(({data}) => {
            function Coupon(objInit) {
                this.code = objInit.code;
                this.couponType = objInit.couponType;
                this.value = objInit.value;
                this.createTime = objInit.createTime;
                this.expiration = objInit.expiration;
                this.amountOfUsers = objInit.amountOfUsers;
            }
            return data.map(coupon => new Coupon(coupon))
        })
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}