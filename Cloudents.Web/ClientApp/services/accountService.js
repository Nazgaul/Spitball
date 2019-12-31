import { connectivityModule } from "./connectivity.module";
import searchService from "../services/searchService.js";

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
                online: objInit.online || false,
                universityName: objInit.universityName,
                description: objInit.description || '',
                calendarShared: objInit.calendarShared || false,
                tutorData: objInit.tutor ? createTutorData(objInit.tutor) : createTutorData({}),
                isTutor: objInit.hasOwnProperty('tutor') || false,
                followers: objInit.followers || '',
                
                firstName: objInit.firstName || '',
                lastName: objInit.lastName || '',
                isFollowing: objInit.isFollowing,
            }
        )
    },
    Review:function(objInit){
      this.reviewText = objInit.reviewText;
      this.rate = objInit.rate;
      this.date = objInit.created;
      this.name = objInit.name;
      this.id = objInit.id;
      this.image = objInit.image;
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
    }
}
function createProfileReviews(objInit){
    return new User.Reviews(objInit)
}
function AccountUser(objInit){
    this.id= objInit.id;
    this.name= objInit.name;
    this.image = objInit.image || '';

    this.online = objInit.online || false;
    
    this.balance= objInit.balance;
    this.email= objInit.email;
    this.token= objInit.token;
    this.universityExists= objInit.universityExists;
    this.score = objInit.score;
    this.phoneNumber = objInit.phoneNumber;
    this.isTutor = objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok';
    this.isTutorState =  createIsTutorState(objInit.isTutor);// state of become tutor request, possible options ok, pending;
    this.needPayment = objInit.needPayment || false;
    this.currencySymbol = objInit.currencySymbol;
}
function createIsTutorState(str){
    if(str && str.toLowerCase() === 'ok'){
        return 'ok';
    }else if(str && str.toLowerCase() === 'pending'){
        return 'pending';
    }else{
        return null;
    }
}
 function TutorData(objInit) {
     this.bio = objInit.bio;
     this.currency = objInit.currency;
     this.documents = objInit.documents;
     this.hasCoupon = objInit.hasCoupon;
     this.lessons = objInit.lessons;
     this.subjects = objInit.subjects;
    //  this.courses = objInit.courses;
     this.price = objInit.price || 0;
     this.rate = objInit.rate || 0;
     this.reviewCount = objInit.reviewCount || 0;
     this.discountPrice = objInit.discountPrice;

     this.firstName = objInit.firstName || '';
     this.lastName = objInit.lastName  || '';
 }

 function createTutorData(objInit) {
     return new TutorData(objInit);
 }
 function createUserPersonalData(objInit) {
    return new User.Profile(objInit)   
 }

function ProfileUserData(objInit){
    this.user= createUserPersonalData(objInit.data) ;
    this.questions = [];
    this.answers = [];
    this.documents = [];
    this.purchasedDocuments= [];
}
function ProfileQuestionData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
    // return arrInit[1].data.map(searchService.createQuestionItem).map(item => {
    //     return {
    //         ...item,
    //         user: arrInit[0].data,
    //     }
    // }) || [];
}

function ProfileAnswerData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
}
function ProfileDocumentData(arrInit){
   return arrInit.data.map(searchService.createDocumentItem) || [];
}
export default {
    getAccount:() => {
       return connectivityModule.http.get("/Account").then(({data})=>{
           let userAccount = new AccountUser(data);
           return userAccount;
       },(err)=>{
           return err;
       });
    },
    setUserName: (data) => {
        return connectivityModule.http.post("/Account/userName", {name: data});
    },
    getUserName: () => {
        return connectivityModule.http.get("/Account/userName");
    },
    uploadImage: (formData) => {
        return connectivityModule.http.post("/Account/image", formData);
    },
    getProfile:(id) => {
        return connectivityModule.http.get(`/Profile/${id}`);
    },
    getProfileReviews:(id) => {
        return connectivityModule.http.get(`/Profile/${id}/about`).then(reviews=>{
            return createProfileReviews(reviews.data)
        });
    },
    getNumberReffered:() => {
        return connectivityModule.http.get(`/Account/referrals`);
    },
    getProfileQuestions:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`Profile/${id}/questions/${strPage}`);
    },
    getProfileAnswers:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/answers/${strPage}`);
    },
    getProfileDocuments:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/documents/${strPage}`);
    },
    getProfilePurchasedDocuments:(id, page)=>{
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/purchaseDocuments/${strPage}`);
    },
    saveTutorInfo: (data)=> {
        let serverFormat= {
            firstName: data.firstName,
            description: data.description,
            lastName: data.lastName,
            bio: data.bio,
            price: data.price
        };
        return connectivityModule.http.post("/Account/settings", serverFormat);
    },
    saveUserInfo: (data)=> {
        let serverFormat= {
                firstName: data.firstName,
                lastName: data.lastName,
                description: data.description

        };
        return connectivityModule.http.post("/Account/settings", serverFormat);
    },
    // createProfileData: (arrInit)=>{
    //     return new ProfileData(arrInit);
    // },
    becomeTutor: (data) => {
        return connectivityModule.http.post("/Account/becomeTutor", data);
    },
    createUserProfileData: (objInit)=>{
        return new ProfileUserData(objInit);
    },
    createProfileQuestionData: (arrInit)=>{
        return new ProfileQuestionData(arrInit);
    },
    createProfileAnswerData: (arrInit)=>{
        return new ProfileAnswerData(arrInit);
    },
    createProfileDocumentData: (arrInit)=>{
        return new ProfileDocumentData(arrInit);
    },
    followProfile: (id)=>{
        return connectivityModule.http.post(`/Profile/follow`,{id});
    },
    unfollowProfile: (id)=>{
        return connectivityModule.http.delete(`/Profile/unFollow/${id}`);
    }
}