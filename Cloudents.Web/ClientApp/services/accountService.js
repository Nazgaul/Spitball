import { connectivityModule } from "./connectivity.module"
import {dollarCalculate} from "../store/constants";

function AccountUser(ObjInit){
    this.balance= ObjInit.balance
    this.email= ObjInit.email
    this.id= ObjInit.id
    this.name= ObjInit.name
    this.token= ObjInit.token
    this.universityExists= ObjInit.universityExists
}

function ProfileData(arrInit){
    this.user= arrInit[0].data;
    this.questions = arrInit[1].data.map(item => {
        return {
            ...item,
            user: arrInit[0].data,
            answersNum: item.answers,
            filesNum: item.files,
        }
    }) || [];
    this.answers= arrInit[2].data.map(i => {
        return {
            ...i,
            answersNum: i.answers,
            filesNum: i.files,
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
    getProfileQuestions:(id, page) => {
        let strPage = page ? `?page=${page}` : ""
        return connectivityModule.http.get(`Profile/${id}/questions/${strPage}`)
    },
    getProfileAnswers:(id, page) => {
        let strPage = page ? `?page=${page}` : ""
        return connectivityModule.http.get(`/Profile/${id}/answers/${strPage}`)
    },
    calculateDollar:(balance)=> {
        return dollarCalculate(balance).toFixed(2)
    },
    createProfileData: (arrInit)=>{
        return new ProfileData(arrInit);
    }
}