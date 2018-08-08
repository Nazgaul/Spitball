module.exports = {
    outputDir: '../wwwroot',

    configureWebpack: {
        output: {
            devtoolModuleFilenameTemplate: '[absolute-resource-path]',
            devtoolFallbackModuleFilenameTemplate: '[absolute-resource-path]?[hash]'
        }
    }
};