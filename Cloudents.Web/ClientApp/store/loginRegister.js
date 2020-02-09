// GLOBALS:
import { router } from '../main.js';
import codesJson from '../components/pages/authenticationPage/CountryCallingCodes';

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
    studentParentFullName: '',

    globalLoading: false,
    stepValidation: false,

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
    setPhoneCode(state, localCode) {
        state.localCode = localCode;
    },
    setGender(state, gender) {
        state.gender = gender;
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
    setFullName(state, fullname) {
        state.studentParentFullName = fullname
    },
    // setFirstName(state, firstName) {
    //     state.firstName = firstName;
    // },
    // setLastName(state, lastName) {
    //     state.lastName = lastName;
    // },
    setStudentGrade(state, grade) {
        state.grade = grade;
    },
    setStepValidation(state, val) {
        state.stepValidation = val;
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
    getStepValidation: state => state.stepValidation,
    getStudentParentFullName: state => state.studentParentFullName
    // getFirstName: state => state.firstName,
    // getLastName: state => state.lastName,
};

const actions = {
    updateToUrl({commit}, url) {
        commit('setToUrl',url);
    },
    // updateFirstName({commit}, firstName) {
    //     commit('setFirstName', firstName)
    // },
    // updateLastName({commit}, lastName) {
    //     commit('setLastName', lastName)
    // },
    updateFullName({commit}, fullname) {
        commit('setFullName', fullname)
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
    updateGrade({commit}, grade) {
        commit('setStudentGrade', grade);
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
    resetState({commit, dispatch}){
        commit('setResetState');
        dispatch('updateRouterStep', 'register')
    },
    updateRegisterType(context, regType) {
        return registrationService.updateUserRegisterType({ userType: regType })
    },
    updateHistoryStep({commit}, stepName) {
        commit('setStepHistory', stepName);
    },
    updateStepValidation({commit}, val) {       
        commit('setStepValidation', val);
    },
    googleSigning({dispatch, commit, state}) {
        if (window.Android) {
            Android.onLogin();
            return;
        }
        let authInstance = gapi.auth2.getAuthInstance();

        return authInstance.signIn().then((googleUser) => {
            let idToken = googleUser.getAuthResponse().id_token;
            return registrationService.googleRegistration(idToken).then(({data}) => {
                if (!data.isSignedIn) {
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
    emailSigning({state, commit}, params) {
        let { recaptcha, password, confirmPassword } = params;
        let { firstName, lastName, email, gender } = state;
        let emailRegObj = { firstName, lastName, email, gender, recaptcha, password, confirmPassword }

        return registrationService.emailRegistration(emailRegObj)
            .then(({data}) => {
                _analytics(['Registration', 'Start']);
                if (data.param && data.param.phoneNumber) {
                    commit('setPhone',data.param.phoneNumber);
                    commit('setPhoneCode')
                }
                // setPhone(state, phoneNumber) {
                //     state.phone = phoneNumber;
                // },
                // setPhoneCode(state, localCode) {
                //     state.localCode = localCode;
                // },
                router.push(data.step);
               // dispatch('updateRouterStep', 'registerEmailConfirmed')
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
                    global.isAuth = true;

                    _analytics(['Forgot Password', 'Updated password']);
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
    updateStudentGrade({ commit, dispatch }) {
        let grade = state.grade
        if(grade) {
            return registrationService.updateGrade({ grade }).then(() => {
                global.isAuth = true;
                commit('setStudentGrade', grade);
                dispatch('updateRouterStep', 'feed');
            })
        }
    },
    updateParentStudent({dispatch, state}) {
        let parentObj = {
            grade: state.grade,
            name: state.studentParentFullName
        }
        return registrationService.updateParentStudentName(parentObj).then(() => {
            global.isAuth = true;
            dispatch('updateRouterStep', 'feed');
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