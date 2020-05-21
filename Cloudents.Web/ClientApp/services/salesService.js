import axios from 'axios';

const salesInstance = axios.create({
    baseURL:'/api/sales'
})

export default {
   async updateBillOffline(params){ 
      return await salesInstance.post('offline',params)
   },
}