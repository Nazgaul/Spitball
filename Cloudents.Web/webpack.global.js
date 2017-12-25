module.exports = {
  getdist: function (isDevBuild) {
    return isDevBuild ? "/dist/" : "//spitball.azureedge.net/dist/";
  },

  loaders: function(isDevBuild) {
    return [
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
              { cleanupIDs: true }
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
              // useRelativePath: !isDevBuild,
              //publicPath: !isDevBuild ? 'cdnUrl' : '/dist/'

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
            css: isDevBuild
              ? "vue-style-loader!css-loader"
              : ExtractTextPlugin.extract({
                use: "css-loader?minimize",
                fallback: "vue-style-loader"
              }),
            less: isDevBuild
              ? "vue-style-loader!css-loader!less-loader"
              : ExtractTextPlugin.extract({
                use: "css-loader?minimize!less-loader",
                fallback: "vue-style-loader"
              })
          }
        }
      },
      {
        test: /\.css$/,
        use: isDevBuild
          ? ["style-loader", "css-loader"]
          : ExtractTextPlugin.extract({ use: "css-loader?minimize" })
      }
    ];
  }
};