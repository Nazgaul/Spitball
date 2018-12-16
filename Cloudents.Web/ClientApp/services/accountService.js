import { connectivityModule } from "./connectivity.module"
import searchService from "../services/searchService.js"
function AccountUser(ObjInit){
    this.balance= ObjInit.balance
    this.email= ObjInit.email
    this.id= ObjInit.id
    this.name= ObjInit.name
    this.token= ObjInit.token
    this.universityExists= ObjInit.universityExists
    this.score = ObjInit.score
}

function ProfileData(arrInit){
    this.user= arrInit[0].data;
    this.questions = arrInit[1].data.map(searchService.createQuestionItem).map(item => {
        return {
            ...item,
            user: arrInit[0].data,
            filesNum: item.files,
        }
    }) || [];
    this.answers= arrInit[2].data.map(searchService.createQuestionItem).map(i => {
        return {
            ...i,
            filesNum: i.files,
        }
    }) || [];
    this.documents= arrInit[3].data.map(searchService.createDocumentItem) || [];
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
    getProfileQuestions:(id, page) => {
        let strPage = page ? `?page=${page}` : ""
        return connectivityModule.http.get(`Profile/${id}/questions/${strPage}`)
    },
    getProfileAnswers:(id, page) => {
        let strPage = page ? `?page=${page}` : ""
        return connectivityModule.http.get(`/Profile/${id}/answers/${strPage}`)
    },
    getProfileDocuments:(id, page) => {
        let strPage = page ? `?page=${page}` : ""
        return connectivityModule.http.get(`/Profile/${id}/documents/${strPage}`)
    },
    createProfileData: (arrInit)=>{
        return new ProfileData(arrInit);
    }
}