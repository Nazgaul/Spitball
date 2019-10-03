const path = require("path");
const webpack = require("webpack");
const bundleOutputDir = "./wwwroot/dist";
const MiniCssExtractPlugin = require("mini-css-extract-plugin-with-rtl");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const WebpackRTLPlugin = require("webpack-rtl-plugin");
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;


module.exports = (env) => {
    
    const isDevBuild =  !(env && env.prod);
    // const isDevBuild =  false;
    const mode = isDevBuild ? 'development' : 'production';
    // This is the "main" file which should include all other modules
   

    return {
        stats: { children: false },
        context: __dirname,
        module: {
            
            rules: [
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
                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: [
                        {
                            loader: "url-loader",
                            options: {
                                limit: 8192
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
                    //include: /ClientApp/
                    test: /\.vue$/,  loader: 'vue-loader',
                    options: {
                        loaders:
                        {
                            js: {
                                use: {
                                    loader: 'babel-loader'

                                }
                            },
                            less: ['vue-style-loader', 'css-loader', 'less-loader'],
                        }
                    }

                    
                },
                {
                    test: /\.css(\?|$)/,
                    use: 
                        isDevBuild ? ['vue-style-loader','rtl-css-loader']
                             :
                            [MiniCssExtractPlugin.loader,'css-loader']
                        //{
                        //    loader: MiniCssExtractPlugin.loader,
                        //    options: {
                        //      // only enable hot in development
                        //      hmr: isDevBuild,
                        //      // if hmr does not work, this is a forceful method.
                        //      reloadAll: true,
                        //    },
                        //  },
                        //'style-loader', 'rtl-css-loader'
                        //'css-loader'
                    
                },
                {
                    test: /\.less(\?|$)/,
                    use:
                        isDevBuild ? ['vue-style-loader', 'rtl-css-loader', 'less-loader']
                        :
                        [MiniCssExtractPlugin.loader, 'css-loader', 'less-loader']
                  
                },
            ]
        },
        devtool: false,
        optimization: {
            minimize: !isDevBuild
            },
        plugins: [
            new VueLoaderPlugin(),
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                }
            }),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require("./wwwroot/dist/vendor-manifest.json")
            })
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
            ]
            : [
                new BundleAnalyzerPlugin({
                    analyzerMode: 'disabled',
                    generateStatsFile: true,
                    statsOptions: { source: false }
                }),
                new MiniCssExtractPlugin({
                    filename: "site.[contenthash].css",
                    rtlEnabled: true,
                    ignoreOrder: true,
                    // allChunks: true

                }),
                new WebpackRTLPlugin({
                    // filename: 'site.[contenthash].rtl.css',
                    //minify: true
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
            ]),
        mode: mode,
        entry: { main: ["@babel/polyfill", "./ClientApp/client.js"] },
       
        
        output: {
            path: path.join(__dirname, bundleOutputDir),
            publicPath: 'dist/',
            filename: isDevBuild ? "[name].js" : "[name].[chunkhash].js",
        }

    };
}