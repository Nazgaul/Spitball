const path = require('path');
const webpack = require('webpack');
const bundleOutputDir = './wwwroot/dist';
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = (env) => {
    const isDevBuild = !(env && env.prod);
    // This is the "main" file which should include all other modules
    return [
        {
            entry: { main: './ClientApp/main.js' },
            context: __dirname,
            resolve: {
                extensions: [".js", ".vue"],
                alias: {
                    "vue$": "vue/dist/vue.esm.js"
                }
            },
            module: {
                // Special compilation rules
                loaders: [
                    {
                        // Ask webpack to check: If this file ends with .js, then apply some transforms
                        test: /\.js$/,
                        // Transform it with babel
                        loader: 'babel-loader',
                        include: /ClientApp/
                        // don't transform node_modules folder (which don't need to be compiled)
                        //exclude: /node_modules/
                    },
                    {
                        test: /\.vue$/,
                        loader: 'vue-loader',
                        include: /ClientApp/,
                        options: { //maybe
                            extractCSS:true
                        }
                    }
                ]
            },
            // Where should the compiled file go?
            output: {
                // To the `dist` folder
                path: path.join(__dirname, bundleOutputDir),
                // With the filename `build.js` so it's dist/build.js
                filename: '[name].js',
                publicPath: 'dist/'
            },
            //resolve: {
            //    extensions: ['.js', '.vue', '.json'],
            //    alias: {
            //        'vue$': 'vue/dist/vue.esm.js'
            //    }
            //},
            plugins: [
                new webpack.DefinePlugin({
                    'process.env': {
                        NODE_ENV: JSON.stringify(isDevBuild ? 'development' : 'production')
                    }
                },
                    new ExtractTextPlugin("style.css"))
            ].concat(isDevBuild
                ? [
                    // Plugins that apply in development builds only
                    new webpack.SourceMapDevToolPlugin({
                        filename: '[file].map', // Remove this line if you prefer inline source maps
                        moduleFilenameTemplate:
                        path.relative(bundleOutputDir,
                            '[resourcePath]') // Point sourcemap entries to the original file locations on disk
                    })
                ]
                : [
                    // Plugins that apply in production builds only
                    new webpack.optimize.UglifyJsPlugin()
                    //new ExtractTextPlugin('site.css')
                ])
        }
    ];
}