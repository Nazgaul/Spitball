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
            let tutorUser = getters.getRoomTutor;
            tutorUser.tutorPrice.amount = data.price;
            
            commit('setCouponError', false);
            commit('setCouponDialog', false);
            return
        }).catch(ex => {
            commit('setCouponError', true);
            console.error(ex);
            return Promise.reject()
        });
    },
    updateCouponDialog({commit}, val) {
        commit('setCouponDialog', val);
    },
    createCoupon(context,paramObj){
        return couponService.createCoupon(paramObj)
    },
    getUserCoupons() {
        return accountService.getCoupons();
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}