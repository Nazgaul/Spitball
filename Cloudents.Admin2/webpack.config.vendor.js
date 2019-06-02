const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    const devMode = isDevBuild ? 'development':'production';

    return [{
        mode: devMode,
        stats: { modules: false },
        resolve: { extensions: [ '.js' ] },
        entry: {
            vendor: [
                'vue',
                'vue-router',
                'vuex',
                "vuetify",
                'v-toaster',
                'axios',
                "vue-clipboard2"
            ]
        },
        module: {
            rules: [
                {
                    test: /\.css(\?|$)/,
                    use: [
                        MiniCssExtractPlugin.loader, 
                        isDevBuild ? 'css-loader' : 'css-loader?minimize' 
                    ]
                },
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' }
            ]
        },
        output: { 
            path: path.join(__dirname, 'wwwroot', 'dist'),
            publicPath: 'dist/',
            filename: '[name].js',
            library: '[name]'
        },
        plugins: [
            new MiniCssExtractPlugin({ filename: '[name].css'}),
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            }),
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]'
            })
        ].concat(isDevBuild ? [] : [
            new UglifyJsPlugin()
            //new webpack.optimize.UglifyJsPlugin()
        ])
    }];
};
