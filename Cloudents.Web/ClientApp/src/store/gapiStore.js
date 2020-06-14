const state = {
    auth2: null,
    auth2faliure: null,
    scopes: {
        calendar: 'profile https://www.googleapis.com/auth/calendar.readonly'
    }
};

const getters = {  
    getAuth2: state => state.auth2,
    getIsFaliure : state => state.auth2faliure != null,
    getFaliureReason: state =>{
        if(state.auth2faliure?.details) {
            return 'Google:' + state.auth2faliure?.details;
        }
       return null;
    }
}
const mutations = {
    setAuth2(state,val){
        state.auth2 = val;
    }
};

const actions = {
    gapiLoad({commit, state}, scopeName){
        if(state.auth2) return Promise.resolve();
        let returnValue = new Promise((resolve,reject) => {
            gapi.load('auth2', function() { 
                //let scopesToUse = state.scopes[scopeName];      
                let auth2InitParams = {
                    client_id: global.client_id,
                }
                
                if(!! state.scopes[scopeName]){
                    auth2InitParams.scope =  state.scopes[scopeName];
                }
               
                gapi.auth2.init(auth2InitParams)
                    .then((x) => {
                   
                    commit('setAuth2', x);
                    resolve();
                }).catch(x => {
                    state.auth2faliure = x
                    reject(x);
                })
            });
        });
        return returnValue;
    },
    gapiSignIn({state,dispatch}){
        return state.auth2.grantOfflineAccess({
            'scope': state.scopes['calendar']
        }).then(authResult=>{
            return dispatch('signInCalendar',authResult).then(res=>{
                return Promise.resolve(res);
            },err=>{
                return Promise.reject(err);
            });
        });
    },
};

export default {
    state,
    getters,
    actions,
    mutations
}
