import {User} from './Dto/user.js';
import axios from 'axios'

const accountInstance = axios.create({
    baseURL:'/api/account'
})

import { connectivityModule } from "./connectivity.module";

export default {
    async getAccount(){ 
        let {data} = await accountInstance.get()
        return new User.Account(data)
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
}