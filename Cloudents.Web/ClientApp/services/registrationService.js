import axios from "axios";



export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration: (email,recaptcha) => axios.post("register", {email,captcha: recaptcha}),
    emailResend: () => axios.post("/Register/resend"),
    smsRegistration: (data) => axios.post("/Register/sms", {number: data}),
    smsCodeVerification: (data) => axios.post("/Register/sms/verify", {number: data}),
    signIn: (email,key,captcha,rememberMe) => axios.post("login", {email,key, captcha, rememberMe}),
    getAccountNum: () => axios.post("/Register/password"),

}