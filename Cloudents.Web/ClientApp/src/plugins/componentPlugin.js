import { auth_SETTER } from '../store/constants/authConstants';
import { TUTOR_EDIT_PROFILE } from '../components/pages/global/toasterInjection/componentConsts'
import { Profile } from '../routes/routeNames'

export default () => {
  return store => {
    store.subscribe((mutation) => {
      if(mutation.payload?.query?.authDialog === auth_SETTER.register) {
        let isLogged = store.getters.getUserLoggedInStatus     
        if (!isLogged){
          store.commit('setComponent', auth_SETTER.register)
        }
      }
      if(mutation.type === 'setRouteStack') {
        let { name, hash } = mutation.payload
        if(name === Profile) {
          if(hash === '#tutorEdit') {
            let isMyProfile = store.getters.getIsMyProfile
            if(isMyProfile) {
              store.commit('addComponent', TUTOR_EDIT_PROFILE)
            }
          }
        }
      }
    })
  }
}