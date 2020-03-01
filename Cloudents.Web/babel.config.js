module.exports = function (api) {
    api.cache(true);
  
    const presets = [ 
            [
                "@babel/preset-env"
            ]
        ];
    const plugins = [ 
        "@babel/plugin-syntax-dynamic-import",
        "transform-object-rest-spread",
        "@babel/plugin-proposal-optional-chaining",
        [
            "transform-imports",
            {
              "vuetify": {
                "transform": "vuetify/es5/components/${member}",
                "preventFullImport": true
              }
            }
          ]
    ];
  
    return {
      presets,
      plugins
    };
  }