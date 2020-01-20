import Api from './Api/profile.js';
import {Profile} from './Constructors/profile.js';

export default {
   async getProfile(id){
      let {data} = await Api.get.profile(id)
      return new Profile.ProfileUserData(data)
   },
   async getProfileReviews(id){
      let {data} = await Api.get.reviews(id)
      return new Profile.Reviews(data)
   },
}
