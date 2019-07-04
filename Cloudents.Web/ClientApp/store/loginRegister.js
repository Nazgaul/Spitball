// GLOBALS:
import {router} from '../main.js';
import codesJson from '../components/loginPageNEW/helpers/CountryCallingCodes';
const isIl = global.country.toLowerCase() === 'il';
const defaultSubmitRoute = isIl ? {path: '/note'} : {path: '/ask'};
const Fingerprint2 = require('fingerprintjs2');

// SERVICES:
import analyticsService from '../services/analytics.service.js'
import registrationService from '../services/registrationService.js'
import { LanguageService } from "../services/language/languageService.js";

// FUNCTIONS:
function _dictionary(key){
    return LanguageService.getValueByKey(key) 
}
function _analytics (params){
    analyticsService.sb_unitedEvent(...params);
}

const state = {
    currentStep: 'getStarted',
    stepsHistory: [],
    toUrl: '',

    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    localCode: '',
    smsCode: '',

    recaptcha: "",
    resetRecaptcha: false,
    siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
    
    googleLoading: false,
    globalLoading: false,
    
    errorMessage: {
        phone: "",
        code: "",
        email: "",
        password: "",
        confirmPassword: "",
        gmail: "",
    },
    passScoreObj: {
        0: {name: _dictionary("login_password_indication_weak"), className: "bad"},
        1: {name: _dictionary("login_password_indication_weak"),className: "bad"},
        2: {name: _dictionary("login_password_indication_strong"),className: "good"},
        3: {name: _dictionary("login_password_indication_strong"),className: "good"},
        4: {name: _dictionary("login_password_indication_strongest"),className: "best"}
    },
}

const mutations = {
    setToUrl(state,url){
        state.toUrl = url
    },
    setResetState(state){
        state.currentStep = 'getStarted'
        state.stepsHistory = []
        state.email = ''
        state.password = ''
        state.confirmPassword = ''
        state.phone = ''
        state.smsCode = ''
        state.recaptcha = ''
        state.resetRecaptcha = false
        state.googleLoading = false
        state.globalLoading = false
        state.errorMessage = {
            phone: "",
            code: "",
            email: "",
            password: "",
            confirmPassword: "",
            gmail: "",
        }
    },
    setEmail(state,email){
        state.email = email
    },
    setPassword(state,password){
        state.password = password
    },
    setConfirmPassword(state,confirmPassword){
        state.confirmPassword = confirmPassword
    },
    setPhone(state,phoneNumber){
        state.phone = phoneNumber;
    },
    setPhoneCode(state,localCode){
        state.localCode = localCode;
    },
    setSmsCode(state,smsCode){
        state.smsCode = smsCode
    },
    setStep(state,stepName){
        state.stepsHistory.push(state.currentStep);
        state.currentStep = stepName
    },
    setBackStep(state){
        let lastStep = state.stepsHistory.pop()
        state.currentStep = lastStep
    },
    setStepFromEmail(state,stepName){
        state.stepsHistory = []
        state.currentStep = stepName
    },
    setGlobalLoading(state,value){
        state.globalLoading = value
    },
    setGoogleLoading(state,value){
        state.googleLoading = value
    },
    setRecaptcha(state,recaptcha){
        state.recaptcha = recaptcha
    },
    setErrorMessages(state,errorMessagesObj){
        state.errorMessage.gmail = (errorMessagesObj.gmail)? errorMessagesObj.gmail : ''
        state.errorMessage.phone = (errorMessagesObj.phone)? errorMessagesObj.phone : ''
        state.errorMessage.code = (errorMessagesObj.code)? errorMessagesObj.code : ''
        state.errorMessage.email = (errorMessagesObj.email)? errorMessagesObj.email : ''
        state.errorMessage.password = (errorMessagesObj.password)? errorMessagesObj.password : ''
        state.errorMessage.confirmPassword = (errorMessagesObj.confirmPassword)? errorMessagesObj.confirmPassword : ''
    },
    setResetRecaptcha(state){
        state.resetRecaptcha = true;
    },
    setLocalCode(state,localCode){
        state.localCode = localCode
    },
}

const getters = {
    getCurrentLoginStep: state => state.currentStep,

    getEmail1: state => state.email,
    getPhone: state => state.phone,
    getCountryCodesList: () => codesJson.sort((a, b) => a.name.localeCompare(b.name)),
    getLocalCode: state => state.localCode,

    getGlobalLoading: state => state.globalLoading,
    getGoogleLoading: state => state.googleLoading,

    getSiteKey: state => state.siteKey,
    getIsReCaptcha: state => (state.recaptcha)? true:false,
    getResetRecaptcha: state => state.resetRecaptcha,

    getErrorMessages: state => state.errorMessage,
    getPassScoreObj: state => state.passScoreObj,

    getIsFormValid: state => (state.email && state.password.length >= 8 && state.recaptcha),
    getIsPhoneFormValid: state => (state.phone.length >= 9 && state.phone.length < 11)
}

const actions = {
    updateToUrl({commit},url){
        commit('setToUrl',url)
    },
    updateRecaptcha({commit},recaptcha){
        commit('setRecaptcha',recaptcha)
    },
    updateEmail({commit},email){
        commit('setEmail',email)
    },
    updatePassword({commit},password){
        commit('setPassword',password)
    },
    updateConfirmPassword({commit},confirmPassword){
        commit('setConfirmPassword',confirmPassword)
    },
    updatePhone({commit},phoneNumber){
        commit('setPhone',phoneNumber)
    },    
    updateLocalCode({commit},selectedLocalCode){
        if(selectedLocalCode){
            commit('setLocalCode',selectedLocalCode)
        } else {
            registrationService.getLocalCode().then(({data}) => {
                commit('setLocalCode',data.code)});
        }
    },
    updateSmsCode({commit},smsCode){
        commit('setSmsCode',smsCode)
    },
    updateStep({commit,state},stepName){
        
        if(stepName === "setPhone" || stepName === 'VerifyPhone' || stepName === 'resetPassword'){
            commit('setStepFromEmail',stepName)
            _analytics(['Registration', 'Email Verified'])
            return
        }
        if(!stepName){
            commit('setResetState')
        }else if(state.stepsHistory.includes(stepName)){
            commit('setBackStep')
        }else {
            history.pushState({}, null);
            commit('setStep',stepName)
        }
    },
    goBackStep({dispatch,state}){
        let stepIndex = state.stepsHistory.length - 1
        let lastStep = state.stepsHistory[stepIndex]
        dispatch('updateStep',lastStep)
    },
    googleSigning({dispatch,commit}){
        commit('setGoogleLoading',true)
        let authInstance = gapi.auth2.getAuthInstance();
        authInstance.signIn().then((googleUser) => {
            let idToken = googleUser.getAuthResponse().id_token;
            registrationService.googleRegistration(idToken)
                .then((resp) => {
                    commit('setGoogleLoading',false)
                    let newUser = resp.data.isNew;
                    if (newUser) {
                        _analytics(['Registration', 'Start Google'])
                        dispatch('updateStep','setPhone')
                    } else {
                        _analytics(['Login', 'Start Google'])
                        global.isAuth = true;
                        router.push({path: `${defaultSubmitRoute.path}`});
                    }
                }, (error) => {
                    commit('setGoogleLoading',false)
                    commit('setErrorMessages',{gmail: error.response.data["Google"] ? error.response.data["Google"][0] : ''})
                });
        }, (error) => {
            commit('setGoogleLoading',false)
        });
    },
    emailSigning({dispatch,state,commit}){
        commit('setGlobalLoading',true)
        return registrationService.emailRegistration(state.email, state.recaptcha, state.password, state.confirmPassword)
            .then((resp) => {
                let nextStep = resp.data.step;
                if(nextStep === "VerifyPhone"){
                    dispatch('updateStep','setPhone')
                }else{
                    dispatch('updateStep',nextStep)
                }
                _analytics(['Registration', 'Start'])
                commit('setGlobalLoading',false)
            },  (error) => {
                console.log('STORE ERROR set email password');
                dispatch('updateRecaptcha','')
                commit('setResetRecaptcha')
                commit('setGlobalLoading',false)
                commit('setErrorMessages',{
                    email: error.response.data["Email"] ? error.response.data["Email"][0] : '',
                    password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                    confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : '',
                })
                // reset recaptcha in component
                return Promise.reject(error);
            });
    },
    resendEmail({dispatch}){
        _analytics(['Registration', 'Resend Email'])
        registrationService.emailResend()
            .then(response => {
                dispatch('updateToasterParams', {
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                });
                },
                error => {
                    // self.updateLoading(false);
                })
    },
    resetState({commit}){
        commit('setResetState')
    },
    sendSMScode({dispatch,commit,state}){
        commit('setGlobalLoading',true)
        registrationService.smsRegistration(state.localCode,state.phone)
            .then(function (resp){
                commit('setErrorMessages',{})
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_verification_code_sent_to_phone"),
                    showToaster: true,
                })
                commit('setGlobalLoading',false)
                _analytics(['Registration', 'Phone Submitted'])
                dispatch('updateStep','VerifyPhone')
            }, function (error){
                commit('setGlobalLoading',false)
                if(error.response.data["PhoneNumber"] && error.response.data["PhoneNumber"][0]){
                    if(error.response.data["PhoneNumber"][0] === "InvalidPhoneNumber"){
                        commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error_invalid")})
                    } else {
                        commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error")})
                    }
                }else{
                    commit('setErrorMessages',{phone: _dictionary("loginRegister_smsconfirm_phone_error_tryagain")}) 
                }
            })
    },
    smsCodeVerify({dispatch,commit,state}) {
        let data = {
            code: state.smsCode,
            fingerprint: ""
        };
        commit('setGlobalLoading',true)

        Fingerprint2.getPromise({})
            .then(components => {
                let values = components.map(component => component.value)
                let murmur = Fingerprint2.x64hash128(values.join(''), 31)
                data.fingerprint = murmur;
                registrationService.smsCodeVerification(data)
                    .then(userId => {
                        // if(state.isNewUser){
                            dispatch('updateStep','congrats')
                            _analytics(['Registration', 'Phone Verified'])
                            if(!!userId){
                                _analytics(['Registration', 'User Id', userId.data.id])
                            }
                            commit('setGlobalLoading',false)
                        // }
                        // else {
                        //      commit('setGlobalLoading',false)
                        //     _analytics(['Login', 'Phone Verified'])
                        //     if(!!userId){
                        //         _analytics(['Registration', 'User Id', userId.data.id])
                        //     }
                        //     let lastActiveRoute = getters.lastActiveRoute
                        //     let url = lastActiveRoute || defaultSubmitRoute;
                        //     global.isAuth = true;
                        //     router.push({path: `${url.path}`});
                        // }
                    }, error =>{
                        commit('setGlobalLoading',false)
                        commit('setErrorMessages',{code: "Invalid code"}) 
                    })
            })     
    },
    callWithCode({dispatch,commit}){
        commit('setGlobalLoading',false)
        _analytics(['Registration', 'Call Voice SMS'])
        registrationService.voiceConfirmation()
            .then((success) => {
                commit('setGlobalLoading',false)                  
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_call_code"),
                    showToaster: true,
                })
            },error => {
                commit('setErrorMessages',{code: error.text})
            })
    },
    changeNumber({dispatch,commit}){
        commit('setResetState')
        dispatch('updateStep','setPhone')
    },
    finishRegister(){
        _analytics(['Registration', 'Congrats'])
        global.isAuth = true;
        router.push({name:'studentTutor'})
    },
    emailValidate({dispatch,commit,state}) {
        commit('setGlobalLoading',true)
            registrationService.validateEmail(encodeURIComponent(state.email))
                .then((response) => {
                    commit('setGlobalLoading',false)
                    _analytics(['Login Email validation', 'email send'])
                    dispatch('updateStep','setPassword')
                }, (error)=> {
                    commit('setGlobalLoading',false)
                    commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''})
                });
    },
    logIn({commit,state}){
        commit('setGlobalLoading',true)
        let data = {
            email: state.email,
            password: state.password,
            fingerprint: ""
        }

        Fingerprint2.getPromise({})
            .then(components =>{
                let values = components.map(component => component.value)
                let murmur = Fingerprint2.x64hash128(values.join(''), 31)
                data.fingerprint = murmur;
                registrationService.signIn(data)
                    .then(response =>{
                        commit('setGlobalLoading',false)
                        _analytics(['Login', 'Start'])
                        global.isAuth = true;
                        global.country = response.data.country;
                        let url = state.toUrl || defaultSubmitRoute;
                        router.push({ path: `${url.path}` });
                    },error =>{
                        commit('setGlobalLoading',false)
                        commit('setErrorMessages',{email: error.response.data["Password"] ? error.response.data["Password"][0] : ''}) 
                    })
            })
    },
    resetPassword({dispatch,state,commit}){
        commit('setGlobalLoading',true)
        registrationService.forgotPasswordReset(state.email)
            .then(response =>{
                commit('setGlobalLoading',false)
                _analytics(['Forgot Password', 'Reset email send'])
                dispatch('updateStep','EmailConfirmed')
            },error =>{
                commit('setGlobalLoading',false)
                commit('setErrorMessages',{email: error.response.data["ForgotPassword"] ? error.response.data["ForgotPassword"][0] : ''}) 
            })
    },
    resendEmailPassword({dispatch,commit}){
        commit('setGlobalLoading',true)
        _analytics(['Registration', 'Resend Email'])
        registrationService.EmailforgotPasswordResend()
            .then(response => {
                commit('setGlobalLoading',false)
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                })
            },error => {
                commit('setGlobalLoading',false)
            })
    },
    changePassword({state,commit},params) {
        let {id} = params
        let {code} = params
        let isValid = state.password === state.confirmPassword
        if(isValid){
            commit('setGlobalLoading',true)
            registrationService.updatePassword(state.password, state.confirmPassword, id, code)
                .then((response) => {
                    _analytics(['Forgot Password', 'Updated password'])
                    global.isAuth = true;
                    commit('setGlobalLoading',false)
                    let url = state.toUrl || defaultSubmitRoute;
                    router.push({path: `${url.path }`});
                }, (error) => {
                    commit('setGlobalLoading',false)
                    commit('setErrorMessages',{
                        password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                        confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : ''}) 
                });
        }else{
            commit('setErrorMessages',{confirmPassword: _dictionary('login_error_not_matched') })
        }
    },
    exit({commit}){
        // let url = state.toUrl || defaultSubmitRoute;
        commit('setResetState')
        router.push({path: `/`});
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}
