import axios from 'axios'
import {User} from './Dto/user.js';

const accountInstance = axios.create({
    baseURL:'/api/account'
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
    async applyCoupon(params){ 
        return await accountInstance.post('/coupon',params)
    }
}