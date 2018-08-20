import { connectivityModule } from "./connectivity.module"
import {dollarCalculate} from "../store/constants";


export default {
    getAccount:() => {
       return connectivityModule.http.get("/Account")
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