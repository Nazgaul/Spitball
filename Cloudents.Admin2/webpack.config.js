const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const bundleOutputDir = './wwwroot/dist';
const VueLoaderPlugin = require('vue-loader/lib/plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    const devMode = isDevBuild ? 'development':'production';
    return [{
        mode: devMode,
        stats: { modules: false },
        context: __dirname,
        resolve: { extensions: [".js"] },
        entry: { 'main': './ClientApp/boot.js' },
        module: {
            rules: [
                {
                    test: /\.vue$/, include: /ClientApp/, loader: 'vue-loader',
                    options: {
                        loaders:
                            {
                            js: {
                                use: {
                                    loader: 'babel-loader'
                                   
                                }
                            },
                                scss: ['vue-style-loader', 'css-loader', 'sass-loader'],
                                sass: ['vue-style-loader', 'css-loader', 'sass-loader']
                            }
                    }
                },
                {
                    test: /\.js$/,
                    loader: 'babel-loader'
                },
                { test: /\.scss$/, use: ['style-loader', 'css-loader', 'sass-loader'] },
                { test: /\.css$/, use: isDevBuild ? ['style-loader', 'css-loader'] : [MiniCssExtractPlugin.loader, 'css-loader'] },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
            ]
        },
        output: {
            path: path.join(__dirname, bundleOutputDir),
            filename: '[name].js',
            publicPath: 'dist/'
        },
        devtool: isDevBuild ? 'inline-source-map' : 'source-map',
        plugins: [
            new VueLoaderPlugin(),
            new CheckerPlugin(),
            new webpack.DefinePlugin({
                'process.env': {
                    NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                }
            }),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            })
        ].concat(isDevBuild ? [
            
            // Plugins that apply in development builds only
            //new webpack.SourceMapDevToolPlugin({
            //    filename: '[file].map', // Remove this line if you prefer inline source maps
            //    moduleFilenameTemplate: path.relative(bundleOutputDir, '[resourcePath]'), // Point sourcemap entries to the original file locations on disk
            //})
        ] : [
            new UglifyJsPlugin(),
            // Plugins that apply in production builds only
            //new webpack.optimize.UglifyJsPlugin(),
                new MiniCssExtractPlugin({
                    filename: 'site.css'
                })
            //new ExtractTextPlugin('site.css')
        ])
    }];
};
