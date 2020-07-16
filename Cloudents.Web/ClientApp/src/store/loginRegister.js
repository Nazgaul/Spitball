// OBSELETE
import { router } from '../main.js';

// SERVICES:
import analyticsService from '../services/analytics.service.js'
import registrationService from '../services/registrationService2.js'
import * as routeNames from '../routes/routeNames.js';

function _analytics(params) {
    analyticsService.sb_unitedEvent(...params);
}

const state = {
    toUrl: '',
    email: '',
    errorMessage: {
        email: "",
    },
};

const mutations = {
    setToUrl(state,url){
        state.toUrl = url;
    },
    setEmail(state, email) {
        state.email = email;
    },
    setErrorMessages(state, errorMessagesObj) {
        state.errorMessage.email = (errorMessagesObj.email) ? errorMessagesObj.email : '';
    }
};

const getters = {    
    getEmail1: state => state.email,
    getErrorMessages: state => state.errorMessage,
};

const actions = {
    updateToUrl({commit}, url) {
        commit('setToUrl',url);
    },
    updateEmail({commit}, email) {
        commit('setEmail',email);
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
    resendEmailPassword(){
        _analytics(['Registration', 'Resend Email']);
        return registrationService.emailforgotPasswordResend()
    },
    changePassword({commit,dispatch},params) {
        let {id, code, password, confirmPassword} = params;
        registrationService.updatePassword(password, id, code, confirmPassword)
            .then(() => {
                dispatch('updateLoginStatus',true)
                _analytics(['Forgot Password', 'Updated password']);
                router.push(state.toUrl);
            }, (error) => {
                commit('setErrorMessages',{
                    password: error.response.data["Password"] ? error.response.data["Password"][0] : error.response.data["ConfirmPassword"][0],
                });
            });
    },
};

export default {
    state,
    mutations,
    getters,
    actions
}