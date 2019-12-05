// GLOBALS:
import {router} from '../main.js';
import codesJson from '../components/loginPageNEW/helpers/CountryCallingCodes';
const isIl = global.country.toLowerCase() === 'il';
const defaultSubmitRoute = {path: '/feed'};

const Fingerprint2 = require('fingerprintjs2');

// SERVICES:
import registrationService from '../services/registrationService.js'
import { LanguageService } from "../services/language/languageService.js";

// FUNCTIONS:
function _dictionary(key){
    return LanguageService.getValueByKey(key);
}

const state = {
    currentStep: 'getStarted',
    stepsHistory: [],
    toUrl: '',

    firstName:'',
    lastName:'',
    email: '',
    phone: '',
    localCode: '',

    globalLoading: false,
    
    errorMessage: {
        phone: "",
        code: "",
        email: "",
        password: "",
        confirmPassword: "",
        gmail: ""
    },
    passScoreObj: {
        0: {name: _dictionary("login_password_indication_weak"), className: "bad"},
        1: {name: _dictionary("login_password_indication_weak"),className: "bad"},
        2: {name: _dictionary("login_password_indication_strong"),className: "good"},
        3: {name: _dictionary("login_password_indication_strong"),className: "good"},
        4: {name: _dictionary("login_password_indication_strongest"),className: "best"}
    }
};

const mutations = {
    setToUrl(state,url){
        state.toUrl = url;
    },
    setResetState(state){
        state.currentStep = 'getStarted';
        state.stepsHistory = [];

        state.email = '';
        state.phone = '';
        state.localCode = '';
        state.firstName = '';
        state.lastName = '';

        state.globalLoading = false;

        state.errorMessage = {
            phone: "",
            code: "",
            email: "",
            password: "",
            confirmPassword: "",
            gmail: ""
        };
    },
    setEmail(state,email){
        state.email = email;
    },
    setPhone(state,phoneNumber){
        state.phone = phoneNumber;
    },
    setPhoneCode(state,localCode){
        state.localCode = localCode;
    },
    setBackStep(state){
        let lastStep = state.stepsHistory.pop();
        state.currentStep = lastStep;
    },
    setStepHistory(state){
        state.stepsHistory.push(state.currentStep);
    },
    setResetStepHistory(state){
        state.stepsHistory = [];
    },
    setCurrentStep(state,stepName){
        state.currentStep = stepName;
    },

    setGlobalLoading(state,value){
        state.globalLoading = value;
    },
    setErrorMessages(state,errorMessagesObj){
        state.errorMessage.gmail = (errorMessagesObj.gmail)? errorMessagesObj.gmail : '';
        state.errorMessage.phone = (errorMessagesObj.phone)? errorMessagesObj.phone : '';
        state.errorMessage.code = (errorMessagesObj.code)? errorMessagesObj.code : '';
        state.errorMessage.email = (errorMessagesObj.email)? errorMessagesObj.email : '';
        state.errorMessage.password = (errorMessagesObj.password)? errorMessagesObj.password : '';
        state.errorMessage.confirmPassword = (errorMessagesObj.confirmPassword)? errorMessagesObj.confirmPassword : '';
    },
    setLocalCode(state,localCode){
        state.localCode = localCode;
    },
    setName(state,fullNameObj){
        state.firstName = fullNameObj.firstName;
        state.lastName = fullNameObj.lastName;
    }
};

const getters = {
    getCurrentLoginStep: state => state.currentStep,
    getEmail1: state => state.email,
    getPhone: state => state.phone,
    getCountryCodesList: () => codesJson.sort((a, b) => a.name.localeCompare(b.name)),
    getLocalCode: state => state.localCode,
    getGlobalLoading: state => state.globalLoading,
    getErrorMessages: state => state.errorMessage,
    getPassScoreObj: state => state.passScoreObj
};

const actions = {
    updateToUrl({commit},url){
        commit('setToUrl',url);
    },
    updateName({commit},fullNameObj){
        commit('setName',fullNameObj)
    },
    updateEmail({commit},email){
        commit('setEmail',email);
    },
    updatePhone({commit},phoneNumber){
        commit('setPhone',phoneNumber);
    },    
    updateLocalCode({commit},selectedLocalCode){
        if(selectedLocalCode){
            commit('setLocalCode',selectedLocalCode);
        } else {
            registrationService.getLocalCode().then(({data}) => {
                commit('setLocalCode',data.code)});
        }
    },
    updateStep({commit,state,dispatch},stepName){
        let specialSteps = ["setphone", "verifyphone", "resetpassword"];

        if(specialSteps.includes(stepName.toLowerCase())){
            commit('setResetStepHistory');
            commit('setCurrentStep',stepName);
            dispatch('updateAnalytics_unitedEvent',['Registration', 'Email Verified']);
            return;
        }
        if(!stepName){
            commit('setResetState');
        }else if(state.stepsHistory.includes(stepName)){
            commit('setBackStep');
        }else {
            history.pushState({}, null);
            commit('setStepHistory');
            commit('setCurrentStep',stepName);
        }
    },
    goBackStep({dispatch,state}){
        let stepIndex = state.stepsHistory.length - 1;
        let lastStep = state.stepsHistory[stepIndex];
        dispatch('updateStep',lastStep);
    },
    googleSigning({dispatch,commit}){
        let authInstance = gapi.auth2.getAuthInstance();

        return authInstance.signIn().then((googleUser) => {
            let idToken = googleUser.getAuthResponse().id_token;
            return registrationService.googleRegistration(idToken).then((resp) => {
                let newUser = resp.data.isNew;
                if (newUser) {
                    dispatch('updateAnalytics_unitedEvent',['Registration', 'Start Google']);
                    dispatch('updateStep','setPhone');
                } else {
                    dispatch('updateAnalytics_unitedEvent',['Login', 'Start Google']);
                    global.isAuth = true;
                    let lastRoute = router.history.current.query.returnUrl || 'feed';
                    router.push({path: `${lastRoute}`});
                }
                return Promise.reject(error);
                }, (error) => {
                    commit('setErrorMessages',{gmail: error.response.data["Google"] ? error.response.data["Google"][0] : ''});
                    return Promise.reject(error);
                });
        },error=>{
            return Promise.reject(error);
            
        });
    },
    emailSigning({dispatch,state,commit},params){
        let {recaptcha} = params;
        let {password} = params;
        let {confirmPassword} = params;
        commit('setGlobalLoading',true);
        return registrationService.emailRegistration(state.firstName,state.lastName,state.email, recaptcha, password, confirmPassword)
            .then((resp) => {
                let nextStep = resp.data.step;
                if(nextStep.toLowerCase() === "verifyphone" || nextStep.toLowerCase() === "enterphone"){
                    dispatch('updateStep','setPhone');
                }else{
                    dispatch('updateStep',nextStep);
                }
                dispatch('updateAnalytics_unitedEvent',['Registration', 'Start']);
                commit('setGlobalLoading',false);
            },  (error) => {
                commit('setGlobalLoading',false);
                commit('setErrorMessages',{
                    email: error.response.data["Email"] ? error.response.data["Email"][0] : '',
                    password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                    confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : ''
                });
                return Promise.reject(error);
            });
    },
    resendEmail({dispatch}){
        dispatch('updateAnalytics_unitedEvent',['Registration', 'Resend Email']);
        registrationService.emailResend()
            .then(response => {
                dispatch('updateToasterParams', {
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                });
                },
                error => {
                    dispatch('updateToasterParams', {
                        toasterText: LanguageService.getValueByKey("put some error"),
                        showToaster: true,
                        toasterType: 'error-toaster'
                    });
                });
    },
    resetState({commit}){
        commit('setResetState');
    },
    sendSMScode({dispatch,commit,state}){
        commit('setGlobalLoading',true);
        registrationService.smsRegistration(state.localCode,state.phone)
            .then(function (resp){
                commit('setErrorMessages',{});
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_verification_code_sent_to_phone"),
                    showToaster: true,
                });
                commit('setGlobalLoading',false);
                dispatch('updateAnalytics_unitedEvent',['Registration', 'Phone Submitted']);
                dispatch('updateStep','VerifyPhone');
            }, function (error){
                commit('setGlobalLoading',false);
                commit('setErrorMessages',{phone: error.response.data["PhoneNumber"]? error.response.data["PhoneNumber"][0]:'' });

                // if(error.response.data["PhoneNumber"] && error.response.data["PhoneNumber"][0]){
                //     if(error.response.data["PhoneNumber"][0] === "InvalidPhoneNumber"){
                //         commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error_invalid")})
                //     } else {
                //         commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error")})
                //     }
                // }else{
                //     commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error_tryagain")}) 
                // }
            });
    },
    smsCodeVerify({dispatch,commit},smsCode) {
        let data = {
            code: smsCode,
            fingerprint: ""
        };
        commit('setGlobalLoading',true);

        Fingerprint2.getPromise({})
            .then(components => {
                let values = components.map(component => component.value);
                let murmur = Fingerprint2.x64hash128(values.join(''), 31);
                data.fingerprint = murmur;
                registrationService.smsCodeVerification(data)
                    .then(userId => {
                            dispatch('updateStep','congrats');
                            dispatch('updateAnalytics_unitedEvent',['Registration', 'Phone Verified']);
                            if(!!userId){
                                dispatch('updateAnalytics_unitedEvent',['Registration', 'User Id', userId.data.id]);
                            }
                            commit('setGlobalLoading',false);
                    }, error =>{
                        commit('setGlobalLoading',false);
                        commit('setErrorMessages',{code: "Invalid code"});
                    });
            });
    },
    callWithCode({dispatch,commit}){
        commit('setGlobalLoading',false);
        dispatch('updateAnalytics_unitedEvent',['Registration', 'Call Voice SMS']);
        registrationService.voiceConfirmation()
            .then((success) => {
                commit('setGlobalLoading',false);
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_call_code"),
                    showToaster: true,
                });
            },error => {
                commit('setErrorMessages',{code: error.text});
            });
    },
    changeNumber({dispatch,commit}){
        commit('setResetState');
        dispatch('updateStep','setPhone');
    },
    finishRegister({dispatch}){
        dispatch('updateAnalytics_unitedEvent',['Registration', 'Congrats']);
        global.isAuth = true;
        router.push({name:'studentTutor'});
    },
    emailValidate({dispatch,commit,state}) {
        commit('setGlobalLoading',true);
        registrationService.validateEmail(encodeURIComponent(state.email))
                .then((response) => {
                    commit('setGlobalLoading',false);
                    dispatch('updateAnalytics_unitedEvent',['Login Email validation', 'email send']);
                    dispatch('updateStep','setPassword');
                }, (error)=> {
                    commit('setGlobalLoading',false);
                    commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''});
                });
    },
    logIn({commit,state,dispatch},password){
        commit('setGlobalLoading',true);
        let data = {
            email: state.email,
            password: password,
            fingerprint: ""
        };

        Fingerprint2.getPromise({})
            .then(components =>{
                let values = components.map(component => component.value);
                let murmur = Fingerprint2.x64hash128(values.join(''), 31);
                data.fingerprint = murmur;
                registrationService.signIn(data)
                    .then(response =>{
                        commit('setGlobalLoading',false);
                        dispatch('updateAnalytics_unitedEvent',['Login', 'Start']);
                        global.isAuth = true;
                        global.country = response.data.country;
                        let url = state.toUrl || defaultSubmitRoute;
                        router.push({ path: `${url.path}` });
                    },error =>{
                        commit('setGlobalLoading',false);
                        commit('setErrorMessages',{email: error.response.data["Password"] ? error.response.data["Password"][0] : ''});
                    });
            });
    },
    resetPassword({dispatch,state,commit}){
        commit('setGlobalLoading',true);
        registrationService.forgotPasswordReset(state.email)
            .then(response =>{
                commit('setGlobalLoading',false);
                dispatch('updateAnalytics_unitedEvent',['Forgot Password', 'Reset email send']);
                dispatch('updateStep','EmailConfirmed');
            },error =>{
                commit('setGlobalLoading',false);
                commit('setErrorMessages',{email: error.response.data["ForgotPassword"] ? error.response.data["ForgotPassword"][0] : error.response.data["Email"][0]});
            });
    },
    resendEmailPassword({dispatch,commit}){
        commit('setGlobalLoading',true);
        dispatch('updateAnalytics_unitedEvent',['Registration', 'Resend Email']);
        registrationService.EmailforgotPasswordResend()
            .then(response => {
                commit('setGlobalLoading',false);
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                });
            },error => {
                commit('setGlobalLoading',false);
            });
    },
    changePassword({state,commit,dispatch},params) {
        let {id} = params;
        let {code} = params;
        let {password} = params;
        let {confirmPassword} = params;
        let isValid = (password === confirmPassword);
        if(isValid){
            commit('setGlobalLoading',true);
            registrationService.updatePassword(password, confirmPassword, id, code)
                .then((response) => {
                    dispatch('updateAnalytics_unitedEvent',['Forgot Password', 'Updated password']);
                    global.isAuth = true;
                    commit('setGlobalLoading',false);
                    let url = state.toUrl || defaultSubmitRoute;
                    router.push({path: `${url.path }`});
                }, (error) => {
                    commit('setGlobalLoading',false);
                    commit('setErrorMessages',{
                        password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                        confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : ''});
                });
        }else{
            commit('setErrorMessages',{confirmPassword: _dictionary('login_error_not_matched') });
        }
    },
    exit({commit}){
        // let url = state.toUrl || defaultSubmitRoute;
        commit('setResetState');
        router.push({path: `/`});
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}