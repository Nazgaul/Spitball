import analyticsService from '../services/analytics.service.js'
import registrationService from '../services/registrationService.js'
import {router} from '../main.js';
const isIl = global.country.toLowerCase() === 'il';
const defaultSubmitRoute = isIl ? {path: '/note'} : {path: '/ask'};

const state = {
    currentStep: 'getStarted',
    stepsHistory: ['getStarted'],

    email: '',
    password: '',
    confirmPassword: '',
    phone: {
        number: '',
        code: ''
    },
    smsCode: '',
    
    passScoreObj: {},
    recaptcha: "03AOLTBLTZ4nPLcBWRHYX8lC-jXl84yH4IUYOBhSmX4-heEKk_7qzscRu6VAw4bZPlrqZGwdV_9yJW1QAI6xGDC2kwRnwiciUiRUa8idgyYS2r6sWrQbYJ19ehE8Mt1e6RN_4SUX8cdOhhCgNhLLYN4qt7FFO0mEw3IqOrO-A5hKkAr2a98NkZtktk-aHUexadeDt6eossddugDTDJE7l4HfBJr0uHHuK6ekOjxWUIMDYdU71yKtQ01x4dI-foMfzxpMQ-kC9X9oTS9OZovVcPABBU9kiFk867Bu__oIx0ngNs6QTVo4s8v6hl_vLuua6a6UOILegHMV-J",
    siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
    googleLoading: false,
    emailLoading: false,
    errorMessage: {
        phone: "",
        email: "",
        password: "",
        confirmPassword: ""
    },
}

const mutations = {
    setEmail(state,email){
        state.email = email
    },
    setPassword(state,password){
        state.password = password
    },
    setConfirmPassword(state,confirmPassword){
        state.confirmPassword = confirmPassword
    },
    setPhone(state,phoneObj){
        state.phone.number = phoneObj.number;
        state.phone.code = phoneObj.code;
    },
    setSmsCode(state,smsCode){
        state.smsCode = smsCode
    },
    setStep(state,stepName){
        state.stepsHistory.push(stepName);
        state.currentStep = stepName
    },
    setGoogleLoading(state,value){
        state.googleLoading = value
    },
    setEmailLoading(state,value){
        state.emailLoading = value
    },
    setRecaptcha(state,recaptcha){
        state.recaptcha = recaptcha
    },
    setErrorMessages(state,errorMessagesObj){
        state.errorMessage.email = errorMessagesObj.email
        state.errorMessage.password = errorMessagesObj.password
        state.errorMessage.confirmPassword = errorMessagesObj.confirmPassword
    },
}

const getters = {
    getEmail: state => state.email,
    getPhone: state => state.phone,
    getCurrentLoginStep: state => state.currentStep,
    getGoogleLoading: state => state.googleLoading,
    getEmailLoading: state => state.emailLoading,
    getSiteKey: state => state.siteKey,
}

const actions = {
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
    updatePhone({commit},phoneObj){
        commit('setPhone',phoneObj)
    },
    updateSmsCode({commit},smsCode){
        commit('setSmsCode',smsCode)
    },
    updateStep({commit},stepName){
        commit('setStep',stepName)
    },
    updateAnalytics({},analytics){
        analyticsService.sb_unitedEvent(...analytics);
    },
    updateRoute({},route){
        router.push({path: `${route}`});
    },
    updateGoogleLoading({commit},value){
        commit('setGoogleLoading',value)
    },
    updateEmailLoading({commit},value){
        commit('setEmailLoading',value)
    },
    googleSigning({dispatch}){
        dispatch('updateGoogleLoading',true)
        let authInstance = gapi.auth2.getAuthInstance();
        authInstance.signIn().then((googleUser) => {
            let idToken = googleUser.getAuthResponse().id_token;
            registrationService.googleRegistration(idToken)
                .then((resp) => {
                    dispatch('updateGoogleLoading',false)
                    let newUser = resp.data.isNew;
                    if (newUser) {
                        dispatch('updateAnalytics',['Registration', 'Start Google'])
                        dispatch('updateStep','setPhone')
                    } else {
                        dispatch('updateAnalytics',['Login', 'Start Google'])
                        global.isAuth = true;
                        dispatch('updateRoute',defaultSubmitRoute.path)
                    }
                }, (error) => {
                    dispatch('updateGoogleLoading',false)
                    // self.errorMessage.gmail = error.response.data["Google"] ? error.response.data["Google"][0] : '';
                    // check it
                });
        }, (error) => {
            dispatch('updateGoogleLoading',false)
        });
    },
    emailSigning({dispatch,state,commit}){
        dispatch('updateEmailLoading',true)
        registrationService.emailRegistration(state.email, state.recaptcha, state.password, state.confirmPassword)
            .then((resp) => {
                let nextStep = resp.data.step;
                dispatch('updateStep',nextStep)
                dispatch('updateAnalytics',['Registration', 'Start'])
                dispatch('updateEmailLoading',false)
            },  (error) => {
                dispatch('updateRecaptcha','')
                self.$refs.recaptcha.reset();
                dispatch('updateEmailLoading',false)
                let errorMessagesObj = {
                    email: error.response.data["Email"] ? error.response.data["Email"][0] : '',
                    password: error.response.data["Password"] ? error.response.data["Password"][0] : '',
                    confirmPassword: error.response.data["ConfirmPassword"] ? error.response.data["ConfirmPassword"][0] : '',
                }
                commit('setErrorMessages',errorMessagesObj)
            });
    }
}

export default {
    state,
    mutations,
    getters,
    actions
}