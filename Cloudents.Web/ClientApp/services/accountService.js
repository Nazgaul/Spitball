import { connectivityModule } from "./connectivity.module";
import searchService from "../services/searchService.js";

function AccountUser(objInit){
    this.balance= objInit.balance;
    this.email= objInit.email;
    this.id= objInit.id;
    this.name= objInit.name;
    this.token= objInit.token;
    this.universityExists= objInit.universityExists;
    this.score = objInit.score;
    this.phoneNumber = objInit.phoneNumber;
    this.isTutor = objInit.isTutor && objInit.isTutor.toLowerCase() === 'ok';
    this.isTutorState =  createIsTutorState(objInit.isTutor);// state of become tutor request, possible options ok, pending;
    this.image = objInit.image || '';
    this.online = objInit.online || false;
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
     this.price = objInit.price || 0;
     this.currency = objInit.currency;
     this.rate = objInit.rate || 0;
     this.reviewCount = objInit.reviewCount || 0;
     this.firstName = objInit.firstName || '';
     this.lastName = objInit.lastName  || '';
     this.discountPrice = objInit.discountPrice;
     this.hasCoupon = objInit.hasCoupon;
 }

 function createTutorData(objInit) {
     return new TutorData(objInit);
 }
 function createUserPersonalData(objInit) {
     return new ProfilePersonalData(objInit);
 }
function ProfilePersonalData(objInit){
    this.id = objInit.id;
    this.name = objInit.name  || '';
    this.description = objInit.description || '';
    this.score = objInit.score;
    this.image = objInit.image || '';
    this.universityName= objInit.universityName;
    this.calendarShared = objInit.calendarShared || false;
    this.isTutor= objInit.hasOwnProperty('tutor') || false;
    this.tutorData = objInit.tutor ? createTutorData(objInit.tutor) : createTutorData({});
    this.online = objInit.online || false;
    this.firstName = objInit.firstName || '';
    this.lastName = objInit.lastName || '';
}

function ReviewItem(objInit){
    this.created = objInit.created;
    this.image = objInit.image;
    this.rate = objInit.rate;
    this.reviewText = objInit.reviewText;
    this.score = objInit.score;
    this.name = objInit.name || '';
    this.id = objInit.id|| '';
}

function createReviewItem(objInit) {
    return new ReviewItem(objInit);

}
function CourseItem(objInit) {
    this.name = objInit.name;
}

function createCourseItem(objInit) {
    return new CourseItem(objInit);
}

function AboutItem(objInit) {
    this.bio = objInit.bio;
    this.courses = objInit.courses.map(createCourseItem);
    this.reviews = objInit.reviews.map(createReviewItem);
}

function createAboutItem(objInit){
     return new AboutItem(objInit);
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
function ProfileAboutData(arrInit){
    let structuredData = createAboutItem(arrInit[0].data);
    let data = {
        bio: structuredData.bio,
        courses: structuredData.courses,
        reviews: structuredData.reviews
    };
    return data;

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
    getNumberReffered:() => {
        return connectivityModule.http.get(`/Account/referrals`);
    },
    getProfileAbout:(id) => {
        return connectivityModule.http.get(`Profile/${id}/about`);
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
    createProfileAbout: (arrInit)=>{
        return new ProfileAboutData(arrInit);
    }
}