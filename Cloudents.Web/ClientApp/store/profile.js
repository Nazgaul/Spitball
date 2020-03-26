import profileService from "../services/profileService";

const state = {
   profile: null,
   profileReviews: null,
   showEditDataDialog: false,
}

const mutations = {
   setProfile(state, val) {
      state.profile = val;
   },
   setPorfileDocuments(state, val) {
      state.profile.documents = val;
   },
   resetProfile(state) {
      state.profile = null;
   },
   setProfileFollower(state, val) {
      state.profile.user.isFollowing = val;
      if (val) {
         state.profile.user.followers += 1;
      } else {
         state.profile.user.followers -= 1;
      }
   },
   setEditDialog(state, val) {
      state.showEditDataDialog = val;
   },
   setProfileReviews(state, val) {
      state.profileReviews = val;
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
}
const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getShowEditDataDialog: state => state.showEditDataDialog,
}

const actions = {
   syncProfile(context, { id, type, params }) {
      profileService.getProfile(id).then(profileUserData => {
         context.commit('setProfile', profileUserData);
         context.dispatch('updateProfileItemsByType', { id, type, params });
         context.dispatch('setUserStatus', profileUserData.user);
         if (profileUserData.user.isTutor) {
            profileService.getProfileReviews(id).then(val => {
               context.commit('setProfileReviews', val);
            })
         }
      });
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
   resetProfileData(context) {
      context.commit('resetProfile');
   },
   toggleProfileFollower({ state, commit }, val) {
      if (val) {
         return profileService.followProfile(state.profile.user.id).then(() => {
            commit('setProfileFollower', true)
            return Promise.resolve()
         })
      } else {
         return profileService.unfollowProfile(state.profile.user.id).then(() => {
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
}

export default {
   state,
   mutations,
   getters,
   actions
}