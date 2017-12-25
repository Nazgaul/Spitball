import { createApp } from './main';

export default context => {
  return new Promise((resolve, reject) => {
    const { app, router } = createApp();
    router.push(context.url);

    router.onReady(() => {
      const matchedComponents = router.getMatchedComponents()
      // no matched routes, reject with 404
      if (!matchedComponents.length) {
        return reject({ code: 404, router });
      }

      // the Promise should resolve to the app instance so it can be rendered
      resolve(app);
    },reject);
    //const matchedComponents = router.getMatchedComponents()
    //if (!matchedComponents.length) {
    //  return reject({ code: 404 });
    //}
    //Promise.all(matchedComponents.map(Component => {
    //  // if (Component.asyncData) {
    //  //   return Component.asyncData({
    //  //     store
    //  //   });
    //  // }
    //})).then(() => {
    //  //context.state = store.state;
    //  resolve(app);
    //}).catch(reject)
    //}, reject)
  });
};