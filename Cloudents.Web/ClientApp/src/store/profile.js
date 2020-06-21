import profileService from "../services/profileService";

import axios from 'axios'

const profileInstance = axios.create({
    baseURL:'/api/profile'
})

const state = {
   profile: null,
   profileReviews: null,
   showEditDataDialog: false,
}

const mutations = {
   setProfile(state, data) {
      let profile = new Profile(data)
      function Profile(objInit) {
         
         this.questions = [];
         this.answers = [];
         this.documents = [];
         this.purchasedDocuments = [];
         this.id = objInit.id;
         this.firstName = objInit.firstName;
         this.lastName = objInit.lastName;
         this.name = `${objInit.firstName} ${objInit.lastName}`;
         this.bio = objInit.bio;
         this.contentCount = objInit.contentCount;
         this.cover = objInit.cover;
         this.description = objInit.description;
         this.documentCourses = objInit.documentCourses;
         this.image = objInit.image;
         this.isFollowing = objInit.isFollowing;
         this.isSubscriber = objInit.isSubscriber;
         this.lessons = objInit.lessons;
         this.followers = objInit.followers;
         this.reviewCount = objInit.reviewCount;
         this.students = objInit.students;
         this.subscriptionPrice = objInit.subscriptionPrice;
         this.calendarShared = objInit.calendarShared;
         this.tutorCountry = objInit.tutorCountry;
      }

      state.profile = profile;
   },
   setPorfileDocuments(state, val) {
      state.profile.documents = val;
   },
   resetProfile(state) {
      state.profile = null;
   },
   setProfileFollower(state, val) {
      if(state.profile?.user) {
         state.profile.user.isFollowing = val;
         if (val) {
            state.profile.user.followers += 1;
         } else {
            state.profile.user.followers -= 1;
         }
      }
   },
   setEditDialog(state, val) {
      state.showEditDataDialog = val;
   },
   setProfileReviews(state, data) {
      let profileReviews = new ProfileReviews(data)

      function ProfileReviews(objInit) {
         this.reviews = objInit.reviews ? objInit.reviews.map(review => {
            return {
               id : review.id || objInit.userId,
               name : review.name,
               firstName : review.firstName,
               lastName : review.lastName,
               image : review.image || '',
               reviewText: review.reviewText,
               rate: review.rate,
               date: review.created,
            }
         }) : null
         this.rates = new Array(5).fill(undefined).map((val, key) => {
            return !!objInit.rates[key] ? objInit.rates[key] : { rate: 0, users: 0 };
         })
      }

      state.profileReviews = profileReviews;
   },
   updateEditedData(state, newData) {
      state.profile.user.tutorData.bio = newData.bio;
      state.profile.user.firstName = newData.firstName;
      state.profile.user.lastName = newData.lastName;
      state.profile.user.tutorData.description = newData.description;
      state.profile.user.tutorData.price = newData.price;
   },
   setProfilePicture(state, imageUrl) {
      if (state.profile && state.profile.user) {
         state.profile.user.image = imageUrl;
      }
   },
   setCoverPicture(state,imageUrl) {
      state.profile.user.cover = imageUrl;
   }
}
const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getShowEditDataDialog: state => state.showEditDataDialog,
   getProfileCoverImage: state => state.profile?.user?.cover || '',
   getProfileTutorSubscription: state => state.profile?.user?.tutorData?.subscriptionPrice,
   getIsMyProfile: (state, _getters) => _getters.getUserLoggedInStatus && (state.user?.id === _getters.getProfile?.id),
   getProfileTutorName: state => state.profile?.name || 'this is a test',
   getIsSubscriber: state => {
      return state.profile?.user?.tutorData?.isSubscriber
   },
}

const actions = {
   syncProfile({commit, dispatch, state}, { id, type, params }) {
      return profileInstance.get(`${id}`).then((res) => {
         commit('setProfile', res.data)
         dispatch('updateProfileItemsByType', { id, type, params });

         const profileUserData = state.profile
         dispatch('setUserStatus', profileUserData.user);
         return profileInstance.get(`${id}/about`).then((res2) => {
            commit('setProfileReviews', res2.data)
            return profileUserData
         })
      }).catch(ex => {
         return ex
      })
   },
   updateProfileItemsByType({ state, commit }, { id, type, params }) {
      if (!!state.profile && !!state.profile.user) {
         if (type == "documents") {
            return profileService.getProfileDocuments(id, params).then(documents => {
               commit('setPorfileDocuments', documents);
            });
         }
      }
   },
   toggleProfileFollower({ state, commit, getters }, val) {
      let tutorId = getters.getCurrTutor?.id || state.profile?.user?.id    
      if (val) {
         return profileService.followProfile(tutorId).then(() => {
            commit('setProfileFollower', true)
            return Promise.resolve()
         })
      } else {
         return profileService.unfollowProfile(tutorId).then(() => {
            commit('setProfileFollower', false)
            return Promise.resolve()
         })
      }
   },
   updateEditDialog(context, val) {
      context.commit('setEditDialog', val);
   },
   updateEditedProfile(context, newdata) {
      context.commit("updateEditedData", newdata);
   },
   getStudyroomLiveSessions(context, id) {
      return profileService.getStudyroomLiveSessions(id)
   },
   updateStudyroomLiveSessions(context ,session) {
      return profileService.updateStudyroomLiveSessions(session.userId, session.studyRoomId)
   }
}

export default {
   state,
   mutations,
   getters,
   actions
}