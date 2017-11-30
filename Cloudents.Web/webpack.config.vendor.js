const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
var resolve = (p) => path.resolve(__dirname, p);
var Visualizer = require("webpack-visualizer-plugin");

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    const extractCSS = new ExtractTextPlugin('vendor.css');

    return [{
        stats: { modules: false },
        //resolve: { extensions: ['.js'] },
        entry: {
            vendor: [
                'vue',
                'vue-router',
                'vue-resource',
                'vuex',
                "./ClientApp/main.styl",
                "./wwwroot/content/main.less",
                "./ClientApp/myFont.font.js",
                'vuex-persistedstate',
                "vue-star-rating",
                "vuetify/es5/components/Vuetify",
                "vuetify/es5/components/VApp",
                "vuetify/es5/components/VGrid",
                "vuetify/es5/components/VChip",
                "vuetify/es5/components/VToolbar",
                "vuetify/es5/components/VExpansionPanel",
                "vuetify/es5/components/VList",
                "vuetify/es5/components/VTextField",
                "vuetify/es5/components/VAvatar",
                "vuetify/es5/components/VCard",
                "vuetify/es5/components/VCarousel",
                "vuetify/es5/components/VProgressCircular",
                "vuetify/es5/components/VProgressLinear",
                "vuetify/es5/components/VSubheader",
                "vuetify/es5/components/VDivider",
                "vuetify/es5/components/VSnackbar",
                "vuetify/es5/components/VDialog",
                "vuetify/es5/components/VBtn",
                "vuetify/es5/components/VTooltip",
                "vuetify/es5/components/VMenu",
                "vuetify/es5/components/VSwitch",
                "vuetify/es5/components/VTabs",
                "vuetify/es5/directives/scroll"
            ]
        },
        module: {
            rules: [
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: isDevBuild ? 'css-loader' : 'css-loader?minimize' }) },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=8192' },
                {
                    test: /\.styl$/,
                    loader: extractCSS.extract({ use: isDevBuild ? 'css-loader!stylus-loader' : 'css-loader?minimize!stylus-loader' })
                },
                {
                    test: /\.less$/,
                    exclude: /ClientApp/,
                    use: extractCSS.extract({ use: isDevBuild ? 'css-loader!less-loader' : 'css-loader?minimize!less-loader' })
                },
                {
                    test: /\.font\.js/,
                    loader: extractCSS.extract({
                        use: [
                            isDevBuild ? 'css-loader' : 'css-loader?minimize',
                            {
                                loader: 'webfonts-loader' //TODO: need to add svg compression

                            }
                        ]
                    })
                }
            ]
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: '/dist/',
            filename: '[name].js',
            library: '[name]_[hash]'
        },
        plugins: [
            extractCSS,
            new Visualizer({
                filename: "./statistics-vendor.html"
            }),
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            }),
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [] : [
            new webpack.optimize.UglifyJsPlugin()
        ])
    }];
};
