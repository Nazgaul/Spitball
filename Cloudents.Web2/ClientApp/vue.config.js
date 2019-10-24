const webpackRtlPlugin = require('webpack-rtl-plugin');

module.exports = {
    configureWebpack: {
        plugins: [
            new webpackRtlPlugin({
                minify: false,
            })
        ]
    }
}