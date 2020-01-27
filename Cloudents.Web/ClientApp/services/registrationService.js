import { connectivityModule } from "./connectivity.module"
import axios from "axios";

export default {
    googleRegistration: (data) => {
        return connectivityModule.http.post("/register/google", {token: data});
    },
    emailRegistration: ({firstName, lastName, email, gender, recaptcha, password, confirmPassword}) => {
        return connectivityModule.http.post("Register", {firstName,lastName, email, gender, captcha: recaptcha, password, confirmPassword }); // TODO add gendre
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
        return connectivityModule.http.get(`LogIn/ValidateEmail?email=${email}`);
    },
    updateGrade: grade => axios.post(`Register/grade`, grade),

    updateParentStudentName: fullname => axios.post(`Register/childName`, fullname),
    
    updateUserRegisterType: type => axios.post('Register/userType', type),
}