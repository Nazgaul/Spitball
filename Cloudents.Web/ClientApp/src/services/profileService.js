import axios from 'axios'
import { Profile } from './Dto/profile.js';

const profileInstance = axios.create({
    baseURL:'/api/profile'
})

let cancelTokenList;

export default {
   async getProfileDocuments(id,params){ 

      cancelTokenList?.cancel();
      // cancelTokenList.forEach(f=> {
      //    f.cancel();
      // });
      const axiosSource = axios.CancelToken.source();
      cancelTokenList = axiosSource;
      let {data} = await profileInstance.get(`${id}/documents`, { params, cancelToken : axiosSource.token })
      return new Profile.ProfileItems(data)
   },
   async followProfile(id){ 
      return await profileInstance.post('follow',{id})
   },
   async unfollowProfile(id) {
      return await profileInstance.delete(`unfollow/${id}`)
   },
}