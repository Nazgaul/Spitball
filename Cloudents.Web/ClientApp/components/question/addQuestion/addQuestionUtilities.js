function ErrorObj(hasError,message){
    this.hasError = hasError;
    this.message = message;
}

function createErrorObj(hasError,message){
    return new ErrorObj(hasError,message);
}

export default{
    createErrorObj
}