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
         this.id = objInit.id
         this.firstName = objInit.firstName
         this.lastName = objInit.lastName
         this.name = `${objInit.firstName} ${objInit.lastName}`
         this.image = objInit.image || ''
         this.documentCourses = objInit.documentCourses
         this.courses = objInit.courses,
         this.coursesString = objInit.courses.toString().replace(/,/g, ", ")
         this.online = objInit.online || false
         this.calendarShared = objInit.calendarShared || false
         this.isTutor = objInit.hasOwnProperty('tutor') || false
         this.followers = objInit.followers || ''
         this.isFollowing = objInit.isFollowing
         this.cover = objInit.cover || ''
         this.tutorData = {
            price: objInit.price || 0,
            bio: objInit.bio || '',
            lessons: objInit.lessons || 0,
            discountPrice: objInit.discountPrice,
            pendingSessionsPayments: objInit.pendingSessionsPayments || null,
            description: objInit.description || '',
            contentCount: objInit.contentCount,
            hasCoupon: objInit.hasCoupon,
            rate: objInit.rate || 0,
            reviewCount: objInit.reviewCount || 0,
            firstName: objInit.firstName || '',
            lastName: objInit.lastName || '',
            students: objInit.students || 0,
            subscriptionPrice: objInit.subscriptionPrice,
            isSubscriber : objInit.isSubscriber
         }
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
      if (state.profile.user.isTutor) {
         state.profile.user.tutorData.bio = newData.bio;
         state.profile.user.firstName = newData.firstName;
         state.profile.user.lastName = newData.lastName;
         state.profile.user.tutorData.description = newData.description;
         state.profile.user.tutorData.price = newData.price;
      } else {
         state.profile.user.name = newData.name;
         state.profile.user.firstName = newData.firstName;
         state.profile.user.lastName = newData.lastName;
         state.profile.user.description = newData.description;
      }
   },
   setProfilePicture(state, imageUrl) {
      if (state.profile && state.profile.user) {
         state.profile.user.image = imageUrl;
      }
   },
   setCoverPicture(state,imageUrl) {
      state.profile.user.cover = imageUrl;
      //state.profile.user.cover = imageUrl;
   }
}
const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getShowEditDataDialog: state => state.showEditDataDialog,
   getProfileCoverImage: state => state.profile?.user?.cover || '',
   getProfileTutorSubscription: state => state.profile?.user?.tutorData?.subscriptionPrice,
   getIsSubscriber: state => {
      return state.profile?.user?.tutorData?.isSubscriber
   },
}

const actions = {
   syncProfile({commit, dispatch, state}, { id, type, params }) {
      if(state.profile) return Promise.resolve(state.profile)
      const profile = profileInstance.get(`${id}`)
      const profileReviews = profileInstance.get(`${id}/about`)

      return Promise.all([profile, profileReviews]).then(res => {
         commit('setProfile', res[0].data)
         commit('setProfileReviews', res[1].data)
         dispatch('updateProfileItemsByType', { id, type, params });
         const profileUserData = state.profile
         dispatch('setUserStatus', profileUserData.user);
         return profileUserData
      })
      
      // return profileService.getProfile(id).then(profileUserData => {
      //    commit('setProfile', profileUserData);
      //    dispatch('updateProfileItemsByType', { id, type, params });
      //    dispatch('setUserStatus', profileUserData.user);
      //    if (profileUserData.user.isTutor) {
      //       profileService.getProfileReviews(id).then(val => {
      //          commit('setProfileReviews', val);
      //       })
      //    }
      //    return profileUserData
      // });
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