const path = require("path");
const webpack = require("webpack");
const bundleOutputDir = "./wwwroot/dist";
const MiniCssExtractPluginRtl = require("mini-css-extract-plugin-with-rtl");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const webpackRtlPlugin = require("webpack-rtl-plugin");
const VueLoaderPlugin = require('vue-loader/lib/plugin');
// const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin;
const TerserPlugin = require('terser-webpack-plugin');
const RemovePlugin = require('remove-files-webpack-plugin');
const VuetifyLoaderPlugin = require('vuetify-loader/lib/plugin');

module.exports = (env) => {
    const isDevBuild =  !(env && env.prod);
    const mode = isDevBuild ? 'development' : 'production';

    return {
        stats: { children: false },
        context: __dirname,
        module: {
            
            rules: [
                {
                    test: /\.svg$/,
                    include: path.resolve(__dirname, 'ClientApp'),
                    use:[
                        //{
                        //    loader: 'babel-loader',
                        //},
                        {
                            loader: "vue-svg-loader",
                        options: {
                        // optional [svgo](https://github.com/svg/svgo) options
                        svgo: {
                            plugins: [
                                { removeDoctype: true },
                                { removeComments: true },
                                { removeTitle: true },
                                { prefixIds: true},
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
                    include: path.resolve(__dirname, 'ClientApp'),
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
                                    quality: 80
                                }
                            }
                        }
                    ]

                },
                {
                    test: path.resolve(__dirname,'./clientApp/myFont.font.js'),
                    //include: path.resolve(__dirname, 'ClientApp'),
                    use: isDevBuild ? [
                        {
                            loader: 'vue-style-loader'
                        },
                        {
                            loader: 'rtl-css-loader'
                        },
                        {
                            loader: 'webfonts-loader',
                            options: {
                                publicPath: '/dist/'
                            }
                        }]
                        :
                    [
                        {
                            loader: MiniCssExtractPluginRtl.loader
                        },
                        {
                            loader: 'css-loader'
                        },
                        {
                            loader: 'webfonts-loader',
                            options: {
                                publicPath: '/dist/'
                            }
                        }
                    ],
                },
                {
                    test: /\.(ogg|mp3|wav)$/i,
                    loader: 'file-loader'
                },
                {
                    test: /\.js$/,
                    include: path.resolve(__dirname, 'ClientApp'),
                    loader: "babel-loader"
                },
                {
                    test: /\.vue$/,
                    include: [
                        path.resolve(__dirname, 'ClientApp'),
                        //path.resolve(__dirname, "./node_modules/vue-upload-component/src/*.vue")
                    ],
                    loader: 'vue-loader',
                },
                {
                    test: /\.css(\?|$)/,
                    use: 
                        isDevBuild ? ['vue-style-loader','rtl-css-loader']
                             :
                        [MiniCssExtractPluginRtl.loader,'css-loader']
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
                    test: /\.s[ac]ss$/i,
                    use:
                        isDevBuild ? ['vue-style-loader', 'rtl-css-loader', 
                        {
                            loader:'sass-loader',
                            options: {
                                implementation: require('sass'),
                                sassOptions: {
                                  fiber: require('fibers'),
                                  indentedSyntax: true, // optional
                                },
                                prependData: `@import "./ClientApp/variables.scss"`,
                            }
                        },
                    ]
                        :
                        [
                            {
                                loader: MiniCssExtractPluginRtl.loader,
                                options: {
                                    publicPath: '/dist/'
                                }
                            },
                            {
                                loader:'css-loader'
                            },
                            {
                                loader:'sass-loader',
                                options: {
                                    implementation: require('sass'),
                                    sassOptions: {
                                      fiber: require('fibers'),
                                      indentedSyntax: true // optional
                                    },
                                    prependData: `@import "./ClientApp/variables.scss"`,
                                }
                            }
                        ]
                },
                {
                    test: /\.less(\?|$)/,
                    include: path.resolve(__dirname, 'ClientApp'),
                    use:
                        isDevBuild ? ['vue-style-loader', 'rtl-css-loader', 'less-loader']
                        :
                        [
                            {
                                loader: MiniCssExtractPluginRtl.loader,
                                options: {
                                    publicPath: '/dist/'
                                }
                            },
                            {
                                loader:'css-loader'
                            },
                            {
                                loader:'less-loader'
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
            minimizer: !isDevBuild ? [new TerserPlugin({


            }), new OptimizeCssAssetsPlugin({
                //assetNameRegExp: /.css$/g,
                cssProcessor: require("cssnano"),
                cssProcessorPluginOptions: {
                    preset: ['default', {
                        discardComments: {
                            remove: function (comment) {
                                return !comment.includes("rtl");
                            },
                            removeAll: true
                        },
                        reduceIdents: false
                    }]

                    //discardComments: {
                    //    remove: function(comment) {
                    //        return !comment.includes("rtl");
                    //    },
                    //    removeAll: true
                    //},
                    //reduceIdents: false
                },
                canPrint: true
            })] : []
        },
        plugins: [
            new RemovePlugin({
                before: {
                    // parameters.
                    include: ['./wwwroot/dist']
                },
                after: {
                    // parameters.
                }
            }),
            
            new VueLoaderPlugin(),
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                }
            }),
            new VuetifyLoaderPlugin()
        ].concat(isDevBuild
            ? [
                // new BundleAnalyzerPlugin({
                //    analyzerMode: 'disabled',
                //    generateStatsFile: true,
                //    statsOptions: { source: false }
                // }),
                new webpack.SourceMapDevToolPlugin({
                    filename: "[file].map", // Remove this line if you prefer inline source maps
                    moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                })
            ]
            : [
                new MiniCssExtractPluginRtl({
                    filename: "site.[contenthash].css",
                    rtlEnabled: true,
                    ignoreOrder: true,

                    // allChunks: true

                }),
                new webpackRtlPlugin({
                    minify: false
                })
            ]),
        mode: mode,
        entry: { main: ["@babel/polyfill", "./ClientApp/client.js"] },
        // entry: { main: "./ClientApp/client.js" },
       
        
        output: {
            path: path.join(__dirname, bundleOutputDir),
            publicPath: 'dist/',
            filename: isDevBuild ? "[name].js" : "[name].[chunkhash].js",
        }

    };
};