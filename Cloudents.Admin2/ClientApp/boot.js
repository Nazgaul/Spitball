import { app, router } from './main'

console.log("sss");
router.onReady(()=>{
    app.$mount('#app')
})
