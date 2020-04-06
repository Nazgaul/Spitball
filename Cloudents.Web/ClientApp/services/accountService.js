import axios from 'axios'
import {User} from './Dto/user.js';
import {Coupon} from './Dto/coupon.js';
import searchService from './searchService';

const accountInstance = axios.create({
    baseURL:'/api/account'
})


//TODO: move this shit to couponServices! ny : HopoG
const couponInstance = axios.create({
    baseURL:'/api/coupon'
})

export default {
    async getAccount(){ 
        let {data} = await accountInstance.get()
        return new User.Account(data)
    },
    async getNumberReffered(){ 
        return await accountInstance.get('/referrals')
    },
    async saveUserInfo(params){ 
        return await accountInstance.post('/settings',params)
    },
    async becomeTutor(params){ 
        return await accountInstance.post('/becomeTutor',params)
    },
    async uploadImage(params){ 
        return await accountInstance.post('/image',params)
    },
    async uploadCover(params){ 
        return await accountInstance.post('/cover',params)
    },
    async applyCoupon(params){ 
        return await couponInstance.post('/apply',params)
    },
    async getAccountStats(days){
        let {data} = await accountInstance.get('/stats', {params: {days}})
        return data.map(stats => new User.Stats(stats))
    },  
    async getCoupons() {
        let { data } = await couponInstance.get();
        return data.map(coupon => new Coupon.Default(coupon))
    },
    async getQuestions(){
        let {data} = await accountInstance.get('/questions')
        return data.map(question => searchService.createQuestionItem(question))
    }
}