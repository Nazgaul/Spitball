let auth2;
const state = {
    auth2: null,
}

const getters = {  
}

const actions = {
    gapiLoad({state},scope){
        return gapi.load('auth2', function() {
            auth2 = gapi.auth2.init({
                client_id: global.client_id,
                'scope': 'profile https://www.googleapis.com/auth/calendar.readonly'
            });
        });
    },
    gapiSignIn({dispatch}){
        return auth2.grantOfflineAccess().then(authResult=>{
            return dispatch('signInCalendar',authResult)
        })
    },
}

export default {
    state,
    getters,
    actions
}
