function errorObj(hasError,message){
    this.hasError = hasError;
    this.message = message;
};

function createErrorObj(hasError,message){
    return new errorObj(hasError,message);
}

export default{
    createErrorObj
}