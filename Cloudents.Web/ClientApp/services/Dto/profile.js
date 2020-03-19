import { User } from './user.js';
import { Item } from './item.js';

export const Profile = {
   Profile: function (objInit) {
      return Object.assign(
         new User.Default(objInit),
         {
            courses: objInit.documentCourses, 
            //Add courses - for the list  profile box
            online: objInit.online || false,
            description: objInit.tutor?.description || '', // remove that moved to objInit.tutor
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
      this.result = objInit.result.map(objData => new Item[objData.documentType](objData));
      this.count = objInit.count;
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
         return !!objInit.rates[key]? objInit.rates[key] : { rate: 0, users: 0 };
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