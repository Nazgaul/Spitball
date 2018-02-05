const Vue = require('vue')
const server = require('express')()
// const renderer = require('vue-server-renderer').createRenderer()
const renderer = createRenderer({
    template: require('fs').readFileSync('./index.template.html', 'utf-8')
})

renderer.renderToString(app, (err, html) => {
    console.log(html) // will be the full page with app content injected.
})
server.get('*', (req, res) => {
    const app = new Vue({
        data: {
            url: req.url
        },
        template: `<div>The visited URL is: {{ url }}</div>`
    })

    renderer.renderToString(app, (err, html) => {
        if (err) {
            res.status(500).end('Internal Server Error')
            return
        }
        console.log("yifat");
        debugger;
        res.end(`
      <!DOCTYPE html>
      <html lang="en">
        <head><title>Hello</title></head>
        <body>Yifat${html}</body>
      </html>
    `)
    })
})
const port = process.env.PORT || 8080;
server.listen(port, () => {
    console.log(`server started at localhost:${port}`)
});