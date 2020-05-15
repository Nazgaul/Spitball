import {User} from './user.js';


export const Item = {
   Default: function (objInit) {
      this.id = objInit.id || null;
      this.type = objInit.type || 'Document';
      this.course = objInit.course;
      this.user = objInit.user ? new User.Default(objInit.user) : '';
      this.views = objInit.views;
      this.downloads = objInit.downloads;
      this.url = objInit.url;
      this.dateTime = objInit.dateTime ? new Date(objInit.dateTime) : '';
      this.votes = !!objInit.vote ? objInit.vote.votes : null;
      this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
      this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;
      this.price = objInit.price ? objInit.price.toFixed(0) : 0;
      this.preview = objInit.preview;
      this.purchased = objInit.purchased;
      this.title = objInit.title;
      this.documentType = objInit.documentType;
      this.subscribed = objInit.subscribed
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
