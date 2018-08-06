import { connectivityModule } from "./connectivity.module"

export default {
    googleRegistration: (data) => { 
        //Ram change
        return connectivityModule.http.post("/SignUser/google", {token: data})
    }, 
    emailRegistration: (email, recaptcha) => {
        //Ram change
        return connectivityModule.http.post("signuser", {email,captcha: recaptcha})
    }, 
    emailResend: () => {
        return connectivityModule.http.post("/SignUser/resend")
    },
    smsRegistration: (data) => {
        return connectivityModule.http.post("/sms", {number: `+${data}`})
    },
    smsCodeVerification: (data) => {
        return connectivityModule.http.post("/sms/verify", {number: data})},
    signIn: (email, captcha) => {
        //Ram change
        return connectivityModule.http.post("signuser", {email, captcha})
    }, 
    resendCode: () => {
        return connectivityModule.http.post("/sms/resend")
    },
    getAccountNum: () => {
        return connectivityModule.http.post("/Register/password")
    },
    getLocalCode:()=> {
        return connectivityModule.http.get("/sms/code")
    }
}