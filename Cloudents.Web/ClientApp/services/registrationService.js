import axios from "axios";



export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration: (email,recaptcha) => axios.post("register", {email,captcha: recaptcha}),
    smsRegistration: (data) => axios.post("/Register/sms", {number: data}),
    smsCodeVerification: (data) => axios.post("/Register/sms/verify", {number: data}),
    signIn: (email,key,captcha) => axios.post("login", {email,key, captcha}),
    getAccountNum: () => axios.post("/Register/password"),

}