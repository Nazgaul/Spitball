
<template >
    <div id="sections">
        Yifat
        <div class="overlay" ></div>
        <app-aside></app-aside>
    <!--<app-header ></app-header>-->
    <section>
        <input id="menu-checkbox" type="checkbox" class="hide_641" />
        
        <div class="content"></div>
    </section>
        </div>
</template>
<script src="./sections.js"></script>
<!--<script>
    //var routes = [
    //    { path: '/ask', component: require('./header/header.vue.html') },
    //    { path: '/note', component: require('./header/header.vue.html') },
    //    { path: '/flashcard', component: require('./header/header.vue.html') }
    //];
    import Vue from 'vue'
    import AsidePage from "./aside/navbar.vue"
    Vue.component("app-aside", {
        render: h => h(require(AsidePage))
        //render: h => h(require('./aside/navbar.vue'))
    });
    window.addEventListener('load', function () {
        var page = new Vue({
            el: "#aaa"
        })
    })
   
     //var page = new Vue({
     //       el: "#aaa",
     //       data() {
     //           isOpen: false
     //       },
     //       params: { isOpen: false },
     //       props: { isOpen }
     //   })
    //export default {   
    //    data() {
    //        return {
    //            isOpen: false,
    //            page: function () {
    //                new Vue({
    //                    el: "#aaa",
    //                    data() {
    //                        isOpen: false
    //                    },
    //                    params: { isOpen: false }
    //                })
    //            }
    //        }
    //    }
    //    //Vue.component('app-header', {
    //    //    render: h => h(require('./header/header.vue.html'))
    //    //});
    //    //var page = new Vue({
    //    //    el: "#aaa",
    //    //    data() {
    //    //        isOpen: false
    //    //    },
    //    //    params: { isOpen: false },
    //    //    props: { isOpen }
    //    //})
    //}
</script>-->
<!--<style src="./sections.less" lang="less"></style>-->

