import accountService from '../services/accountService.js';
import couponService from '../services/couponService.js'

const state = {
    couponError: false,
    couponDialog: false,
};

const getters = {
    getCouponError: state => state.couponError,
    getCouponDialog: state => state.couponDialog,
};

const mutations = {
    setCouponError: (state, error) => state.couponError = error,
    setCouponDialog(state, val){ 
        state.couponDialog = val;
        state.couponError = false;
    }
};

const actions = {
    updateCoupon({commit, getters}, couponObj){
        return accountService.applyCoupon(couponObj).then(({data}) => {
            let tutorUser = getters.getProfile.user.tutorData;
            if(data.price === 0) {
                tutorUser.discountPrice = tutorUser.price;
                tutorUser.price = data.price;
            } else {
                tutorUser.discountPrice = data.price;
            }
            
            if(!tutorUser.hasCoupon)  tutorUser.hasCoupon = true;

            commit('setCouponError', false);
            commit('setCouponDialog', false);
        }).catch(ex => {
            commit('setCouponError', true);
            console.log(ex);
        });
    },
    updateCouponDialog({commit}, val) {
        commit('setCouponDialog', val);
    },
    createCoupon(context,paramObj){
        return couponService.createCoupon(paramObj)
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}