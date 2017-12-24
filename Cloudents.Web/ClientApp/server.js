import { app, router, store } from './main';

export default context => {
  return new Promise((resolve, reject) => {
    router.push(context.url);

    router.onReady(() => {
      const matchedComponents = router.getMatchedComponents()
      if (!matchedComponents.length) {
        return reject({ code: 404 });
      }
      Promise.all(matchedComponents.map(Component => {
        // if (Component.asyncData) {
        //   return Component.asyncData({
        //     store
        //   });
        // }
      })).then(() => {
        //context.state = store.state;
        resolve(app);
      }).catch(reject)
    }, reject)
  })
};