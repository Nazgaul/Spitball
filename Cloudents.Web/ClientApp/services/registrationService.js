import { connectivityModule } from "./connectivity.module"

export default {
    googleRegistration: (data) => {
        return connectivityModule.http.post("/register/google", {token: data});
    },
    emailRegistration: (firstName,lastName,email, recaptcha, password, confirmPassword) => {
        return connectivityModule.http.post("Register", {firstName,lastName, email, captcha: recaptcha, password, confirmPassword });
    },
    emailResend: () => {
        return connectivityModule.http.post("Register/resend");
    },
    smsRegistration: (code, phoneNumber) => {
        return connectivityModule.http.post("/sms", {
            "countryCode": code,
            "phoneNumber": phoneNumber
        });
    },
    smsCodeVerification: (data) => {
        return connectivityModule.http.post("/sms/verify", {number: data.code, fingerprint: data.fingerprint});
    },
    signIn: (data) => {
        return connectivityModule.http.post("LogIn", { email: data.email, password: data.password, fingerprint: data.fingerprint });
    },
    resendCode: () => {
        return connectivityModule.http.post("/sms/resend");
    },
    voiceConfirmation: () => {
        return connectivityModule.http.post("/sms/call");
    },
    getLocalCode: () => {
        return connectivityModule.http.get("/sms/code");
    },
    forgotPasswordReset: (email) => {
        return connectivityModule.http.post("ForgotPassword", {email});
    },
    EmailforgotPasswordResend: () => {
        return connectivityModule.http.post("ForgotPassword/resend");
    },
    updatePassword: (password, confirmPassword, id, code) => {
        return connectivityModule.http.post("ForgotPassword/reset", {id, code, password, confirmPassword});
    },
    validateEmail: (email) => {
        return connectivityModule.http.get(`LogIn/ValidateEmail?email=${email}`,);
    },
}