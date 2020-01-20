import { connectivityModule } from "./connectivity.module";
import {User,Item} from './constructors/constructors.js'

import searchService from "../services/searchService.js";
function itemTypeChcker(type){
    if(type.toLowerCase() === 'document'){
       return 'Document';
    }
    if(type.toLowerCase() === 'video'){
        return 'Video';
    }
 }

function createProfileItems(objInit){
    return Object.assign(
        {
            result: objInit.data.result.map(objData => {
                return new Item[itemTypeChcker(objData.documentType)](objData)
            }),
            count: objInit.data.count,
        }
    )
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
    getProfileDocuments:(id, page,pageSize) => {
        let strPage = `?page=${page}&pageSize=${pageSize}`;
        return connectivityModule.http.get(`/Profile/${id}/documents/${strPage}`).then(createProfileItems);
    },
    // getProfileDocuments:(id, page) => {
    //     let strPage = page ? `?page=${page}` : "";
    //     return connectivityModule.http.get(`/Profile/${id}/documents/${strPage}`);
    // },
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