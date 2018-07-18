import axios from "axios";



export default {
    googleRegistration: (data) => axios.post("/SignUser/google", {token: data}), //Ram change
    emailRegistration: (email,recaptcha) => axios.post("signuser", {email,captcha: recaptcha}), //Ram change
    emailResend: () => axios.post("/SignUser/resend"),
    smsRegistration: (data) => axios.post("/sms", {number: `+${data}`}),
    smsCodeVerification: (data) => axios.post("/sms/verify", {number: data}),
    signIn: (email, captcha) => axios.post("signuser", {email, captcha}), //Ram change
    resendCode: () => axios.post("/sms/resend"),
    getAccountNum: () => axios.post("/Register/password"),
    getLocalCode:()=>axios.get("/sms/code")
}