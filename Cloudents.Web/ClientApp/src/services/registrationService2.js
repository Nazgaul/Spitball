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


function googleRegistration(userType) {
    
    // if (window.Android) {
    //     Android.onLogin();
    //     return Promise.reject();
    // }

    let gapiInstance = gapi.auth2.getAuthInstance();
    
    return gapiInstance.signIn().then((googleUser) => {
        let token = googleUser.getAuthResponse().id_token;
        return authInstance.post("/register/google", { token, userType })
    }, error => {
        return Promise.reject(error);
    });
}


export default {
    googleRegistration,
    emailLogin: logObj => authInstance.post("/LogIn", logObj),
    emailRegistration: regObj => authInstance.post("/Register", regObj),
    smsRegistration: smsObj => authInstance.post("/sms", smsObj),
    voiceConfirmation: () => authInstance.post("/sms/call"),
    smsCodeVerification: data => authInstance.post("/sms/verify", data),
    getLocalCode: () => authInstance.get("/sms/code"),
    sendSmsCode: () => authInstance.post("/sms/sendCode"),
    resendCode: () => authInstance.post("/sms/resend"),
    forgotPasswordReset: email => authInstance.post("ForgotPassword", {email}),
    emailforgotPasswordResend: () => authInstance.post("ForgotPassword/resend"),
}