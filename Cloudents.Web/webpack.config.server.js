﻿const ExtractTextPlugin = require("extract-text-webpack-plugin");
const webpack = require('webpack');
const path = require("path"),
glob = require('glob');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    return {
        // stats: {
        //     modules: false
        // },
        context: __dirname,
        module: {
            rules: [
                // {
                //     test: /\.svg$/,
                //     loader: "vue-svg-loader",
                //     options: {
                //         // optional [svgo](https://github.com/svg/svgo) options
                //         svgo: {
                //             plugins: [
                //                 { removeDoctype: true },
                //                 { removeComments: true },
                //                 { removeTitle: true },
                //                 { cleanupIDs: true }
                //             ]
                //         }
                //     }

                // },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: [
                        {
                            loader: "url-loader",
                            options: {
                                limit: 8192,
                                // useRelativePath: !isDevBuild,
                                //publicPath: !isDevBuild ? 'cdnUrl' : '/dist/'

                            }
                        },
                        {
                            loader: "image-webpack-loader",
                            options: {
                                bypassOnDebug: true,
                                optipng: {
                                    enabled: true
                                }
                            }
                        }
                    ]

                },
                {
                    test: /\.js$/,
                    loader: "babel-loader",
                    // babelrc: false,
                    options: {
                        "presets": [["env", {
                            "targets": {
                                "node": "8.9.3"
                            }
                        }]],
                        //"plugins": [
                        //    "syntax-dynamic-import",
                        //    "transform-object-rest-spread",
                        //    ["transform-imports", {
                        //        "vuetify": {
                        //            "transform": "vuetify/es5/components/${member}",
                        //            "preventFullImport": true
                        //        }
                        //    }]]
                    },
                },
                {
                    test: /\.vue$/,
                    loader: "vue-loader",
                    options: {
                        optimizeSSR: false,
                        preserveWhitespace: isDevBuild ? false : true,
                        loaders: {
                            css: "vue-style-loader!css-loader",
                            //isDevBuild
                            // ? "vue-style-loader!css-loader"
                            // : ExtractTextPlugin.extract({
                            //     use: "css-loader?minimize",
                            //     fallback: "vue-style-loader"
                            // }),
                            less: "vue-style-loader!css-loader!less-loader"
                            //isDevBuild
                            //? "vue-style-loader!css-loader!less-loader"
                            //: ExtractTextPlugin.extract({
                            //    use: "css-loader?minimize!less-loader",
                            //    fallback: "vue-style-loader"
                            //})
                        }
                    }
                },
                {
                    test: /\.css$/,
                    use: ["style-loader", "css-loader"]
                    //isDevBuild
                    // ? ["style-loader", "css-loader"]
                    //: ExtractTextPlugin.extract({ use: "css-loader?minimize" })
                },
                // {
                //     test: /\.css(\?|$)/,
                //     use: ExtractTextPlugin.extract({ use: isDevBuild ? "css-loader" : "css-loader?minimize" })
                // },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: "url-loader?limit=8192" },
                {
                    test: /\.styl$/,
                    loader: "css-loader!stylus-loader"
                    //ExtractTextPlugin.extract({
                    //use: isDevBuild ? "css-loader!stylus-loader" : "css-loader?minimize!stylus-loader"
                    //})
                },
                {
                    test: /\.less$/,
                    exclude: /ClientApp/,
                    use: "css-loader?minimize!less-loader"
                    //ExtractTextPlugin.extract({
                    //use: isDevBuild ? "css-loader!less-loader" : "css-loader?minimize!less-loader"
                    //})
                },
                {
                    test: /\.font\.js/,
                    use: "css-loader?minimize!webfonts-loader"
                    // loader: ExtractTextPlugin.extract({
                    //     use: [
                    //         isDevBuild ? "css-loader" : "css-loader?minimize",
                    //         "webfonts-loader"
                    //     ]
                    // })
                }
            ]
        },
        target: 'node',
        //resolve: { mainFields: ['main'] },
        entry: {
            // 'satellitePages': glob.sync('./ClientApp/satellite/*.vue'),
            'main-server': './ClientApp/server.js' },
        devtool: 'sourcemap',
        output: {
            libraryTarget: 'commonjs2',
            path:  path.join(__dirname, 'wwwroot/dist'),
            filename: '[name].js',
            // publicPath: 'dist/'
        },
        //module: {
        //    rules: [ { test: /\.css(\?|$)/, use: ['to-string-loader', isDevBuild ? 'css-loader' : 'css-loader?minimize' ] } ]
        //},
        plugins: [
            new webpack.DefinePlugin({
                'NICE_FEATURE':true,
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? "development" : "production")
                }
            })
            // new webpack.DllPlugin({
            //     publicPath: 'dist/',
            //     filename: '[name].js',
            //     library: '[name]_[hash]',
            //     path: path.join(__dirname, 'ClientApp', 'dist', '[name]-manifest.json'),
            //     name: '[name]_[hash]'
            // }),

        ].concat(isDevBuild ? [
            // new CleanWebpackPlugin(path.join(__dirname, "ClientApp", "dist"))
        ] : [])
    }
}
