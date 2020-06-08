import {User} from './user.js';


export const Item = {
   Default: function (objInit) {
      this.id = objInit.id;
      this.type = objInit.type;
      this.course = objInit.course;
      //On profile page we do not pass user type
      if (objInit.user) {
         this.user = new User.Default(objInit.user);
      }
     // this.views = objInit.views;
     // this.downloads = objInit.downloads;
      this.url = objInit.url;
      this.dateTime = new Date(objInit.dateTime);
      this.votes = !!objInit.vote ? objInit.vote.votes : null;
      this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
      //TODO REMOVE THIS
      this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;
      this.price = objInit.price ? objInit.price.toFixed(0) : 0;

      this.priceType =  objInit.priceType || 'Free'; //Free,HasPrice,Subscriber
      if (this.price == 0 ) {
         this.priceType = 'Free'
      }

      this.preview = objInit.preview;
    //  this.purchased = objInit.purchased;
      this.title = objInit.title;
      this.documentType = objInit.documentType;
      this.template = 'result-note';
   },
   Video: function (objInit) {
      return Object.assign(
         new Item.Default(objInit),
         {
            itemDuration: objInit.duration,
         }
      )
   },
   Document: function (objInit) {
      return Object.assign(
         new Item.Default(objInit),
         {
            snippet: objInit.snippet,
         }
      )
   },
}
