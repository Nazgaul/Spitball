﻿const path = require("path");
const webpack = require("webpack");
const bundleOutputDir = "./wwwroot/dist";
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const merge = require('webpack-merge');
const serverConfig = require("./webpack.config.server.js");
var Visualizer = require("webpack-visualizer-plugin");
var StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;
var t = require("./webpack.global.js");
const VueSSRServerPlugin = require('vue-server-renderer/server-plugin');
const CleanWebpackPlugin = require("clean-webpack-plugin");
const { UnusedFilesWebpackPlugin } = require("unused-files-webpack-plugin");
const WebpackRTLPlugin = require("webpack-rtl-plugin");



module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    // This is the "main" file which should include all other modules
    const sharedConfig = () => ({
        // return [
        // {
        // entry: {
        //     main: "./ClientApp/main.js"

        // },
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
                                { cleanupIDs: true }
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
                                limit: 8192,
                                // useRelativePath: !isDevBuild,
                                //publicPath: !isDevBuild ? 'cdnUrl' : '/dist/'

                            }
                        },
                        {
                            loader: "image-webpack-loader",
                            options: {
                                bypassOnDebug: false,
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
                                    use: "css-loader?minimize",
                                    fallback: "vue-style-loader"
                                }),
                            less: ExtractTextPlugin.extract({
                                    use: "css-loader?minimize!less-loader",
                                    fallback: "vue-style-loader"
                                }),
                            scss: ExtractTextPlugin.extract({
                                    use: "css-loader?minimize!sass-loader",
                                    fallback: "vue-style-loader"
                                })
                        }
                    }
                },
                {
                    test: /\.css$/,
                    use: isDevBuild
                        ? ["style-loader", "css-loader"]
                        : ExtractTextPlugin.extract({ use: "css-loader?minimize" })
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
                new ExtractTextPlugin({ filename: "site.[contenthash].css", allChunks: true }),
                new WebpackRTLPlugin({
                    filename: '[name].[contenthash].rtl.css'
                })
            ]
            : [
                // Plugins that apply in production builds only
                new ExtractTextPlugin({ filename: "site.[contenthash].css", allChunks: true }),
                new WebpackRTLPlugin({
                    filename: '[name].[contenthash].rtl.css'
                }),
                new OptimizeCssAssetsPlugin({
                    assetNameRegExp: /\.optimize\.css$/g,
                    cssProcessor: require("cssnano"),
                    cssProcessorOptions: { discardComments: { removeAll: true } },
                    canPrint: true
                }),
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
        entry: { 'main': './ClientApp/client.js' },
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
            })
        ].concat(isDevBuild
            ? [
                
                new Visualizer({
                    filename: "./statistics.html"
                }),
                new webpack.SourceMapDevToolPlugin({
                filename: "[file].map", // Remove this line if you prefer inline source maps
                moduleFilenameTemplate:
                    path.relative(bundleOutputDir,
                        "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                }),
                //new UnusedFilesWebpackPlugin({
                //    patterns: "./ClientApp/**/*.*"
                //})
                //new UnusedWebpackPlugin({
                //    // Source directories
                //    directories: [path.join(__dirname, 'src')],
                //    // Exclude patterns
                //    exclude: ['*.test.js',],
                //    // Root directory (optional)
                //    root: __dirname,
                //})
            ] :
            [
                new webpack.optimize.UglifyJsPlugin({
                    compress: {
                        warnings: false,
                        drop_console: true
                    }
                }),
            ])

    });

    const serverBundleConfig = merge(serverConfig(env), {
        //target: 'node',
        //entry: { 'main-server': './ClientApp/server.js' },
        //output: {
        //    libraryTarget: 'commonjs2',
        //    path: path.join(__dirname, './ClientApp/dist'),
        //    filename: '[name].js',
        //    publicPath: 'dist/' // Webpack dev middleware, if enabled, handles requests for this URL prefix
        //},
        //plugins: [
        //    new webpack.DllReferencePlugin({
        //        context: __dirname,
        //        manifest: require("./ClientApp/dist/vendor-manifest.json"),
        //        sourceType: 'commonjs2',
        //        //name: './vendor'
        //    }),
        //   // new VueSSRServerPlugin()
        //]
        // module: {
        //     rules: [
        //         {
        //             test: /\.json?$/,
        //             loader: 'json-loader'
        //         }
        //     ]
        // },
    });

    return [clientBundleConfig];
    //];
}