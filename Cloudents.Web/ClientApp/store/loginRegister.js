// GLOBALS:
import { router } from '../main.js';
import codesJson from '../components/pages/authenticationPage/CountryCallingCodes';

const Fingerprint2 = require('fingerprintjs2');

// SERVICES:
import analyticsService from '../services/analytics.service.js'
import registrationService from '../services/registrationService.js'
import { LanguageService } from "../services/language/languageService.js";
import * as routeNames from '../routes/routeNames.js';

// FUNCTIONS:
function _dictionary(key) {
    return LanguageService.getValueByKey(key);
}
function _analytics(params) {
    analyticsService.sb_unitedEvent(...params);
}

const state = {
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
    setStudentGrade(state, grade) {
        state.grade = grade;
    },
};

const getters = {    
    getEmail1: state => state.email,
    getPhone: state => state.phone,
    getCountryCodesList: () => codesJson.sort((a, b) => a.name.localeCompare(b.name)),
    getLocalCode: state => state.localCode,
    getGlobalLoading: state => state.globalLoading,
    getErrorMessages: state => state.errorMessage,
    getPassScoreObj: state => state.passScoreObj,
    getStudentGrade: state => state.grade,
    getStudentParentFullName: state => state.studentParentFullName
};

const actions = {
    updateToUrl({commit}, url) {
        commit('setToUrl',url);
    },
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
    resetState({commit}){
        commit('setResetState');
        router.push({name: routeNames.Register});
    },
    updateRegisterType(context, regType) {
        return registrationService.updateUserRegisterType({ userType: regType })
    },
    googleSigning({commit, state,dispatch}) {
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
                    router.push({name: routeNames.RegisterSetPhone});
                } else {
                    _analytics(['Login', 'Start Google']);
                    dispatch('updateLoginStatus',true)
                    router.push(state.toUrl)     
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
                router.push(data.step);
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
                router.push({name: routeNames.RegisterVerifyPhone});
            }, function (error){
                commit('setErrorMessages', {phone: error.response.data["PhoneNumber"] ? error.response.data["PhoneNumber"][0] : '' });
            });
    },
    smsCodeVerify({commit}, smsCode) {
        let data = { code: smsCode, fingerprint: "" };

        Fingerprint2.getPromise({})
            .then(components => {
                let values = components.map(component => component.value);
                let murmur = Fingerprint2.x64hash128(values.join(''), 31);
                data.fingerprint = murmur;
                registrationService.smsCodeVerification(data)
                    .then(userId => {
                            router.push({name: routeNames.RegisterType});
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
    changeNumber({commit}) {
        commit('setResetState');
        router.push({name: routeNames.RegisterSetPhone});
    },
    emailValidate({commit, state}) {
        registrationService.validateEmail(encodeURIComponent(state.email))
            .then(() => {
                _analytics(['Login Email validation', 'email send']);
                router.push({name: routeNames.LoginSetPassword});
            }, (error)=> {
                commit('setErrorMessages',{email: error.response.data["Email"] ? error.response.data["Email"][0] : ''});
            });
    },
    logIn({commit, state,dispatch}, password) {
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
                        global.country = response.data.country;
                        dispatch('updateLoginStatus',true)
                        router.push(state.toUrl);
                    },error =>{
                        commit('setErrorMessages',{email: error.response.data["Password"] ? error.response.data["Password"][0] : ''});
                    });
            });
    },
    resetPassword({state, commit}){
        registrationService.forgotPasswordReset(state.email)
            .then(() =>{
                _analytics(['Forgot Password', 'Reset email send']);
                router.push({name: routeNames.LoginEmailConfirmed});
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
    changePassword({commit,dispatch},params) {
        let {id, code, password, confirmPassword} = params;
        let isValid = (password === confirmPassword);
        if(isValid){
            registrationService.updatePassword(password, confirmPassword, id, code) //TODO: send object instead
                .then(() => {
                    dispatch('updateLoginStatus',true)
                    _analytics(['Forgot Password', 'Updated password']);
                    router.push(state.toUrl);
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
    exit({commit}){
        commit('setResetState');
        router.push(state.toUrl);
    },
    updateStudentGrade({ commit ,dispatch}) {
        let grade = state.grade
        if(grade) {
            return registrationService.updateGrade({ grade }).then(() => {
                dispatch('updateLoginStatus',true)
                commit('setStudentGrade', grade);
                router.push('/');
            })
        }
    },
    updateParentStudent({state,dispatch}) {
        let parentObj = {
            grade: state.grade,
            name: state.studentParentFullName
        }
        return registrationService.updateParentStudentName(parentObj).then(() => {
            dispatch('updateLoginStatus',true)
            return
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