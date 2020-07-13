import axios from 'axios'
const profileInstance = axios.create({
    baseURL:'/api/profile'
})

let cancelTokenList;

const state = {
   profile: null,
   // documents: [],
   faq: [],
   documents: [],
   profileReviews: null,
   profileLiveSessions: [],
   //showEditDataDialog: false,
   amountOfReviews: 0,
   profileCoverLoading: false
}

const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getProfileLiveSessions: state => state.profileLiveSessions,
   getProfileStatsHours: state => state.profile?.user?.hoursTaught,
   getProfileStatsReviews: state => state.profile?.user?.reviewCount,
   getProfileStatsFollowers: state => state.profile?.user?.followers,
   getProfileStatsResources: state => state.profile?.user?.contentCount,
   // getShowEditDataDialog: state => state.showEditDataDialog,
   getProfileCoverImage: state => state.profile?.user?.cover || '',
   getProfileTutorSubscription: state => state.profile?.user?.tutorData?.subscriptionPrice,
   getIsMyProfile: (state, _getters) => _getters.getUserLoggedInStatus && (state.profile?.user?.id === _getters.accountUser?.id),
   getProfileFirstName:  state => state.profile?.user?.firstName,
   getProfileLastName:  state => state.profile?.user?.lastName,
   getProfileTutorName: state => state.profile?.user?.name,
   getIsSubscriber: state => state.profile?.user?.tutorData?.isSubscriber,
   getProfileTitle: state => state.profile?.user?.tutorData?.title,
   getProfileBio: state => state.profile?.user?.tutorData?.bio,
   getProfileParagraph: state => state.profile?.user?.tutorData?.paragraph,
   getAverageRate: state => ( state.amountOfReviews/state.profile?.user?.reviewCount) || 0,
   getProfileIsCalendar: state => state.profile?.user?.calendarShared,
   getProfileDocuments: state => state.documents,
   getProfileDocumentsLength: state => state.documents.length,
   getProfileFaq: state => state.faq,
   getProfileCoverLoading: state => state.profileCoverLoading,
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
            documentCourses: objInit.documentCourses,
            coursesString: objInit.documentCourses.toString().replace(/,/g, ", "),
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
   setProfileDocuments(state, data) {
      state.documents = Object.keys(data).map(objData => new Document(objData, data[objData]))

      function Document(name, objInit) {
         this.result = Object.keys(objInit).map(objData => new DocumentItem(objInit[objData]));
         this.courseName = name
         this.isExpand = false
         this.count = objInit.length
      }

      function DocumentItem(objInit) {
         this.id = objInit.id;
         // this.type = objInit.type;
         this.course = objInit.course;
         this.dateTime = new Date(objInit.dateTime);
         this.documentType = objInit.documentType;
         this.preview = objInit.preview;
         this.title = objInit.title;
         this.url = `/document/${this.course}/${this.title}/${this.id}`;
         this.snippet = objInit.snippet
         this.itemDuration = objInit.duration
         this.template = 'result-note';
         this.priceType =  objInit.priceType || 'Free'; //Free,HasPrice,Subscriber
         this.price = objInit.price ? objInit.price.toFixed(0) : 0;
         if (this.price == 0 ) {
            this.priceType = 'Free'
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
   setLiveSession(state, data) {
      state.profileLiveSessions = data.map(broadcast => new BroadcastSession(broadcast))

      function BroadcastSession(objInit) {
         this.id = objInit.id;
         this.name = objInit.name;
         this.price = {
            amount: objInit.price.amount,
            currency: objInit.price.currency
         }
         this.isFull= objInit.isFull
         this.created = objInit.dateTime ? new Date(objInit.dateTime) : '';
         this.enrolled = objInit.enrolled;
         this.description = objInit.description;
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
   // setEditDialog(state, val) {
   //    state.showEditDataDialog = val;
   // },
   setProfileTutorInfo(state, newData) {
      state.profile.user.name = `${newData.firstName} ${newData.lastName}`;
      state.profile.user.firstName = newData.firstName;
      state.profile.user.lastName = newData.lastName;
      state.profile.user.tutorData.bio =  newData.shortParagraph;
      state.profile.user.tutorData.title = newData.title;
      state.profile.user.tutorData.paragraph = newData.bio;
   },
   setProfilePicture(state, imageUrl) {
      if (state.profile && state.profile.user) {
         state.profile.user.image = imageUrl;
      }
   },
   setCoverPicture(state, imageUrl) {
      state.profile.user.cover = imageUrl;
   },
   setProfileCoverLoading(state, val) {
      state.profileCoverLoading = val;
   },
   setExpandItems(state, item) {
      let document = state.documents.filter(doc => doc.courseName === item.courseName)[0]
      document.isExpand = !document.isExpand
   }
}

const actions = {
   syncProfile({commit, dispatch, state}, id) {
      if(state.profile?.id == id) return Promise.resolve()
      return profileInstance.get(`${id}`).then(({data}) => {
         commit('setProfile', data)
         dispatch('setUserStatus', state.profile.user);
         return
      })
   },
   updateProfileReviews({commit}, id) {
      return profileInstance.get(`${id}/about`).then(({data}) => {
         commit('setProfileReviews', data)
      })
   },
   updateProfileItemsByType({ commit }, id) {
      cancelTokenList?.cancel();
      const axiosSource = axios.CancelToken.source();
      cancelTokenList = axiosSource;

      return profileInstance.get(`${id}/documents`, { cancelToken : axiosSource.token })
         .then(({data}) => {
            commit('setProfileDocuments', data);
         });
   },
   // toggleProfileFollower({ state, commit, getters }, val) {
   //    let id = getters.getCurrTutor?.id || state.profile?.user?.id
   //    if (val) {
   //       return profileInstance.post('follow',{ id }).then(() => {
   //          commit('setProfileFollower', true)
   //          return Promise.resolve()
   //       })
   //    } else {
   //       return profileInstance.delete(`unfollow/${id}`).then(() => {
   //          commit('setProfileFollower', false)
   //          return Promise.resolve()
   //       })
   //    }
   // },
   getStudyroomLiveSessions({ commit }, id) {
      profileInstance.get(`${id}/studyRoom`).then(({data}) => {
         commit('setLiveSession', data)
      })
   },
   async updateStudyroomLiveSessionsWithPrice(context,session) {
      let studyRoomId = session.studyRoomId
      let {data} = await axios.post(`wallet/Stripe/StudyRoom/${studyRoomId}`);
      return data.sessionId;
   },
   async updateStudyroomLiveSessions(context, session) {
       let id = session.userId
       let studyRoomId = session.studyRoomId
    
      return profileInstance.post(`${id}/studyRoom`, { studyRoomId })
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
   }
}

export default {
   state,
   mutations,
   getters,
   actions
}