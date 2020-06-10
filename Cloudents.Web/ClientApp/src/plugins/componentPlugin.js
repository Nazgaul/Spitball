import { auth_SETTER } from '../store/constants/authConstants';

export default () => {
  return store => {
    store.subscribe((mutation) => {
      if(mutation.payload?.query?.authDialog === auth_SETTER.register) {
          let isLogged = store.getters.getUserLoggedInStatus     
          if (!isLogged){
            store.commit('setComponent', auth_SETTER.register)
          }
      }
    })
  }
}