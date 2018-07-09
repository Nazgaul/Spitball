import axios from "axios";



export default {
    googleRegistration: (data) => axios.post("/Register/google", {token: data}),
    emailRegistration: (email,recaptcha) => axios.post("register", {email,captcha: recaptcha}),
    emailResend: () => axios.post("/Register/resend"),
    smsRegistration: (data) => axios.post("/sms", {number: `+${data}`}),
    smsCodeVerification: (data) => axios.post("/sms/verify", {number: data}),
    signIn: (email,captcha,rememberMe) => axios.post("login", {email, captcha, rememberMe}),
    resendCode: () => axios.post("/sms/resend"),
    getAccountNum: () => axios.post("/Register/password"),
    getLocalCode:()=>axios.get("/sms/code")
}