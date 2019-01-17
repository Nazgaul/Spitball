function errorObj(hasError,message,targetElement,ref){
    this.hasError = hasError;
    this.message = message;
    this.targetElement = targetElement;
    this.ref = ref;
};

function createErrorObj(hasError,message,targetElement,ref){
    return new errorObj(hasError,message,targetElement,ref)
}

export default{
    createErrorObj
}