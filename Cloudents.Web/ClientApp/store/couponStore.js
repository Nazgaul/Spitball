import couponService from "../services/couponService";

const state = {
    couponError: false
};

const getters = {
    getCouponError: state => state.couponError,
};

const mutations = {
    setCouponError: (state, error) => state.couponError = error,
};

const actions = {
    updateCoupon({commit, getters}, couponObj){
        couponService.applyCoupon(couponObj).then(res => {
            let tutorUser = getters.getProfile.user.tutorData;
            tutorUser.discountPrice = res.data.price;
            commit('setCouponError', false);
        }).catch(ex => {
            commit('setCouponError', true);
            console.log(ex);
        })
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}