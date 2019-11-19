module.exports = function (api) {
    api.cache(true);
  
    const presets = [ 
            [
                "@babel/preset-env",
            ]
        ];
    const plugins = [ 
        "syntax-dynamic-import",
        "transform-object-rest-spread",
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