const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
var resolve = (p) => path.resolve(__dirname, p);

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
                //'vuetify',
                'vuex',
                "./ClientApp/main.styl",
                "./wwwroot/content/main.less",
                //'vuetify/dist/vuetify.css',
                'vuex-persistedstate'
            ]
        },
        module: {
            rules: [
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: isDevBuild ? 'css-loader' : 'css-loader?minimize' }) },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=8192' },
                {
                    test: /\.styl$/,
                    loader: extractCSS.extract({ use: isDevBuild ? 'css-loader!stylus-loader' : 'css-loader?minimize!stylus-loader' })
                    //loader: isDevBuild ? ['style-loader', 'css-loader', 'stylus-loader', {
                    //    loader: 'vuetify-loader',
                    //    options: {
                    //        theme: resolve('./ClientApp/theme.styl')
                    //    }
                    //}] : ExtractTextPlugin.extract({
                    //    use: 'css-loader?minimize!stylus-loader'

                    //})
                },
                {
                    test: /\.less$/,
                    exclude: /ClientApp/,
                    use: extractCSS.extract({ use: isDevBuild ? 'css-loader!less-loader' : 'css-loader?minimize!less-loader' })
                 //isDevBuild ? ['style-loader', 'css-loader', "less-loader"] :
                 //        ExtractTextPlugin.extract(
                 //           {
                 //               use: "css-loader?minimize!less-loader",
                 //               fallback: 'style-loader'
                 //           }
                      //  )

                },
            ]
        },
        output: {
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]_[hash]'
        },
        plugins: [
            extractCSS,
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
