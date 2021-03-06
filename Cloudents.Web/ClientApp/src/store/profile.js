import axios from 'axios'
const profileInstance = axios.create({
    baseURL:'/api/profile'
})

const state = {
   courses: [],
   profile: null,
   faq: [],
   profileReviews: null,
   amountOfReviews: 0,
   profileCoverLoading: false,
   profileDrawerState: false,
   tempTitle: '',
   tempBio: '',
   tempParagraph: '',
   previewCoverImage: '',
   profilePreviewImage: '',
   coverPreview: ''
}

const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getProfileStatsHours: state => state.profile?.user?.hoursTaught,
   getProfileStatsReviews: state => state.profile?.user?.reviewCount,
   getProfileStatsFollowers: state => state.profile?.user?.followers,
   getProfileStatsResources: state => state.profile?.user?.contentCount,
   getProfileCoverImage: state => state.profile?.user?.cover || '',
   getProfileTutorSubscription: state => state.profile?.user?.tutorData?.subscriptionPrice,
   getIsMyProfile: (state, _getters) => _getters.getUserLoggedInStatus && (state.profile?.user?.id === _getters.accountUser?.id),
   getProfileFirstName:  state => state.profile?.user?.firstName,
   getProfileLastName:  state => state.profile?.user?.lastName,
   getProfileTutorName: state => state.profile?.user?.name,
   getIsSubscriber: state => state.profile?.user?.tutorData?.isSubscriber,
   getProfileTitle: state => state.profile?.user?.tutorData?.title,
   getProfileTempTitle: state => state.tempTitle,
   getProfileBio: state => state.profile?.user?.tutorData?.bio,
   getProfileTempBio: state => state.tempBio,
   
   getProfileParagraph: state => state.profile?.user?.tutorData?.paragraph,
   getProfileTempParagraph: state => state.tempParagraph,
   getPreviewCover: state => state.coverPreview,
   getProfilePreviewCoverImage: state => state.previewCoverImage,
   getAverageRate: state => ( state.amountOfReviews/state.profile?.user?.reviewCount) || 0,
   getProfileIsCalendar: state => state.profile?.user?.calendarShared,
   getProfileFaq: state => state.faq,
   getProfileCoverLoading: state => state.profileCoverLoading,
   getProfileCoverDrawer: state => state.profileDrawerState,
   //getProfileCountry: state => state.profile?.user?.tutorCountry,
   getProfileCourses: state => state.courses,
   getIsProfileFollowing: state => state.profile?.user?.isFollowing,
}

const mutations = {
   setProfile(state, data) {
      state.profile = new Profile(data)

      function Profile(objInit) {
         this.user = {
            id: objInit.id,
            firstName: objInit.firstName,
            lastName: objInit.lastName,
            name: `${objInit.firstName} ${objInit.lastName}`,
            image: objInit.image || '',
            cover: objInit.cover || '',
            calendarShared: objInit.calendarShared || false,
            isFollowing: objInit.isFollowing,
            followers: objInit.followers || 0,
            contentCount: objInit.contentCount,
            hoursTaught: objInit.hoursTaught,
            reviewCount: objInit.reviewCount || 0,
            online: objInit.online || false,
            tutorCountry: objInit.tutorCountry,
            tutorData: {
               bio: objInit.paragraph2 || '',
               lessons: objInit.lessons || 0,
               title: objInit.title || '',
               students: objInit.students || 0,
               rate: objInit.rate || 0,
               subscriptionPrice: objInit.subscriptionPrice,
               isSubscriber : objInit.isSubscriber,
               paragraph: objInit.paragraph3 || '',
            }
         }
      }
   },
   setProfileReviews(state, data) {
      state.profileReviews = new ProfileReviews(data)

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
         let amountOfRevies = 0;
         this.rates = new Array(5).fill(undefined).map((val, key) => {
            const temp =  !!objInit.rates[key] ? objInit.rates[key] : { rate: 0, users: 0 };
            amountOfRevies += temp.rate*temp.users;
            return temp;
         })
         state.amountOfReviews = amountOfRevies;
      }
   },
   setProfileFaq(state, data) {
      state.faq = data.map(faq => new ProfileFaq(faq))

      function ProfileFaq(objInit) {
         this.id = objInit.id
         this.title = objInit.title
         this.answer = objInit.answer
      }
   },
   resetProfile(state) {
      state.profile = null;
      state.profileCoverLoading = false;
      state.profileDrawerState = false
      state.profilePreviewImage = '';
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
   setProfileTutorInfo(state, {passData, coverImageUrl}) {
      state.profile.user.name = `${passData.firstName} ${passData.lastName}`;
      state.profile.user.firstName = passData.firstName;
      state.profile.user.lastName = passData.lastName;
      state.profile.user.tutorData.bio =  passData.shortParagraph;
      state.profile.user.tutorData.title = passData.title;
      state.profile.user.tutorData.paragraph = passData.paragraph;
      if(coverImageUrl) {
         state.profile.user.cover = coverImageUrl;
      }
   },
   setProfilePreviewPicture(state, imageUrl) {
      state.profilePreviewImage = imageUrl;
   },
   resetPreviewCover(state) {
      state.coverPreview = '' 
   },
   setCoverPicture(state, imageUrl) {
      state.previewCoverImage = imageUrl
      // state.profile.user.cover = imageUrl;
   },
   setProfileCoverLoading(state, val) {
      state.profileCoverLoading = val;
   },
   showPreviewCoverImage(state, coverImage) {
      state.coverPreview = coverImage
   },
   setProfileCourses(state,courses){
      state.courses = courses.map(course => new Course(course))

      function Course(objInit){
         this.id = objInit.id;
         this.name = objInit.name;
         this.image = objInit.image;
         this.price = {
            amount: objInit.price?.amount,
            currency: objInit.price?.currency
         }
         this.studyRoomCount = objInit.studyRoomCount;
         this.startTime = objInit.startTime;
         
         this.description = objInit.description;
         this.subscriptionPrice = {
            amount: objInit.subscriptionPrice?.amount,
            currency: objInit.subscriptionPrice?.currency
         }
      }
   },
   setFakeShorParagraph(state, paragraph) {
      // state.profile.user.tutorData.bio = paragraph
      state.tempParagraph = paragraph
   },
   setFakeShortTitle(state, title) {
      // state.profile.user.tutorData.title = title
      state.tempTitle = title
   },
   setFakeBio(state, bio) {
      // state.profile.user.tutorData.paragraph = bio
      state.tempBio = bio

   },
   setToggleProfileDrawer(state, val) {
      state.profileDrawerState = val
   }
}

const actions = {
   syncProfile({commit, dispatch, state}, id) {
      if(state.profile?.id == id) return Promise.resolve()
      return profileInstance.get(`${id}`).then(({data}) => {
         commit('setProfile', data)
         dispatch('setUserStatus', state.profile.user);

      })
   },
   updateProfileReviews({commit}, id) {
      return profileInstance.get(`${id}/about`).then(({data}) => {
         commit('setProfileReviews', data)
      })
   },
   updateProfileCourses({commit},id){
      profileInstance.get(`${id}/courses`).then(({data}) => {
         commit('setProfileCourses', data)
      })
   },
   async updateStudyroomLiveSessionsWithPrice(context,session) {
      let studyRoomId = session.studyRoomId
      let {data} = await axios.post(`wallet/Stripe/Course/${studyRoomId}`);
      return data.sessionId;
   },
   async updateStudyroomLiveSessionsWithPricePayMe(context,session) {
      let studyRoomId = session.studyRoomId
      let {data} = await axios.post(`wallet/Payme/Course/${studyRoomId}`);
      return data.sessionId;
   },
   updateProfileFaq({commit}) {
      // profileInstance.get(``).then(({data}) => {
         let data = [
            { id: 1, title: 'title 1', answer: 'answer1'},
            { id: 2, title: 'title 2', answer: 'answer2'},
            { id: 3, title: 'title 3', answer: 'answer3'}
         ]
         commit('setProfileFaq', data)
      // })
   },
   toggleProfileFollower({ state, commit }, val) {
      let id = state.profile?.user?.id
      if (val) {
         return profileInstance.post('follow',{ id }).then(() => {
            commit('setProfileFollower', true)
            return Promise.resolve()
         })
      } else {
         return profileInstance.delete(`unfollow/${id}`).then(() => {
            commit('setProfileFollower', false)
            return Promise.resolve()
         })
      }
   },
   updateProfileClassPosition(context, {oldIndex, newIndex}) {
      let params = {
         oldPosition: oldIndex,
         newPosition: newIndex,
         visibleOnly: true
      }
      axios.post(`course/move`, params)
   },
}

export default {
   state,
   mutations,
   getters,
   actions
}