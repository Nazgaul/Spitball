const webpackRtlPlugin = require('webpack-rtl-plugin');
const MiniCssExtractPlugin = require("mini-css-extract-plugin-with-rtl");

module.exports = {
    configureWebpack: {
        plugins: [
          new MiniCssExtractPlugin({
            filename: "site.[contenthash].css",
            rtlEnabled: true,
            ignoreOrder: true,
            rtlGlobalVar: 'pageDir'
            // allChunks: true

        }),
            new webpackRtlPlugin({
                minify: false,
            })
        ],
    }
}