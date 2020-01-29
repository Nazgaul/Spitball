// GLOBALS:
import { router } from '../main.js';
import codesJson from '../components/pages/authenticationPage/CountryCallingCodes';
const defaultSubmitRoute = { name: 'feed' };

const Fingerprint2 = require('fingerprintjs2');

// SERVICES:
import analyticsService from '../services/analytics.service.js'
import registrationService from '../services/registrationService.js'
import { LanguageService } from "../services/language/languageService.js";

// FUNCTIONS:
function _dictionary(key) {
    return LanguageService.getValueByKey(key);
}
function _analytics(params) {
    analyticsService.sb_unitedEvent(...params);
}

const state = {
    currentStep: 'getStarted',
    stepsHistory: [],
    toUrl: '',

    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    gender: 'male',
    localCode: '',

    grade: '',

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
        0: { name: _dictionary("login_password_indication_weak"), className: "bad" },
        1: { name: _dictionary("login_password_indication_weak"), className: "bad" },
        2: { name: _dictionary("login_password_indication_strong"), className: "good" },
        3: { name: _dictionary("login_password_indication_strong"), className: "good" },
        4: { name: _dictionary("login_password_indication_strongest"), className: "best" }
    }
};

const mutations = {
    setToUrl(state,url){
        state.toUrl = url;
    },
    setResetState(state) {
        state.currentStep = 'getStarted';
        state.stepsHistory = [];

        state.email = '';
        state.phone = '';
        state.localCode = '';
        state.firstName = '';
        state.lastName = '';
        state.gender = 'male',
        state.grade = ''

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
    setEmail(state, email) {
        state.email = email;
    },
    setPhone(state, phoneNumber) {
        state.phone = phoneNumber;
    },
    setGender(state, gender) {
        state.gender = gender;
    },
    setPhoneCode(state, localCode) {
        state.localCode = localCode;
    },
    setBackStep(state) {
        let lastStep = state.stepsHistory.pop();
        state.currentStep = lastStep;
    },
    setStepHistory(state, step) {
        state.stepsHistory.push(step);
    },
    setResetStepHistory(state) {
        state.stepsHistory = [];
    },
    setCurrentStep(state, stepName) {
        state.currentStep = stepName;
    },

    setGlobalLoading(state, value) {
        state.globalLoading = value;
    },
    setErrorMessages(state, errorMessagesObj) {
        state.errorMessage.gmail = (errorMessagesObj.gmail) ? errorMessagesObj.gmail : '';
        state.errorMessage.phone = (errorMessagesObj.phone) ? errorMessagesObj.phone : '';
        state.errorMessage.code = (errorMessagesObj.code) ? errorMessagesObj.code : '';
        state.errorMessage.email = (errorMessagesObj.email) ? errorMessagesObj.email : '';
        state.errorMessage.password = (errorMessagesObj.password) ? errorMessagesObj.password : '';
        state.errorMessage.confirmPassword = (errorMessagesObj.confirmPassword) ? errorMessagesObj.confirmPassword : '';
    },
    setLocalCode(state, localCode) {
        state.localCode = localCode;
    },
    setName(state, fullNameObj) {
        state.firstName = fullNameObj.firstName;
        state.lastName = fullNameObj.lastName;
    },
    setStudentGrade(state, grade) {
        state.grade = grade;
    }
};

const getters = {
    getCurrentRegisterStep: state => state.currentRegTypeStep,
    getCurrentLoginStep: state => state.currentStep,
    getEmail1: state => state.email,
    getPhone: state => state.phone,
    getCountryCodesList: () => codesJson.sort((a, b) => a.name.localeCompare(b.name)),
    getLocalCode: state => state.localCode,
    getGlobalLoading: state => state.globalLoading,
    getErrorMessages: state => state.errorMessage,
    getPassScoreObj: state => state.passScoreObj,
    getStudentGrade: state => state.grade,
};

const actions = {
    updateToUrl({commit}, url) {
        commit('setToUrl',url);
    },
    updateName({commit}, fullNameObj) {
        commit('setName',fullNameObj);
    },
    updateEmail({commit}, email) {
        commit('setEmail',email);
    },
    updatePhone({commit}, phoneNumber) {
        commit('setPhone',phoneNumber);
    },    
    updateGender({commit}, gender) {
        commit('setGender', gender)
    },
    updateLocalCode({commit}, selectedLocalCode) {
        if(selectedLocalCode){
            commit('setLocalCode',selectedLocalCode);
        } else {
            registrationService.getLocalCode().then(({ data }) => {
                commit('setLocalCode', data.code)
            });
        }
    },
    updateRouterStep(context, name) {
        router.push({name: name});
    },
    resetState({commit}){
        commit('setResetState');
    },
    updateRegisterType(context, regType) {
        return registrationService.updateUserRegisterType({ userType: regType })
    },
    updateHistoryStep({commit}, stepName) {
        commit('setStepHistory', stepName);
    },
    // updateStep({commit,state},stepName){
    //     let specialSteps = ["setphone", "verifyphone", "resetpassword"];

    //     if(specialSteps.includes(stepName.toLowerCase())){
    //         commit('setResetStepHistory');
    //         commit('setCurrentStep',stepName);
    //         _analytics(['Registration', 'Email Verified']);
    //         return;
    //     }
    //     if(!stepName){
    //         commit('setResetState');
    //     }else if(state.stepsHistory.includes(stepName)){
    //         commit('setBackStep');
    //     }else {
    //         history.pushState({}, null);
    //         commit('setStepHistory');
    //         commit('setCurrentStep',stepName);
    //     }
    // },
    googleSigning({dispatch, commit, state}) {
        let authInstance = gapi.auth2.getAuthInstance();

        return authInstance.signIn().then((googleUser) => {
            let idToken = googleUser.getAuthResponse().id_token;
            return registrationService.googleRegistration(idToken).then((resp) => {
                let newUser = resp.data.isNew;
                if (newUser) {
                    _analytics(['Registration', 'Start Google']);
                    dispatch('updateHistoryStep', 'setPhone');
                    dispatch('updateRouterStep', 'setPhone')
                } else {
                    _analytics(['Login', 'Start Google']);
                    global.isAuth = true;

                    state.toUrl ? router.push(state.toUrl) : dispatch('updateRouterStep', 'feed');
                }
                return Promise.reject();
            }, (error) => {
                commit('setErrorMessages', { gmail: error.response.data["Google"] ? error.response.data["Google"][0] : '' });
                return Promise.reject(error);
            });
        }, error => {
            return Promise.reject(error);
        });
    },
    emailSigning({dispatch, state, commit}, params) {
        let { recaptcha, password, confirmPassword } = params;
        let { firstName, lastName, email, gender } = state;
        let emailRegObj = { firstName, lastName, email, gender, recaptcha, password, confirmPassword }

        return registrationService.emailRegistration(emailRegObj)
            .then(({data}) => {
                let nextStep = data.step.name;
                
                // if(nextStep.toLowerCase() === "verifyphone" || nextStep.toLowerCase() === "enterphone"){
                //     // dispatch('updateStep','setPhone');
                //     dispatch('updateRouterStep', 'setPhone')
                // }else{
                    // dispatch('updateStep',nextStep);
                    dispatch('updateRouterStep', nextStep)
                //}
                _analytics(['Registration', 'Start']);
                // updateRouterStep(nextStep) 
            },  (error) => {
                commit('setErrorMessages',{
                    email: error.response.data["Email"] ? error.response.data["Email"][0] : '',
                    password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                    confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : ''
                });
                return Promise.reject(error);
            });
    },
    resendEmail({dispatch}) {
        _analytics(['Registration', 'Resend Email']);
        registrationService.emailResend()
            .then(() => {
                dispatch('updateToasterParams', {
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                });
            }, () => {
                dispatch('updateToasterParams', {
                    toasterText: LanguageService.getValueByKey("put some error"),
                    showToaster: true,
                    toasterType: 'error-toaster'
                });
            });
    },
    sendSMScode({dispatch, commit, state}) {
        registrationService.smsRegistration(state.localCode,state.phone)
            .then(function (){
                commit('setErrorMessages', {});
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_verification_code_sent_to_phone"),
                    showToaster: true,
                });
                _analytics(['Registration', 'Phone Submitted']);
                // dispatch('updateStep','VerifyPhone');
                dispatch('updateRouterStep', 'verifyPhone');
            }, function (error){
                commit('setErrorMessages', {phone: error.response.data["PhoneNumber"] ? error.response.data["PhoneNumber"][0] : '' });
            });
    },
    smsCodeVerify({dispatch, commit}, smsCode) {
        let data = { code: smsCode, fingerprint: "" };

        Fingerprint2.getPromise({})
            .then(components => {
                let values = components.map(component => component.value);
                let murmur = Fingerprint2.x64hash128(values.join(''), 31);
                data.fingerprint = murmur;
                registrationService.smsCodeVerification(data)
                    .then(userId => {
                            // dispatch('updateStep','registerType');
                            dispatch('updateRouterStep', 'registerType');
                            _analytics(['Registration', 'Phone Verified']);
                            if(!!userId){
                                _analytics(['Registration', 'User Id', userId.data.id]);
                            }
                    }, () => {
                        commit('setErrorMessages',{code: "Invalid code"});
                    });
            });
    },
    callWithCode({dispatch, commit}) {
        _analytics(['Registration', 'Call Voice SMS']);
        registrationService.voiceConfirmation()
            .then(() => {
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_call_code"),
                    showToaster: true,
                });
            }, error => {
                commit('setErrorMessages',{code: error.text});
            });
    },
    changeNumber({dispatch, commit}) {
        commit('setResetState');
        dispatch('updateRouterStep', 'setPhone');
        // dispatch('updateStep','setPhone');
    },
    finishRegister({dispatch}) {
        global.isAuth = true;
        _analytics(['Registration', 'Congrats']); //TODO: no more congrats. what analytic need to send???
        dispatch('updateRouterStep', 'studentTutor');
    },
    emailValidate({dispatch, commit, state}) {
        registrationService.validateEmail(encodeURIComponent(state.email))
            .then(() => {
                _analytics(['Login Email validation', 'email send']);
                dispatch('updateRouterStep', 'setPassword');
            }, (error)=> {
                commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''});
            });
    },
    logIn({dispatch, commit, state}, password) {
        let data = {
            email: state.email,
            password: password,
        };

        Fingerprint2.getPromise({})
            .then(components => {
                let values = components.map(component => component.value);
                let murmur = Fingerprint2.x64hash128(values.join(''), 31);
                data.fingerprint = murmur;
                registrationService.signIn(data)
                    .then(response => {
                        _analytics(['Login', 'Start']);
                        global.isAuth = true;
                        global.country = response.data.country;
                        
                        if(global.country) {
                            state.toUrl ? router.push(state.toUrl) : dispatch('updateRouterStep', 'feed');
                        } else {
                            //TODO: what error resource should i need here??
                            dispatch('updateToasterParams', {
                                toasterText: LanguageService.getValueByKey("loginRegister_country_error"),
                                showToaster: true,
                                toasterType: 'error-toaster'
                            });
                        }
                    },error =>{
                        commit('setErrorMessages',{email: error.response.data["Password"] ? error.response.data["Password"][0] : ''});
                    });
            });
    },
    resetPassword({dispatch, state, commit}){
        registrationService.forgotPasswordReset(state.email)
            .then(() =>{
                _analytics(['Forgot Password', 'Reset email send']);
                // dispatch('updateStep','EmailConfirmed');
                dispatch('updateRouterStep', 'emailConfirmed');
            },error =>{
                commit('setErrorMessages',{email: error.response.data["ForgotPassword"] ? error.response.data["ForgotPassword"][0] : error.response.data["Email"][0]});
            });
    },
    resendEmailPassword({dispatch}){
        _analytics(['Registration', 'Resend Email']);
        registrationService.EmailforgotPasswordResend()
            .then(() => {
                dispatch('updateToasterParams',{
                    toasterText: _dictionary("login_email_sent"),
                    showToaster: true,
                });
            });
    },
    changePassword({dispatch, commit},params) {
        let {id, code, password, confirmPassword} = params;
        let isValid = (password === confirmPassword);
        if(isValid){
            registrationService.updatePassword(password, confirmPassword, id, code) //TODO: send object instead
                .then(() => {
                    _analytics(['Forgot Password', 'Updated password']);
                    global.isAuth = true;
                    // router.push({path: defaultSubmitRoute.path});
                    dispatch('updateRouterStep', 'feed')
                }, (error) => {
                    commit('setErrorMessages',{
                        password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                        confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : ''
                    });
                });
        } else {
            commit('setErrorMessages', { confirmPassword: _dictionary('login_error_not_matched') });
        }
    },
    exit({dispatch, commit}){
        commit('setResetState');
        dispatch('updateRouterStep', 'feed');
    },
    updateStudentGrade({ commit }, grade) {
        return registrationService.updateGrade({ grade }).then(() => {
            commit('setStudentGrade', grade);
        })
    },
    updateParentStudent(context, fullname) {
        return registrationService.updateParentStudentName(fullname);
    },
    parentRegister({dispatch}, {grade, firstname, lastname}) {
        let promiseGrade = registrationService.updateGrade({grade});
        let promiseStudentParent = dispatch('updateParentStudent', {firstname, lastname});

        Promise.all([promiseGrade, promiseStudentParent])
            .then(() => {
                console.log('grade call success');
            }).then(() => {
                console.log('child name call success');
            }).catch(ex => {
                console.log(ex);
            })
    }
};

export default {
    state,
    mutations,
    getters,
    actions
}