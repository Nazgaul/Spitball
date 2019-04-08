import { connectivityModule } from "./connectivity.module";
import searchService from "../services/searchService.js";
function AccountUser(ObjInit){
    this.balance= ObjInit.balance;
    this.email= ObjInit.email;
    this.id= ObjInit.id;
    this.name= ObjInit.name;
    this.token= ObjInit.token;
    this.universityExists= ObjInit.universityExists;
    this.score = ObjInit.score;
    this.phoneNumber = ObjInit.phoneNumber;
    this.isTutor = ObjInit.isTutor || false;
    this.image = ObjInit.image || '';

}
 function TutorData(objOnit) {
     this.online = objOnit.online || false;
     this.price= objOnit.price || 0;
     this.rate = objOnit.rate || 0;
     this.reviewCount = objOnit.reviewCount || 0;
     this.firstName = objOnit.firstName;
     this.lastName = objOnit.lastName;
 }

 function CreateTutorData(objInit) {
     return new TutorData(objInit);
 }
 function createUserPersonalData(objInit) {
     return new ProfilePersonalData(objInit);
 }
function ProfilePersonalData(objInit){
    this.id = objInit.id;
    this.name = objInit.name;
    this.description = objInit.description || '';
    this.score = objInit.score;
    this.image = objInit.image || '';
    this.universityName= objInit.universityName;
    this.isTutor= objInit.hasOwnProperty('tutor') || false;
    this.tutorData = objInit.tutor ? CreateTutorData(objInit.tutor) : CreateTutorData({});

}

function ReviewItem(objInit){
    this.created = objInit.created;
    this.image = objInit.image;
    this.rate = objInit.rate;
    this.reviewText = objInit.reviewText;
    this.score = objInit.score;
    this.name = objInit.name || '';
}

function createReviewItem(objInit) {
    return new ReviewItem(objInit)

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


function profileUserData(objInit){
    this.user= createUserPersonalData(objInit.data) ;
    this.questions = [];
    this.answers = [];
    this.documents = [];
    this.purchasedDocuments= [];
}
function profileQuestionData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
    // return arrInit[1].data.map(searchService.createQuestionItem).map(item => {
    //     return {
    //         ...item,
    //         user: arrInit[0].data,
    //     }
    // }) || [];
}

function profileAnswerData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
}
function profileDocumentData(arrInit){
   return arrInit.data.map(searchService.createDocumentItem) || [];
}
function profileAboutData(arrInit){
    let structuredData = createAboutItem(arrInit[1].data);
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
           let UserAccount = new AccountUser(data);
           return UserAccount;
       },(err)=>{
           return err;
       })
    },
    setUserName: (data) => {
        return connectivityModule.http.post("/Account/userName", {name: data})
    },
    getUserName: () => {
        return connectivityModule.http.get("/Account/userName")
    },
    uploadImage: (formData) => {
        return connectivityModule.http.post("/Account/image", formData)
    },
    getProfile:(id) => {
        return connectivityModule.http.get(`/Profile/${id}`)
    },
    getNumberReffered:(id) => {
        return connectivityModule.http.get(`/Account/referrals`)
    },
    getProfileAbout:(id) => {
        return connectivityModule.http.get(`Profile/${id}/about/`)
    },
    getProfileQuestions:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`Profile/${id}/questions/${strPage}`)
    },
    getProfileAnswers:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/answers/${strPage}`)
    },
    getProfileDocuments:(id, page) => {
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/documents/${strPage}`)
    },
    getProfilePurchasedDocuments:(id, page)=>{
        let strPage = page ? `?page=${page}` : "";
        return connectivityModule.http.get(`/Profile/${id}/purchaseDocuments/${strPage}`)
    },
    saveTutorInfo: (data)=> {
        let serverFormatt= {
            firstName: data.name,
            description: data.description,
            lastName: data.lastName,
            bio: data.bio,
        };
        return connectivityModule.http.post("/Account/settings", serverFormatt)
    },
    saveUserInfo: (data)=> {
        let serverFormatt= {
                firstName: data.name,
                description: data.description

        };
        return connectivityModule.http.post("/Account/settings", serverFormatt)
    },
    // createProfileData: (arrInit)=>{
    //     return new ProfileData(arrInit);
    // },
    createUserProfileData: (objInit)=>{
        return new profileUserData(objInit);
    },
    createProfileQuestionData: (arrInit)=>{
        return new profileQuestionData(arrInit);
    },
    createProfileAnswerData: (arrInit)=>{
        return new profileAnswerData(arrInit)
    },
    createProfileDocumentData: (arrInit)=>{
        return new profileDocumentData(arrInit)
    },
    createProfileAbout: (arrInit)=>{
        return new profileAboutData(arrInit)
    }
}