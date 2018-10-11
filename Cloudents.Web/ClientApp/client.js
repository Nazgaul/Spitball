import "./publicPath";
import {GetDictionary} from './services/language/languageService'
// get dictionary before we load the website
GetDictionary().then(()=>{
    function getComponent() {
        return import("./main").catch(error => 'An error occurred while loading the component');
      }

      // dynamic import the main component
      getComponent().then(component => {
          component.router.onReady(() => {
            const matchedComponents = component.router.getMatchedComponents()
            // no matched routes, reject with 404
            if (!matchedComponents.length) {
               window.location = "/error/notfound"
            }
            component.app.$mount("#app");
        })
      })
})
