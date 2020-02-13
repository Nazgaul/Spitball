import store from '../../../../store/index.js';

const LOGIN = 'login';

const dialogsPremissions = {
   login: [],
   exitRegisterDialog: [],
   becomeTutor: ["auth"],
   upload: ["auth","courses"],
   createCoupon: ["auth","tutor"]
}

const _handler = {
   auth: function(dialogName){
      if(!store.getters.getUserLoggedInStatus){
         return LOGIN;
      }else{
         return dialogName
      }
   },
   tutor: function(dialogName){
      if(!store.getters.accountUser.isTutor){
         return ''
      }else{
         return dialogName;
      }
   },
   courses: function(dialogName){
      if(store.getters.getSelectedClasses.length > 0){
         return dialogName;
      }else{
         return { name: "addCourse" }
      }
   }
}

class DialogConfing {
   constructor(dialogName){
      this.name = dialogName;
   }
   getDialog(){
      let dialogRequireds = dialogsPremissions[this.name];
      for (let i = 0; i < dialogRequireds.length; i++){
         let required = dialogRequireds[i]
         let result;
         if(typeof required === 'function'){
            result = required(this.name)
         }else{
            let handlerFunction = _handler[required];
            result = handlerFunction(this.name)
         }
         if(result !== this.name){
            return result
         }
      }
      return this.name
   }
}

export default {
   getDialog: (dialogName) => {
      if(!dialogName){return ''}
      return new DialogConfing(dialogName).getDialog()
   }
}