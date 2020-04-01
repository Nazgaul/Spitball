import store from '../store/';
import axios from "axios";

const authInstance = axios.create({ 
    baseURL: '/api',
});

authInstance.interceptors.request.use((config) => {
    store.commit('setGlobalLoading', true);
    return config;
}, (error) => {
    return Promise.reject(error);
});

authInstance.interceptors.response.use((config) => {
    store.commit('setGlobalLoading', false);
    return config;
}, (error) => {
    store.commit('setGlobalLoading', false);
    return Promise.reject(error);
});


function googleRegistration() {
    if (window.Android) {
        Android.onLogin();
        return Promise.reject();
    }

    let gapiInstance = gapi.auth2.getAuthInstance();
    
    return gapiInstance.signIn().then((googleUser) => {
        let token = googleUser.getAuthResponse().id_token;
        return authInstance.post("/register/google", { token })
    }, error => {
        return Promise.reject(error);
    });
}


export default {
    googleRegistration,
    // googleRegistration: data => authInstance.post("/register/google", {token: data}),
    smsCodeVerification: data => authInstance.post("/sms/verify", data),
    signIn: data => authInstance.post("LogIn", { email: data.email, password: data.password, fingerprint: data.fingerprint }),
    resendCode: () => authInstance.post("/sms/resend"),
    voiceConfirmation: () => authInstance.post("/sms/call"),
    getLocalCode: () => authInstance.get("/sms/code"),
    forgotPasswordReset: email => authInstance.post("ForgotPassword", {email}),
    EmailforgotPasswordResend: () => authInstance.post("ForgotPassword/resend"),
    updatePassword: (password, confirmPassword, id, code) => authInstance.post("ForgotPassword/reset", {id, code, password, confirmPassword}),
    validateEmail: email => authInstance.get(`LogIn/ValidateEmail?email=${email}`),
    updateGrade: grade => authInstance.post(`Register/grade`, grade),
    updateParentStudentName: parentObj => authInstance.post(`Register/childName`, parentObj),
    updateUserRegisterType: type => authInstance.post('Register/userType', type),
    emailResend: () => authInstance.post("Register/resend"),
    // emailRegistration: ({firstName, lastName, email, gender, recaptcha, password, confirmPassword}) => {
    //     return authInstance.post("Register", {firstName,lastName, email, gender, captcha: recaptcha, password, confirmPassword });
    // },
    emailRegistration2: regObj => authInstance.post("Register", regObj),
    smsRegistration: (code, phoneNumber) => {
        return authInstance.post("/sms", {
            "countryCode": code,
            "phoneNumber": phoneNumber
        });
    },
}