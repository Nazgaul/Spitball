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
      
      if(mutation.type === 'route/ROUTE_CHANGED') {
        let { name, hash, params } = store.state.route
        if(name === Profile) {
          if(hash === '#tutorEdit') {
            let userId = store.getters.accountUser?.id
            if(params.id == userId) {
              store.commit('addComponent', TUTOR_EDIT_PROFILE)
            }
          }
        }
      }
    })
  }
}