module.exports = {
    getdist: function (isDevBuild) {
        return isDevBuild ? "/dist/" : "//spitball.azureedge.net/dist/";
    }
};