import "./publicPath";
import { GetDictionary, GetVersion } from './services/language/languageService'
import analyticsService from './services/analytics.service'
// get dictionary before we load the website
global.lang = document.getElementsByTagName("html")[0].getAttribute("lang");
global.mainCdn = true;

//TODO change this fix to something else
/*makes sure user have the latest client version temporary solution*/
function VersionCheck() {
    let inStudyRoom = global.location.pathname.indexOf('studyroom') > -1;
    if(!inStudyRoom){
        GetVersion().then(version => {
            if(version != global.version) {
                analyticsService.sb_unitedEvent('VERSION_UPGRADE', `Previous_Version: ${global.version} Current_Version: ${version}`);
                location.reload(true);
            }
        });
    }
}
let fiveMinutes = 10000 * 5;
window.setInterval(VersionCheck, fiveMinutes);
VersionCheck();

GetDictionary().then(() => {
    function getComponent() {
        return import("./main").catch(error => error);
    }

    // dynamic import the main component
    getComponent().then(component => {
        try {
            component.router.onReady(() => {
                const matchedComponents = component.router.getMatchedComponents();
                // no matched routes, reject with 404
                if (!matchedComponents.length) {
                    window.location = "/error/notfound";
                }
                component.app.$mount("#app");
            });
        }
        catch (err) {
            let appEl = document.getElementById("app");
            for (let prop in err) {
                let el = document.createElement('div');
                el.innerHTML = err[prop];
                appEl.appendChild(el);
            }
        }
    });
});
