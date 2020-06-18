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
         this.user = {
            id: objInit.id,
            firstName: objInit.firstName,
            lastName: objInit.lastName,
            name: `${objInit.firstName} ${objInit.lastName}`,
            image: objInit.image || '',
            documentCourses: objInit.documentCourses,
            courses: objInit.courses,
            coursesString: objInit.courses.toString().replace(/,/g, ", "),
            online: objInit.online || false,
            calendarShared: objInit.calendarShared || false,
            isTutor: objInit.hasOwnProperty('tutor') || false,
            followers: objInit.followers || '',
            isFollowing: objInit.isFollowing,
            cover: objInit.cover || '',
            tutorData: {
               price: objInit.tutor.price || 0,
               bio: objInit.tutor.bio || '',
               lessons: objInit.tutor.lessons || 0,
               discountPrice: objInit.tutor.discountPrice,
               pendingSessionsPayments: objInit.tutor.pendingSessionsPayments || null,
               description: objInit.tutor.description || '',
               contentCount: objInit.tutor.contentCount,
               hasCoupon: objInit.tutor.hasCoupon,
               rate: objInit.tutor.rate || 0,
               reviewCount: objInit.tutor.reviewCount || 0,
               firstName: objInit.tutor.firstName || '',
               lastName: objInit.tutor.lastName || '',
               students: objInit.tutor.students || 0,
               subscriptionPrice: objInit.tutor.subscriptionPrice,
               isSubscriber : objInit.tutor.isSubscriber
            }
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
      return profileInstance.get(`${id}`).then((res) => {
         if(!res.data.tutor) return { user: { isTutor: false } }
         commit('setProfile', res.data)
         dispatch('updateProfileItemsByType', { id, type, params });

         const profileUserData = state.profile
         dispatch('setUserStatus', profileUserData.user);
         return profileInstance.get(`${id}/about`).then((res2) => {
            commit('setProfileReviews', res2.data)
            return profileUserData
         })
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