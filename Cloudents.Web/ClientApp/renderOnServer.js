//import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
process.env.VUE_ENV = 'server';

const fs = require('fs');
const path = require('path');

const filePath = path.join(__dirname, './dist/main-server.js');
const code = fs.readFileSync(filePath, 'utf8');
const bundleRenderer = require('vue-server-renderer').createBundleRenderer(code);

var prerendering = require('aspnet-prerendering');



module.exports = prerendering.createServerRenderer(function (params) {

  return new Promise(function (resolve, reject) {


      //const filePath = path.join(__dirname, './dist/main-server.js')
      //const code = fs.readFileSync(filePath, 'utf8');

    //resolve({
    //    html: "<div>" + code + "</div>"
    //  });
    //return;
    const context = { url: params.url };


    //renderer.renderToString(app).then(html => {
    //    resolve({
    //        html: html,
    //        globals: {
    //            __INITIAL_STATE__: context.state
    //        }
    //    });
    //}).catch(err => {
    //    reject(err.message);
    //})

    bundleRenderer.renderToString(context,
      (err, resultHtml) => {
        if (err) {
          if (err.code === 404) {
            resolve({
              html: "a" + JSON.stringify(err.router.options),
              statusCode: 404
            });
            //res.status(404).end('Page not found')
          } else {
            resolve({
                html: "ba " +  err,
              statusCode: 500
            });
          }
        }
        resolve({
            html: resultHtml
          //html: result + "<div>" + JSON.stringify(params, null, 2) + "</div>"
          //html: resultHtml,
          //globals: {
          //  __INITIAL_STATE__: context.state
          //}
        });
      });

  });
});