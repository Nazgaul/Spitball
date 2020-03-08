import axios from 'axios'

const couponInstance = axios.create({
   baseURL: '/api/coupon'
})

export default {
   async createCoupon(params) {
      return await couponInstance.post('',params)
   },
}