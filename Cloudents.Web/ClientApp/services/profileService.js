import axios from 'axios'
import { Profile } from './Dto/profile.js';

const profileInstance = axios.create({
    baseURL:'/api/profile'
})

export default {
   async getProfile(id){ 
      let {data} = await profileInstance.get(`${id}`)
      return new Profile.ProfileUserData(data)
   },
   async getProfileDocuments(id,params){ 
      let {data} = await profileInstance.get(`${id}/documents`, { params })
      return new Profile.ProfileItems(data)
   },
   async followProfile(id){ 
      return await profileInstance.post('follow',{id})
   },
   async getProfileReviews(id){ 
      let {data} = await profileInstance.get(`${id}/about`)
     return new Profile.Reviews(data)
   },
   async unfollowProfile(id) {
      return await profileInstance.delete(`unfollow/${id}`)
   },
   async getStudyroomLiveSessions(id) {
      let { data } = await profileInstance.get(`${id}/studyRoom`)
      return data.map(broadcast => new Profile.BroadCastSessions(broadcast))
   },
   updateStudyroomLiveSessions(id, studyRoomId) {
      return profileInstance.post(`${id}/studyRoom`, { studyRoomId })     
   }
}