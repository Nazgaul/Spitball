import { User } from './user.js';
import { Item } from './item.js';

export const Profile = {
   Profile: function (objInit) {
      return Object.assign(
         new User.Default(objInit),
         {
            courses: objInit.courses,
            online: objInit.online || false,
            universityName: objInit.universityName,
            description: objInit.description || '',
            calendarShared: objInit.calendarShared || false,
            tutorData: objInit.tutor ? new User.Tutor(objInit.tutor) : '',
            isTutor: objInit.hasOwnProperty('tutor') || false,
            followers: objInit.followers || '',
            firstName: objInit.firstName || '',
            lastName: objInit.lastName || '',
            isFollowing: objInit.isFollowing,
         }
      )
   },
   ProfileItems: function (objInit) {
      return Object.assign(
         {
            result: objInit.result.map(objData => {
               return new Item[objData.documentType](objData)
            }),
            count: objInit.count,
         }
      )
   },
   Review: function (objInit) {
      return Object.assign(
         new User.Default(objInit),
         {
            reviewText: objInit.reviewText,
            rate: objInit.rate,
            date: objInit.created,
         }
      )
   },
   Reviews: function (objInit) {
      this.reviews = objInit.reviews ? objInit.reviews.map(review => new Profile.Review(review)) : null;
      this.rates = new Array(5).fill(undefined).map((val, key) => {
         if (!!objInit.rates[key]) {
            return objInit.rates[key];
         } else {
            return { rate: 0, users: 0 }
         }
      })
   },
   ProfileUserData: function (objInit) {
      this.user = new Profile.Profile(objInit);
      this.questions = [];
      this.answers = [];
      this.documents = [];
      this.purchasedDocuments = [];
   },
}