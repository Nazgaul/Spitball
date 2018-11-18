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
        return connectivityModule.http.get("/Profile/" + id)
    },
    calculateDollar:(balance)=> {
        return dollarCalculate(balance).toFixed(2)
    }
}