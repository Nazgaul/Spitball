import axios from 'axios'

const couponInstance = axios.create({
    baseURL: '/api/coupon'
})

const state = {
    couponError: false,
    couponDialog: false,
    couponList:[]
};

const getters = {
    getCouponError: state => state.couponError,
    //getCouponDialog: state => state.couponDialog,
    getCouponList: state => state.couponList,
};

const mutations = {
    setCouponError: (state, error) => state.couponError = error,
    setCouponDialog(state, val){ 
        state.couponDialog = val;
        state.couponError = false;
    },
    addCoupon(state,coupon){
        state.couponList.unshift({
            code: coupon.code,
            couponType: coupon.couponType,
            value: coupon.value,
            expiration: coupon.expiration,
            amountOfUsers: 0,
            createTime: new Date().toISOString()
        })
    },
    setCouponList(state, coupons){ 
        state.couponList = coupons.map(coupon => new Coupon(coupon));
        function Coupon(objInit) {
            this.code = objInit.code;
            this.couponType = objInit.couponType;
            this.value = objInit.value;
            this.createTime = objInit.createTime;
            this.expiration = objInit.expiration;
            this.amountOfUsers = objInit.amountOfUsers;
        }
    }
};

const actions = {
    createCoupon({commit}, params){
        return couponInstance.post('', params).then(()=>{
            commit('addCoupon',params)
        })
    },
    updateCoupon({commit, getters}, params){
        return couponInstance.post('/apply', params).then(({data}) => {
            let tutorUser = getters.getCoursePrice || getters.getRoomTutor.tutorUser?.tutorPrice
            tutorUser.amount = data.price;
            
            commit('setCouponError', false);
            commit('setCouponDialog', false);
            return
        }).catch(ex => {
            commit('setCouponError', true);
            console.error(ex);
            return Promise.reject(ex)
        });
    },
    getUserCoupons({commit}) {
        return couponInstance.get().then(({data}) => {
            commit('setCouponList',data)
        })
    }
};

export default {
    actions,
    state,
    getters,
    mutations
}