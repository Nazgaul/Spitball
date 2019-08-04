﻿const path = require("path");
const webpack = require("webpack");
const bundleOutputDir = "./wwwroot/dist";
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const merge = require('webpack-merge');
//const serverConfig = require("./webpack.config.server.js");
var StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;
//var t = require("./webpack.global.js");
//const VueSSRServerPlugin = require('vue-server-renderer/server-plugin');
//const CleanWebpackPlugin = require("clean-webpack-plugin");
const WebpackRTLPlugin = require("webpack-rtl-plugin");
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;


module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    // This is the "main" file which should include all other modules
    const sharedConfig = () => ({
        // return [
        // {
        // entry: {
        //     main: "./ClientApp/main.js"

        // },
        stats: { children: false },
        context: __dirname,
        module: {
            loaders: [
                {
                    test: /\.svg$/,
                    loader: "vue-svg-loader",
                    options: {
                        // optional [svgo](https://github.com/svg/svgo) options
                        svgo: {
                            plugins: [
                                { removeDoctype: true },
                                { removeComments: true },
                                { removeTitle: true },
                                { cleanupIDs: true },
                                {convertPathData: false},
                                {removeMetadata: true},
                                {cleanupAttrs: false},
                                {removeEditorsNSData: true},
                                {removeEmptyAttrs: true },
                                {convertTransform: false},
                                {removeUnusedNS: true}

                            ]
                        }
                    }

                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: [
                        {
                            loader: "url-loader",
                            options: {
                                limit: 8192
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
                                },
                                mozjpeg: {
                                    progressive: true,
                                    quality: 90
                                }
                            }
                        }
                    ]

                },
                {
                    test: /\.js$/,
                    loader: "babel-loader"
                },
                {
                    test: /\.vue$/,
                    loader: "vue-loader",
                    options: {
                        preserveWhitespace: isDevBuild ? true : false,
                        loaders: {
                            //css: isDevBuild
                            //    ? "vue-style-loader!css-loader"
                            //    : ExtractTextPlugin.extract({
                            //        use: "css-loader?minimize",
                            //        fallback: "vue-style-loader"
                            //    }),
                            //less: isDevBuild
                            //    ? "vue-style-loader!css-loader!less-loader"
                            //    : ExtractTextPlugin.extract({
                            //        use: "css-loader?minimize!less-loader",
                            //        fallback: "vue-style-loader"
                            //    }),
                            //scss: isDevBuild
                            //    ? "vue-style-loader!css-loader!sass-loader"
                            //    : ExtractTextPlugin.extract({
                            //        use: "css-loader?minimize!sass-loader",
                            //        fallback: "vue-style-loader"
                            //    })

                            //RTL support
                            css: ExtractTextPlugin.extract({
                                use: "css-loader",
                                fallback: "vue-style-loader"
                            }),
                            less: ExtractTextPlugin.extract({
                                use: "css-loader!less-loader",
                                fallback: "vue-style-loader"
                            }),
                            scss: ExtractTextPlugin.extract({
                                use: "css-loader!sass-loader",
                                fallback: "vue-style-loader"
                            })
                        }
                    }
                },
                {
                    test: /\.css$/,
                    use: isDevBuild
                        ? ["style-loader", "css-loader"]
                        : ExtractTextPlugin.extract({use: "css-loader"})
                }
            ]
        },
        plugins: [
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? "development" : "production")
                }
            })
        ].concat(isDevBuild
            ? [
                new ExtractTextPlugin({
                    filename: "site.[contenthash].css",
                    allChunks: true
                   
                }),
                new WebpackRTLPlugin({
                    filename: 'site.[contenthash].rtl.css',
                    minify: false
                })
            ]
            : [
                // Plugins that apply in production builds only
               
                new ExtractTextPlugin({filename: "site.[contenthash].css", allChunks: true}),
                new WebpackRTLPlugin({
                    filename: 'site.[contenthash].rtl.css',
                    minify: false
                }),
                new OptimizeCssAssetsPlugin({
                    //assetNameRegExp: /.css$/g,
                    cssProcessor: require("cssnano"),
                    cssProcessorOptions: {
                        discardComments: {
                            remove: function (comment) {
                                return !comment.includes("rtl");
                            },
                            removeAll: true
                        },
                        reduceIdents: false
                    },
                    canPrint: true
                })
                
                //new PurifyCSSPlugin({
                //    // Give paths to parse for rules. These should be absolute!
                //    paths: glob.sync(path.join(__dirname, 'clientapp/**/*.vue')),
                //    minimize: true,
                //    purifyOptions: {
                //        whitelist: ["spitball-*"]
                //    }
                //})
            ])
    });

    const clientBundleConfig = merge(sharedConfig(), {
        entry: { main: ["babel-polyfill", "./ClientApp/client.js"]},
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: isDevBuild ? "[name].js" : "[name].[chunkhash].js",
            publicPath: "/dist/"
        },
        plugins: [
            new StatsWriterPlugin({
                filename: "main.json",
                transform: function (data, opts) {
                    return JSON.stringify(data.assetsByChunkName, null, 2);
                }
            }),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require("./wwwroot/dist/vendor-manifest.json")
            }),
            
        ].concat(isDevBuild
            ? [
                new BundleAnalyzerPlugin({
                    analyzerMode: 'disabled',
                    generateStatsFile: true,
                    statsOptions: { source: false }
                }),
                new webpack.SourceMapDevToolPlugin({
                    filename: "[file].map", // Remove this line if you prefer inline source maps
                    moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                })

            ] :
            [
                new webpack.optimize.UglifyJsPlugin({
                    compress: {
                        warnings: false,
                        drop_console: true
                    }
                })
                //new webpack.optimize.UglifyJsPlugin({
                //    compress: {
                //     //   dead_code: true,
                //        drop_debugger :true,
                //        //warnings: false,
                //        drop_console: true,
                //        //pure_funcs: ['console.log','console.trace']
                //    }
                //}),
            ])

    });

   

    return [clientBundleConfig];
}