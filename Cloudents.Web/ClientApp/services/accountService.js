import { connectivityModule } from "./connectivity.module"
import searchService from "../services/searchService.js"
function AccountUser(ObjInit){
    this.balance= ObjInit.balance;
    this.email= ObjInit.email;
    this.id= ObjInit.id;
    this.name= ObjInit.name;
    this.token= ObjInit.token;
    this.universityExists= ObjInit.universityExists;
    this.score = ObjInit.score;
    this.phoneNumber = ObjInit.phoneNumber;

}

function profileUserData(objInit){
    this.user= objInit.data;
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
    return arrInit[1].data.map(searchService.createAboutItem).map(item => {
        return {
            ...item,
            // user: arrInit[0].data,
        }
    }) || [];

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