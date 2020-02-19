const registerModule = function(store, moduleName, moduleObj){
    store.registerModule(moduleName, moduleObj);
};
const unregisterModule = function(store, moduleName){
    return store.unregisterModule(moduleName);
};
const lazyRegisterModule = function(store, moduleName, moduleObj){
    //wil register the module and keep it alive
    if(!store._modules.root._children[moduleName]){
        store.registerModule(moduleName, moduleObj);
    }
};

export default {
    registerModule,
    unregisterModule,
    lazyRegisterModule
}