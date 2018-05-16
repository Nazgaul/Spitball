import axios from "axios";
import qs from "query-string";

axios.defaults.paramsSerializer = params => qs.stringify(params, {indices: false});
axios.defaults.responseType = "json";
var instance = axios.create({
    baseURL : "/api"
});

export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration(email,recaptcha) {
        return instance.post("register", qs.stringify( {email,captcha: recaptcha}));
    },
    smsRegistration: (data) => axios.post("/Register/sms", {number: data}),
    smsCodeVerification: (data) => axios.post("/Register/sms/verify", {number: data}),
    getUserName: () => axios.get("/Register/userName"),
    setUserName: (data) => axios.post("/Register/userName", {name: data}),
    getAccountNum: () => axios.post("/Register/password"),
    signIn(email,key,captcha) {
        return instance.post("login", qs.stringify( {email,key, captcha}));
    }
}