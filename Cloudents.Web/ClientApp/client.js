import "./publicPath";
import { app, router } from "./main";
router.onReady(() => {
    const matchedComponents = router.getMatchedComponents()
    // no matched routes, reject with 404
    if (!matchedComponents.length) {
       window.location = "/error/notfound"
    }
    app.$mount("#app");
})