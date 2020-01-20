const User = {
   Default:function(objInit){
       this.id = objInit.id;
       this.name = objInit.name;
       this.image = objInit.image || '';
   },
   Profile:function(objInit){
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
   Tutor:function(objInit){
      this.bio = objInit.bio;
      this.currency = objInit.currency;
      this.documents = objInit.documents;
      this.hasCoupon = objInit.hasCoupon;
      this.lessons = objInit.lessons;
      this.subjects = objInit.subjects;
      this.price = objInit.price || 0;
      this.rate = objInit.rate || 0;
      this.reviewCount = objInit.reviewCount || 0;
      this.discountPrice = objInit.discountPrice;
      this.firstName = objInit.firstName || '';
      this.lastName = objInit.lastName  || '';
   },
   Review:function(objInit){
       return Object.assign(
           new User.Default(objInit),
           {
               reviewText: objInit.reviewText,
               rate: objInit.rate,
               date: objInit.created,
           }
       )
   },
   Reviews:function(objInit){
       this.reviews = objInit.reviews? objInit.reviews.map(review => new User.Review(review)) : null;
       this.rates = new Array(5).fill(undefined).map((val, key) => {
           if(!!objInit.rates[key]){
               return objInit.rates[key];
           }else{
               return {rate: 0,users: 0}
           }
       })
   },
}

const Item = {
   Default: function (objInit) {
      this.id = objInit.id || null;
      this.type = objInit.type || 'Document';
      this.course = objInit.course;
      this.university = objInit.university;
      this.user = objInit.user ? new User.Default(objInit.user) : '';
      this.views = objInit.views;
      this.downloads = objInit.downloads;
      this.url = objInit.url;
      this.dateTime = objInit.dateTime;
      this.votes = !!objInit.vote ? objInit.vote.votes : null;
      this.upvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "up" ? true : false) : false) : null;
      this.downvoted = !!objInit.vote ? (!!objInit.vote.vote ? (objInit.vote.vote.toLowerCase() === "down" ? true : false) : false) : null;
      this.price = objInit.price;
      this.preview = objInit.preview;
      this.purchased = objInit.purchased;
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
export {
   User,
   Item,
}