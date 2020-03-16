export default () => {
  return store => {
    store.subscribe((mutation) => {
      if(mutation.type === 'updateUser') {
        if(store.getters.getPendingPayment > 0) {
          store.commit('setToaster', 'linkToaster');
        }
      }
    })
  }
}