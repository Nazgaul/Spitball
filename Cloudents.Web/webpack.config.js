const path = require("path");
const glob = require('glob');
const webpack = require("webpack");
const bundleOutputDir = "./wwwroot/dist";
const ExtractTextPlugin = require("extract-text-webpack-plugin");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const PurifyCSSPlugin = require('purifycss-webpack');
var Visualizer = require("webpack-visualizer-plugin");
//var resolve = (p) => path.resolve(__dirname, p);


module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    // This is the "main" file which should include all other modules
    return [
        {
            entry: {
                main: "./ClientApp/main.js"

            },
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
                                    { cleanupIDs : true}
                                ]
                            }
                        }

                    },
                    {
                        test: /\.(png|jpg|jpeg|gif)$/,
                        use: [
                            {
                                loader: "url-loader", options: {
                                    limit: 8192

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
                        loader: "babel-loader"
                    },
                    {
                        test: /\.vue$/,
                        loader: "vue-loader",
                        options: {
                            preserveWhitespace: isDevBuild ? false : true,
                            loaders: {
                                css: isDevBuild ? "vue-style-loader!css-loader" : ExtractTextPlugin.extract({
                                    use: "css-loader?minimize",
                                    fallback: "vue-style-loader"
                                }),
                                less: isDevBuild ? "vue-style-loader!css-loader!less-loader" : ExtractTextPlugin.extract({
                                    use: "css-loader?minimize!less-loader",
                                    fallback: "vue-style-loader"
                                })
                            }
                        }
                    },
                    {
                        test: /\.css$/,
                        use: isDevBuild ? ["style-loader", "css-loader"] : ExtractTextPlugin.extract({ use: "css-loader?minimize" })
                    }
                ]
            },
            // Where should the compiled file go?
            output: {
                // To the `dist` folder
                path: path.join(__dirname, bundleOutputDir),
                // With the filename `build.js` so it's dist/build.js
                filename: "[name].js",
                publicPath: "/dist/"
            },
            plugins: [
                new webpack.DefinePlugin({
                    'process.env': {
                        NODE_ENV: JSON.stringify(isDevBuild ? "development" : "production")
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
                    // Plugins that apply in development builds only
                    new webpack.SourceMapDevToolPlugin({
                        filename: "[file].map", // Remove this line if you prefer inline source maps
                        moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                    })
                ]
                : [
                    // Plugins that apply in production builds only
                    new webpack.optimize.UglifyJsPlugin({
                        compress: {
                            warnings: false
                        }
                    }),
                    new ExtractTextPlugin({ filename: "site.css", allChunks: true }),
                    new OptimizeCssAssetsPlugin({
                        assetNameRegExp: /\.optimize\.css$/g,
                        cssProcessor: require("cssnano"),
                        cssProcessorOptions: { discardComments: { removeAll: true } },
                        canPrint: true
                    }),
                    new PurifyCSSPlugin({
                        // Give paths to parse for rules. These should be absolute!
                        paths: glob.sync(path.join(__dirname, 'clientapp/**/*.vue')),
                        minimize: true,
                        purifyOptions: {
                            whitelist: ["spitball-*"]
                        }
                    })

                ])
        }
    ];
}