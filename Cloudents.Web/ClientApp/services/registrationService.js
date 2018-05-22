import axios from "axios";
import qs from "query-string";



export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration: (email,recaptcha) => instance.post("register", {email,captcha: recaptcha}),
    smsRegistration: (data) => axios.post("/Register/sms", {number: data}),
    smsCodeVerification: (data) => axios.post("/Register/sms/verify", {number: data}),
    getUserName: () => axios.get("/Register/userName"),
    setUserName: (data) => axios.post("/Register/userName", {name: data}),
    getAccountNum: () => axios.post("/Register/password"),
    signIn: (email,key,captcha) => instance.post("login", {email,key, captcha})
}