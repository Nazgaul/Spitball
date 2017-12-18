﻿const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const CleanWebpackPlugin = require("clean-webpack-plugin");
var Visualizer = require("webpack-visualizer-plugin");
var StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;
var t = require("./webpack.global.js");


module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return [
        {
            stats: { modules: false },
            //resolve: { extensions: ['.js'] },
            entry: {
                vendor: [
                    "vue",
                    "vue-router",
                    "vuex",
                    "vue-analytics",
                    "vue-lazyload",
                    "axios",
                    "querystring",
                    "./ClientApp/main.styl",
                    "./wwwroot/content/main.less",
                    "./ClientApp/myFont.font.js",
                    "vuex-persistedstate",
                    "vue-star-rating",
                    "vuetify/es5/components/Vuetify",
                    "vuetify/es5/components/VApp",
                    "vuetify/es5/components/VGrid",
                    "vuetify/es5/components/VChip",
                    "vuetify/es5/components/VToolbar",
                    "vuetify/es5/components/VExpansionPanel",
                    "vuetify/es5/components/VList",
                    "vuetify/es5/components/VTextField",
                    "vuetify/es5/components/VCard",
                    "vuetify/es5/components/VCarousel",
                    "vuetify/es5/components/VProgressCircular",
                    "vuetify/es5/components/VProgressLinear",
                    "vuetify/es5/components/VSubheader",
                    "vuetify/es5/components/VDivider",
                    "vuetify/es5/components/VDialog",
                    "vuetify/es5/components/VBtn",
                    "vuetify/es5/components/VTooltip",
                    "vuetify/es5/components/VMenu",
                    "vuetify/es5/components/VSwitch",
                    "vuetify/es5/components/VTabs",
                    "vuetify/es5/directives/scroll",
                    "vuetify/es5/components/VIcon"
                ]
            },
            module: {
                rules: [
                    { test: /\.css(\?|$)/, use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader" : "css-loader?minimize" }) },
                    { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: "url-loader?limit=8192" },
                    {
                        test: /\.styl$/,
                        loader: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader!stylus-loader" : "css-loader?minimize!stylus-loader" })
                    },
                    {
                        test: /\.less$/,
                        exclude: /ClientApp/,
                        use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader!less-loader" : "css-loader?minimize!less-loader" })
                    },
                    {
                        test: /\.font\.js/,
                        loader: ExtractTextPlugin.extract({
                            use: [
                                isDevBuild ? "css-loader" : "css-loader?minimize",
                                "webfonts-loader"
                            ]
                        })
                    }
                ]
            },
            output: {
                path: path.join(__dirname, "wwwroot", "dist"),
                publicPath: t.getdist(isDevBuild), // isDevBuild ? "/dist/" : "//spitball.azureedge.net/dist/",
                filename: "[name].[chunkhash].js",
                library: "[name]"
            },
            plugins: [
                new ExtractTextPlugin({
                    filename: '[name].[contenthash].css'
                }),
                //new PurifyCSSPlugin({
                //    // Give paths to parse for rules. These should be absolute!
                //    paths: glob.sync(path.join(__dirname, 'clientapp/**/*.vue')),
                //    minimize: !isDevBuild,
                //    purifyOptions: {
                //        whitelist: ["*spitball*"]
                //    }
                //}),
                new webpack.DefinePlugin({
                    'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
                }),
                new webpack.DllPlugin({
                    path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
                    name: "[name]"
                }),
                new StatsWriterPlugin({
                    filename: "vendor.json",
                    transform: function (data, opts) {
                        return JSON.stringify(data.assetsByChunkName);
                    }
                })
            ].concat(isDevBuild ? [
                new CleanWebpackPlugin(path.join(__dirname, "wwwroot", "dist")),
                new Visualizer({
                    filename: "./statistics-vendor.html"
                })
            ] : [
                    new webpack.optimize.UglifyJsPlugin()
                ])
        }];
};
