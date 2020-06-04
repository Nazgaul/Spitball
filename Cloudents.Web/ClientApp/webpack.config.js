﻿/* eslint-disable no-undef */
const path = require("path");
const webpack = require("webpack");
const bundleOutputDir = "./../wwwroot/dist";
const MiniCssExtractPluginRtl = require("@automattic/mini-css-extract-plugin-with-rtl");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const webpackRtlPlugin = require("webpack-rtl-plugin");
const VueLoaderPlugin = require("vue-loader/lib/plugin");
//const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const TerserPlugin = require("terser-webpack-plugin");
const VuetifyLoaderPlugin = require("vuetify-loader/lib/plugin");
var CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin');
const { RetryChunkLoadPlugin } = require('webpack-retry-chunk-load-plugin');
const UnusedWebpackPlugin = require('unused-webpack-plugin');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    const mode = isDevBuild ? "development" : "production";

    return {
        stats: { children: false },
        context: __dirname,
        module: {
            rules: [
                {
                    test: /\.svg$/,
                    include: path.resolve(__dirname),
                    exclude: path.resolve(__dirname ,'node_modules'),
                    use: [
                        {
                            loader: "vue-svg-loader",
                            options: {
                                // optional [svgo](https://github.com/svg/svgo) options
                                svgo: {
                                    plugins: [
                                        { removeDoctype: true },
                                        { removeComments: true },
                                        { removeTitle: true },
                                        { prefixIds: true },
                                        // { cleanupIDs: { prefix: `svg${hash(relative(context, resource))}` } },
                                        { convertPathData: false },
                                        { removeMetadata: true },
                                        { cleanupAttrs: false },
                                        { removeEditorsNSData: true },
                                        { removeEmptyAttrs: true },
                                        { convertTransform: false },
                                        { removeUnusedNS: true }

                                    ]
                                }
                            }
                        }
                    ]

                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    include: path.resolve(__dirname),
                    exclude: path.resolve(__dirname ,'node_modules'),
                    use: [
                        {
                            loader: "url-loader",
                            options: {
                                limit: 8192,
                                esModule: false,
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
                                    quality: 80
                                }
                            }
                        }
                    ]

                },
                {
                    test: path.resolve(__dirname, "./myFont.font.js"),
                    exclude: path.resolve(__dirname ,'node_modules'),
                    use: isDevBuild ? [
                        {
                            loader: "vue-style-loader"
                        },
                        {
                            loader: "rtl-css-loader"
                        },
                        {
                            loader: "webfonts-loader",
                            options: {
                                publicPath: "/dist/"
                            }
                        }]
                        :
                        [
                            {
                                loader: MiniCssExtractPluginRtl.loader
                            },
                            {
                                loader: "css-loader"
                            },
                            {
                                loader: "webfonts-loader",
                                options: {
                                    publicPath: "/dist/"
                                }
                            }
                        ]
                },
                {
                    test: /\.(ogg|mp3|wav)$/i,
                    loader: "file-loader",
                    exclude: path.resolve(__dirname ,'node_modules'),

                },
                {
                    test: /\.js$/,
                    include: path.resolve(__dirname),
                    loader: "babel-loader",
                    exclude: path.resolve(__dirname ,'node_modules'),
                },
                {
                    test: /\.vue$/,
                    include: [
                        path.resolve(__dirname)
                    ],
                    loader: "vue-loader",
                    exclude: path.resolve(__dirname ,'node_modules'),
                },
                {
                    test: /\.css(\?|$)/,
                    include: [
                        path.resolve(__dirname),
                        path.resolve(__dirname, "./node_modules/codemirror/addon"),
                        path.resolve(__dirname, "./node_modules/vue-mathjax/dist/vue-mathjax.css")
                    ],
                   // exclude: 'node_modules/*',
                    use:
                        isDevBuild ? ["vue-style-loader", "rtl-css-loader"]
                            :
                            [MiniCssExtractPluginRtl.loader, "css-loader"]
                },
                {
                    test: /\.s[ac]ss$/i,
                    include: path.resolve(__dirname, "./node_modules/vuetify/src/"),
                    use:
                        isDevBuild ? ["vue-style-loader", "rtl-css-loader",
                            {
                                loader: "sass-loader",
                                options: {
                                    implementation: require("sass"),
                                    sassOptions: {
                                        fiber: require("fibers"),
                                        indentedSyntax: true, // optional
                                    },
                                    prependData: "@import './styles/variables.scss'",
                                }
                            }
                        ]
                            :
                            [
                                {
                                    loader: MiniCssExtractPluginRtl.loader,
                                    options: {
                                        publicPath: "/dist/"
                                    }
                                },
                                {
                                    loader: "css-loader"
                                },
                                {
                                    loader: "sass-loader",
                                    options: {
                                        implementation: require("sass"),
                                        sassOptions: {
                                            fiber: require("fibers"),
                                            indentedSyntax: true // optional
                                        },
                                        prependData: "@import './styles/variables.scss'"
                                    }
                                }
                            ]
                },
                {
                    test: /\.less(\?|$)/,
                    include: path.resolve(__dirname),
                    exclude: path.resolve(__dirname ,'node_modules'),
                    use:
                        isDevBuild ? ["vue-style-loader", "rtl-css-loader", "less-loader"]
                            :
                            [
                                {
                                    loader: MiniCssExtractPluginRtl.loader,
                                    options: {
                                        publicPath: "/dist/"
                                    }
                                },
                                {
                                    loader: "css-loader"
                                },
                                {
                                    loader: "less-loader"
                                }
                            ]
                },

            ]
        },
        devtool: false,
        optimization: {
            minimize: !isDevBuild,
            //splitChunks: {
            //    chunks: 'all'
            //},
            minimizer: !isDevBuild ? [
                new TerserPlugin({
                    terserOptions: {
                        compress: {
                            drop_console: true
                        }
                    }
                }),
                new OptimizeCssAssetsPlugin({
                    cssProcessor: require("cssnano"),
                    cssProcessorPluginOptions: {
                        preset: ["default", {
                            discardComments: {
                                remove: function (comment) {
                                    return !comment.includes("rtl");
                                },
                                removeAll: true
                            },
                            reduceIdents: false
                        }]
                    },
                    canPrint: true
                })] : []
        },
        plugins: [
            new VueLoaderPlugin(),
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? "development" : "production")
                }
            }),
            new VuetifyLoaderPlugin(),
           
            
            //new BundleAnalyzerPlugin({
            //    analyzerMode: 'disabled',
            //    generateStatsFile: true,
            //    statsOptions: { source: false }
            //}),
        ].concat(isDevBuild
            ? [
                new CaseSensitivePathsPlugin(),
                new webpack.SourceMapDevToolPlugin({
                    filename: "[file].map", // Remove this line if you prefer inline source maps
                    moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                }),
                new UnusedWebpackPlugin({
                    // Source directories
                    directories: [path.join(__dirname)],
                    // Exclude patterns
                    exclude: ['*.test.js', 'font-icon/*','*.spec.js','node_modules/*'],
                    // Root directory (optional)
                   // root: path.join(__dirname, 'ClientApp'),
                }),
            ]
            : [
                new MiniCssExtractPluginRtl({
                    filename: "site.[contenthash].css",
                    rtlEnabled: true,
                    ignoreOrder: true
                    // allChunks: true

                }),
                new webpackRtlPlugin({
                    minify: false
                }),
                new RetryChunkLoadPlugin({
                    // optional stringified function to get the cache busting query string appended to the script src
                    // if not set will default to appending the string `?cache-bust=true`
                    //cacheBust: `function() {
                    //        return Date.now();
                    //    }`
                })
            ]),
        mode: mode,
        entry: { main: ["@babel/polyfill", "./client.js"] },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            publicPath: "dist/",
            filename: isDevBuild ? "[name].js" : "[name].[chunkhash].js",
            crossOriginLoading: 'anonymous'
        }
    };
};