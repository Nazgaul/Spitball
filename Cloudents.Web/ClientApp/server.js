import { createApp } from './mainServer';
export default context => {
    return new Promise((resolve, reject) => {
        const { app, router } = createApp()
        const { url } = context
        const { fullPath } = router.resolve(url).route

        if (fullPath !== url) {
            return reject({ url: fullPath })
        }

        // set router's location
        router.push(url);

        router.onReady(() => {
            const matchedComponents = router.getMatchedComponents()
            // no matched routes, reject with 404
            if (!matchedComponents.length) {
                return reject({ code: 404, router });
            }

            // Call fetchData hooks on components matched by the route.
            // A preFetch hook dispatches a store action and returns a Promise,
            // which is resolved when the action is complete and store state has been
            // updated.
            Promise.all(matchedComponents.map(({ asyncData }) => asyncData && asyncData({
                route: router.currentRoute
            }))).then(() => {
                // isDev && console.log(`data pre-fetch: ${Date.now() - s}ms`)
                // After all preFetch hooks are resolved, our store is now
                // filled with the state needed to render the app.
                // Expose the state on the render context, and let the request handler
                // inline the state in the HTML response. This allows the client-side
                // store to pick-up the server-side state without having to duplicate
                // the initial data fetching on the client.
                // context.state = store.state
                resolve(app)
            }).catch(reject)
        }, reject)
        //resolve(app);
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