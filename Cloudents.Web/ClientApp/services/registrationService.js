import store from '../store/';
import axios from "axios";

const authInstance = axios.create({ 
    baseURL: '/api',
});

axios.interceptors.request.use((config) => {
    store.commit('setGlobalLoading', true);
    return config;
}, (error) => {
    return Promise.reject(error);
});

axios.interceptors.response.use((config) => {
    store.commit('setGlobalLoading', false);
    return config;
}, (error) => {
    return Promise.reject(error);
});

export default {
    googleRegistration: data => authInstance.post("/register/google", {token: data}),
    smsCodeVerification: data => authInstance.post("/sms/verify", {number: data.code, fingerprint: data.fingerprint}),
    signIn: data => authInstance.post("LogIn", { email: data.email, password: data.password, fingerprint: data.fingerprint }),
    resendCode: () => authInstance.post("/sms/resend"),
    voiceConfirmation: () => authInstance.post("/sms/call"),
    getLocalCode: () => authInstance.get("/sms/code"),
    forgotPasswordReset: email => authInstance.post("ForgotPassword", {email}),
    EmailforgotPasswordResend: () => authInstance.post("ForgotPassword/resend"),
    updatePassword: (password, confirmPassword, id, code) => authInstance.post("ForgotPassword/reset", {id, code, password, confirmPassword}),
    validateEmail: email => authInstance.get(`LogIn/ValidateEmail?email=${email}`),
    updateGrade: grade => authInstance.post(`Register/grade`, grade),
    updateParentStudentName: fullname => authInstance.post(`Register/childName`, fullname),
    updateUserRegisterType: type => authInstance.post('Register/userType', type),
    emailResend: () => authInstance.post("Register/resend"),
    emailRegistration: ({firstName, lastName, email, gender, recaptcha, password, confirmPassword}) => {
        return authInstance.post("Register", {firstName,lastName, email, gender, captcha: recaptcha, password, confirmPassword });
    },
    smsRegistration: (code, phoneNumber) => {
        return authInstance.post("/sms", {
            "countryCode": code,
            "phoneNumber": phoneNumber
        });
    },
}