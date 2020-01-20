import { connectivityModule } from "./connectivity.module";
import searchService from "../services/searchService.js";

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
function ProfileQuestionData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
}
function ProfileAnswerData(arrInit){
    return arrInit.data.map(searchService.createQuestionItem) || [];
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
    becomeTutor: (data) => {
        return connectivityModule.http.post("/Account/becomeTutor", data);
    },
    createProfileQuestionData: (arrInit)=>{
        return new ProfileQuestionData(arrInit);
    },
    createProfileAnswerData: (arrInit)=>{
        return new ProfileAnswerData(arrInit);
    },
}