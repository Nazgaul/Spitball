import * as routeNames from '../routes/routeNames.js';
import codeEditor_store from "../store/studyRoomStore/codeEditor_store.js";
export default () => {
    let registeredStore = [];

    return store => {

        let registeredModules =[];
        const registerModule = function(moduleName, moduleObj){
            store.registerModule(moduleName, moduleObj);
            registeredModules.push(moduleName);
        };
        const unregisterModules = function(){
            registeredStore.forEach(f=> {
                store.unregisterModule(f);
            })
            //store.unregisterModule(moduleName);


        };
        // const lazyRegisterModule = function(store, moduleName, moduleObj){
        //     //wil register the module and keep it alive
        //     if(!store._modules.root._children[moduleName]){
        //         store.registerModule(moduleName, moduleObj);
        //     }
        // };
        
        // export default {
        //     registerModule,
        //     unregisterModule,
        //     lazyRegisterModule
        // }

        store.subscribe((mutation) => {
            if (mutation.type === 'setRouteStack') {
                unregisterModules();
                if (mutation.payload.name === routeNames.StudyRoom) {
                    //registerModule(moduleName, moduleObj);
                   registerModule('codeEditor_store',codeEditor_store);
                }
            
        }
        });
        
    }
}