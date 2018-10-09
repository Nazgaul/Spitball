﻿const path = require("path");
const webpack = require("webpack");
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const CleanWebpackPlugin = require("clean-webpack-plugin");
var Visualizer = require("webpack-visualizer-plugin");
var StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;
var t = require("./webpack.global.js");
const merge = require('webpack-merge');
const WebpackRTLPlugin = require("webpack-rtl-plugin");
const bundleOutputDir = "./wwwroot/dist";

const allModules = [
    "vue",
    "vue-router",
    "vuex",
    "vue-analytics",
    "axios",
    "query-string",
    "./ClientApp/main.styl",
    "./wwwroot/content/main.less",
    "./ClientApp/myFont.font.js",
    "vuex-persistedstate",
    "vuetify/es5/components/Vuetify",
    "vuetify/es5/components/VApp",
    "vuetify/es5/components/VGrid",
    "vuetify/es5/components/VChip",
    "vuetify/es5/components/VToolbar",
    "vuetify/es5/components/VExpansionPanel",
    "vuetify/es5/components/VList",
    "vuetify/es5/components/VTextField",
    "vuetify/es5/components/VSelect",
    "vuetify/es5/components/VCard",
    "vuetify/es5/components/VCarousel",
    "vuetify/es5/components/VProgressCircular",
    "vuetify/es5/components/VProgressLinear",
    "vuetify/es5/components/VSubheader",
    "vuetify/es5/components/VDivider",
    "vuetify/es5/components/VDialog",
    "vuetify/es5/components/VBtn",
    "vuetify/es5/components/VBtnToggle",
    "vuetify/es5/components/VTooltip",
    "vuetify/es5/components/VMenu",
    "vuetify/es5/components/VSwitch",
    "vuetify/es5/components/VTabs",
    "vuetify/es5/directives/scroll",
    "vuetify/es5/components/VIcon",
    "vuetify/es5/components/VSnackbar",
    "vuetify/es5/components/VNavigationDrawer",
    "vuetify/es5/components/VAvatar",
    "vuetify/es5/components/VPagination",
    "vuetify/es5/components/VDataTable",
    "webfontloader",
   

];

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);

    const sharedConfig = () => ({
        stats: { modules: false },

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
                    test: /\.scss$/,
                    exclude: /ClientApp/,
                    use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader!sass-loader" : "css-loader?minimize!sass-loader" })
                },
                {
                    test: /\.font\.js/,
                    loader: ExtractTextPlugin.extract({
                        use: [
                            isDevBuild ? "css-loader" : "css-loader?minimize",
                            "webfonts-loader"
                        ]
                    })
                },
            ]
        },
        plugins: [
            new ExtractTextPlugin({
                filename: '[name].[contenthash].css'
            }),

            new WebpackRTLPlugin({
                filename: '[name].[contenthash].rtl.css'
                //options: {
                //    'clean': true,
                //    'stringMap': [
                //        {
                //            'name': 'left-right',
                //            'priority': 100,
                //            'search': ['left', 'Left', 'LEFT'],
                //            'replace': ['right', 'Right', 'RIGHT'],
                //            'options': {
                //                'scope': '*',
                //                'ignoreCase': false
                //            }
                //        },
                //        {
                //            'name': 'ltr-rtl',
                //            'priority': 100,
                //            'search': ['ltr', 'Ltr', 'LTR'],
                //            'replace': ['rtl', 'Rtl', 'RTL'],
                //            'options': {
                //                'scope': '*',
                //                'ignoreCase': false
                //            }
                //        }
                //    ]
                //},
               // plugins: [],
               // diffOnly: false,
                //minify: false,
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


        ].concat(isDevBuild ? [

        ] : [

            ])
    });
    const clientBundleConfig = merge(sharedConfig(),
        {
            output: {
                path: path.join(__dirname, "wwwroot", "dist"),
                publicPath: "/dist/", 
                filename: "[name].[chunkhash].js",
                library: "[name]"
            },
           
            entry: {
                vendor: allModules
            },
            plugins: [
                new StatsWriterPlugin({
                    filename: "vendor.json",
                    transform: function (data, opts) {
                        return JSON.stringify(data.assetsByChunkName);
                    }
                }),
                new webpack.DllPlugin({
                    path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
                    name: "[name]"
                })
            ].concat(isDevBuild ? [
                new CleanWebpackPlugin(path.join(__dirname, "wwwroot", "dist")),
                new Visualizer({
                    filename: "./statistics-vendor.html"
                }),
                new webpack.SourceMapDevToolPlugin({
                    filename: "[file].map", // Remove this line if you prefer inline source maps
                    moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                })
            ] : [
                    new webpack.optimize.UglifyJsPlugin()
                ])

        });

    const serverBundleConfig = merge(sharedConfig(), {
        target: 'node',
        resolve: { mainFields: ['main'] },
        entry: {
            vendor: allModules.concat(['aspnet-prerendering'])
        },
        output: {
            path: path.join(__dirname, 'ClientApp', 'dist'),
            filename: "[name].js",
            libraryTarget: 'commonjs2'
        },
        //module: {
        //    rules: [ { test: /\.css(\?|$)/, use: ['to-string-loader', isDevBuild ? 'css-loader' : 'css-loader?minimize' ] } ]
        //},
        plugins: [
            new webpack.DllPlugin({
                publicPath: 'dist/',
                filename: '[name].js',
                library: '[name]_[hash]',
                path: path.join(__dirname, 'ClientApp', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            }),

        ].concat(isDevBuild ? [
            new CleanWebpackPlugin(path.join(__dirname, "ClientApp", "dist"))
        ] : [])
    });
    return [clientBundleConfig];

};
