import profileService from "../services/profileService";
import axios from 'axios'

const profileInstance = axios.create({
    baseURL:'/api/profile'
})

let cancelTokenList;

const state = {
   profile: null,
   profileReviews: null,
   profileLiveSessions: [],
   showEditDataDialog: false,
   amountOfReviews :0
}

const getters = {
   getProfile: state => state.profile,
   getProfileReviews: state => state.profileReviews,
   getProfileLiveSessions: state => state.profileLiveSessions,
   getProfileStatsHours: state => state.profile?.user?.hoursTaught,
   getProfileStatsReviews: state => state.profile?.user?.reviewCount,
   getProfileStatsFollowers: state => state.profile?.user?.followers,
   getProfileStatsResources: state => state.profile?.user?.contentCount,
   getShowEditDataDialog: state => state.showEditDataDialog,
   getProfileCoverImage: state => state.profile?.user?.cover || '',
   getProfileTutorSubscription: state => state.profile?.user?.tutorData?.subscriptionPrice,
   getIsMyProfile: (state, _getters) => _getters.getUserLoggedInStatus && (state.profile?.user?.id === _getters.accountUser?.id),
   getProfileTutorName: state => state.profile?.user?.name,
   getIsSubscriber: state => {
      return state.profile?.user?.tutorData?.isSubscriber
   },
   getProfileDescription: state => state.profile?.user?.tutorData?.description,
   getProfileBio: state => state.profile?.user?.tutorData?.bio,
   getProfileParagraph: state => state.profile?.user?.tutorData?.paragraph,
   getAverageRate: state => ( state.amountOfReviews/state.profile?.user?.reviewCount) || 0,
   getProfileIsCalendar: state => state.profile?.user?.calendarShared
}

const mutations = {
   setProfile(state, data) {
      let profile = new Profile(data)

      function Profile(objInit) {
         //this.questions = [];
         //this.answers = [];
         this.documents = [];
         this.purchasedDocuments = [];
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
               bio: objInit.bio || '',
               lessons: objInit.lessons || 0,
               description: objInit.description || '',
               students: objInit.students || 0,
               rate: objInit.rate || 0,
               subscriptionPrice: objInit.subscriptionPrice,
               isSubscriber : objInit.isSubscriber,
               paragraph: objInit.paragraph || '',
            }
         }
      }
      state.profile = profile;
   },
   setProfileDocuments(state, data) {
      let documents = new Document(data)

      function Document(objInit) {
         this.result = objInit.result.map(objData => new DocumentItem(objData));
         this.count = objInit.count;         
      }

      function DocumentItem(objInit) {
         this.id = objInit.id;
         this.type = objInit.type;
         this.course = objInit.course;
         this.dateTime = new Date(objInit.dateTime);
         this.documentType = objInit.documentType;
         this.preview = objInit.preview;
         this.title = objInit.title;
         this.url = objInit.url;
         this.snippet = objInit.snippet
         this.itemDuration = objInit.duration
         this.template = 'result-note';
         this.priceType =  objInit.priceType || 'Free'; //Free,HasPrice,Subscriber
         this.price = objInit.price ? objInit.price.toFixed(0) : 0;
         if (this.price == 0 ) {
            this.priceType = 'Free'
         }

         //On profile page we do not pass user type
         // if (objInit.user) {
         //    this.user = {
         //       id: objInit.id || objInit.userId,
         //       name: objInit.name,
         //       firstName: objInit.firstName,
         //       lastName: objInit.lastName,
         //       image: objInit.image || ''
         //    };
         // }
        // this.views = objInit.views;
        // this.downloads = objInit.downloads;
         // this.votes = !!objInit.vote ? objInit.vote.votes : null;
         // this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
         //TODO REMOVE THIS
         // this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;
       //  this.purchased = objInit.purchased;
      }

      state.profile.documents = documents;
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
        let amountOfRevies = 0;
         this.rates = new Array(5).fill(undefined).map((val, key) => {
            const temp =  !!objInit.rates[key] ? objInit.rates[key] : { rate: 0, users: 0 };
            amountOfRevies += temp.rate*temp.users;
            return temp;
         })
         state.amountOfReviews = amountOfRevies;
         //let x=[];
         
        
      }

      state.profileReviews = profileReviews;
   },
   setLiveSession(state, data) {
      let broadcastSession = data.map(broadcast => new BroadcastSession(broadcast))

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
      state.profileLiveSessions = broadcastSession;
   },
   updateEditedData(state, newData) {
      state.profile.user.name = `${newData.firstName} ${newData.lastName}`;
      state.profile.user.firstName = newData.firstName;
      state.profile.user.lastName = newData.lastName;
      state.profile.user.tutorData.bio = newData.bio;
      state.profile.user.tutorData.description = newData.description;
      state.profile.user.tutorData.paragraph = newData.paragraph;
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

const actions = {
   syncProfile({commit, dispatch, state}, { id, pageSize }) {
      let option = {
         id,
         // type: 'documents',
         params: {
            page: 0,
            pageSize,
         }
      }
      return profileInstance.get(`${id}`).then((res) => {
         commit('setProfile', res.data)
         dispatch('updateProfileItemsByType', option);

         const profileUserData = state.profile
         dispatch('setUserStatus', profileUserData.user);
         //TODO - why are we waiting for the first one
         return profileInstance.get(`${id}/about`).then((res2) => {
            commit('setProfileReviews', res2.data)
         })
      })
   },
   updateProfileItemsByType({ commit }, { id, params }) {
      // if (!!state.profile && !!state.profile.user) {
         // if (type == "documents") {

            cancelTokenList?.cancel();
            // cancelTokenList.forEach(f=> {
            //    f.cancel();
            // });
            const axiosSource = axios.CancelToken.source();
            cancelTokenList = axiosSource;

            return profileInstance.get(`${id}/documents`, { params, cancelToken : axiosSource.token })
               .then(({data}) => {
                  commit('setProfileDocuments', data);
               });

            // return profileService.getProfileDocuments(id, params).then(documents => {
            //    commit('setProfileDocuments', documents);
            // });
         // }
      // }
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
   getStudyroomLiveSessions({ commit }, id) {
      profileInstance.get(`${id}/studyRoom`).then(({data}) => {
         commit('setLiveSession', data)
      })
   },
   updateStudyroomLiveSessions(context, session) {
      let id = session.userId
      let studyRoomId = session.studyRoomId
      return profileInstance.post(`${id}/studyRoom`, { studyRoomId })
   }
}

export default {
   state,
   mutations,
   getters,
   actions
}