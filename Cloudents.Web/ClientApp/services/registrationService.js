import { connectivityModule } from "./connectivity.module"

export default {
    googleRegistration: (data) => {
        return connectivityModule.http.post("/register/google", {token: data})
    },
    emailRegistration: (email, recaptcha, password, confirmPassword) => {
        return connectivityModule.http.post("Register", {email, captcha: recaptcha, password, confirmPassword})
    },
    emailResend: () => {
        return connectivityModule.http.post("Register/resend")
    },
    smsRegistration: (code, phoneNumber) => {
        return connectivityModule.http.post("/sms", {
            "countryCode": code,
            "phoneNumber": phoneNumber
        })
    },
    smsCodeVerification: (data) => {
        return connectivityModule.http.post("/sms/verify", {number: data})
    },
    signIn: (email, captcha, password) => {
        return connectivityModule.http.post("LogIn", {email, captcha, password})
    },
    resendCode: () => {
        return connectivityModule.http.post("/sms/resend")
    },
    getAccountNum: () => {
        return connectivityModule.http.post("/Register/password")
    },
    getLocalCode: () => {
        return connectivityModule.http.get("/sms/code")
    },
    forgotPasswordReset: (email) => {
        return connectivityModule.http.post("ForgotPassword", {email})
    },
    EmailforgotPasswordResend: () => {
        return connectivityModule.http.post("ForgotPassword/resend")
    },
    updatePassword: (password, confirmPassword, id, code) => {
        return connectivityModule.http.post("ForgotPassword/reset", {id, code, password, confirmPassword})
    },

}