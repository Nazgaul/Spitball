const state = {
    auth2: null,
}

const getters = {  
    getAuth2: state => state.auth2
}
const mutations = {
    setAuth2(state,val){
        state.auth2 = val
    }
}

const actions = {
    gapiLoad({commit, state},scope){
        if(state.auth2) return;
        return gapi.load('auth2', function() {
            let auth2 = gapi.auth2.init({
                client_id: global.client_id,
                'scope': 'profile https://www.googleapis.com/auth/calendar.readonly'
            });
            commit('setAuth2',auth2)
        });
    },
    gapiSignIn({state,dispatch}){
        return state.auth2.grantOfflineAccess().then(authResult=>{
            return dispatch('signInCalendar',authResult).then(res=>{
                return Promise.resolve(res)
            },err=>{
                return Promise.reject(err)
            })
        })
    },
}

export default {
    state,
    getters,
    actions,
    mutations
}
