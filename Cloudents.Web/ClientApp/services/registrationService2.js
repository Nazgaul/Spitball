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
    googleRegistration, // ok
    emailLogin: logObj => authInstance.post("/LogIn", logObj), // ok
    emailRegistration: regObj => authInstance.post("/Register", regObj), // ok
    resetPassword: email => authInstance.post("/ForgotPassword", email), // ok
    emailForgotPasswordResend: () => authInstance.post("/ForgotPassword/resend"), // not ok ?? not sure if need it anymore @idan
    smsRegistration: smsObj => authInstance.post("/sms", smsObj), // ok
    voiceConfirmation: () => authInstance.post("/sms/call"), // ok
    smsCodeVerification: data => authInstance.post("/sms/verify", data), // ok
    updatePassword: (password, confirmPassword, id, code) => authInstance.post("ForgotPassword/reset", {id, code, password}), // not ok // what about change password
    // emailResend: () => authInstance.post("Register/resend"),
    // resendCode: () => authInstance.post("/sms/resend"),
    getLocalCode: () => authInstance.get("/sms/code"), // ok
    validateEmail: email => authInstance.get(`/LogIn/ValidateEmail`, { params: { email } }), // not ok ?? not sure if need it anymore @idan
}