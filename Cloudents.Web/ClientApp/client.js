﻿import "./publicPath";
import { GetDictionary, GetVersion } from './services/language/languageService'
import analyticsService from './services/analytics.service'
// get dictionary before we load the website
global.lang = document.getElementsByTagName("html")[0].getAttribute("lang");
global.mainCdn = true;
global.client_id = '341737442078-ajaf5f42pajkosgu9p3i1bcvgibvicbq.apps.googleusercontent.com';

//TODO change this fix to something else
/*makes sure user have the latest client version temporary solution*/
function versionCheck() {
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
let minute = 60000;
window.setInterval(versionCheck, minute * 30);
versionCheck();

function errorHandling(err) {
    let body = document.body;
    var errJson = JSON.stringify(err);
    console.error(err);
    //for (let prop in err) {
        let el = document.createElement('div');
        el.innerHTML = errJson
        body.appendChild(el);  
}
GetDictionary().then(() => {
    function getComponent() {
        return import("./main").catch(err => {
            errorHandling(err);
        });
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
            errorHandling(err);
        }
    });
});
