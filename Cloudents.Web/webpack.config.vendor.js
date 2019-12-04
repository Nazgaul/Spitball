const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin-with-rtl");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const webpackRtlPlugin = require("webpack-rtl-plugin");
const bundleOutputDir = "./wwwroot/dist";
const RemovePlugin = require('remove-files-webpack-plugin');

const allModules = [
    // "@babel/polyfill",
    // "vue",
    // "vue-router",
    // "vuex",
    // "vue-analytics",
    // "axios",
    // "query-string",
    // "vue-plugin-load-script",
    // "vue-line-clamp",
    "./ClientApp/main.styl",
    "./ClientApp/components/app/main.less",
    "./ClientApp/myFont.font.js",
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
    "vuetify/es5/components/VSystemBar",
    "vuetify/es5/components/VTabs",
    "vuetify/es5/directives/scroll",
    "vuetify/es5/components/VIcon",
    "vuetify/es5/components/VSnackbar",
    "vuetify/es5/components/VNavigationDrawer",
    "vuetify/es5/components/VAvatar",
    "vuetify/es5/components/VPagination",
    "vuetify/es5/components/VDataTable",
    "vuetify/es5/components/VStepper",
    "vuetify/es5/components/VCombobox",
    "vuetify/es5/components/VCheckbox",
    "vuetify/es5/components/VBottomNav",
    "vuetify/es5/components/VTextarea",
    "vuetify/es5/components/VRating",
    "vuetify/es5/components/VForm",
    "vuetify/es5/components/VAutocomplete",
    "vuetify/es5/components/VSheet",
    "vuetify/es5/components/VCalendar"

];

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    // const isDevBuild =  false;
    const mode =  isDevBuild ? 'development':'production';

    const clientBundleConfig =
    {
        stats: { modules: false, children: false },
        module: {
            rules: [
                {
                    test: /\.css(\?|$)/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader'
                    ]
                },
                {
                    test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/,
                    use: [
                        'url-loader?limit=8192'
                    ]
                },
                {
                    test: /\.styl(\?|$)/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        //{
                        //    loader: "style-loader" // creates style nodes from JS strings
                        //},
                        {
                            loader: "css-loader" // translates CSS into CommonJS
                        },
                        {
                            loader: 'stylus-loader' // compiles Stylus to CSS
                        }

                    ]
                },
                {
                    test: /\.less(\?|$)/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        {
                            loader: 'css-loader' // translates CSS into CommonJS
                        },
                        {
                            loader: 'less-loader' // compiles Less to CSS
                        }
                    ]
                },
                {
                    test: /\.font\.js/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        'css-loader',
                        {
                            loader: 'webfonts-loader',
                            options: {
                                publicPath: '/dist/'
                            }
                        }
                    ]
                }
               
            ]
        },
        devtool: false,
        optimization: {
            minimize: !isDevBuild
        },
        plugins: [
           
            new MiniCssExtractPlugin({
                filename: '[name].[contenthash].css'
            }),

            new webpackRtlPlugin({
            }),
            //new StatsWriterPlugin({
            //    filename: "vendor.json",
            //    transform: function (data, opts) {
            //        return JSON.stringify(data.assetsByChunkName);
            //    }
            //}),
            new webpack.DllPlugin({
                path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
                name: "[name]"
            })
        ]
            .concat(isDevBuild ? [
                new RemovePlugin({
                    before: {
                        // parameters.
                        include: ['./wwwroot/dist']
                    },
                    after: {
                        // parameters.
                    }
                }),
                new webpack.SourceMapDevToolPlugin({
                    filename: "[file].map", // Remove this line if you prefer inline source maps
                    moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            "[resourcePath]") // Point sourcemap entries to the original file locations on disk
                })
            ] : [
                    new OptimizeCssAssetsPlugin({
                        //assetNameRegExp: /.css$/g,
                        cssProcessor: require("cssnano"),
                        cssProcessorOptions: {
                            discardComments: { removeAll: true },
                            reduceIdents: false
                        },
                        canPrint: true
                    })
                ]),
        mode,
        output: {
            path: path.join(__dirname, "wwwroot", "dist"),
            //publicPath: "/dist/vendor",
            filename: "[name].[chunkhash].js",
            library: "[name]"
        },

        entry: {
            vendor: allModules
        }
    };

    return [clientBundleConfig];

};
