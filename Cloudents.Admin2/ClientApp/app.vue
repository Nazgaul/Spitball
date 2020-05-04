<template>
    <v-app>
        <navigation class="navigation-wrap"></navigation>
        <v-content fluid fill-height>
            <router-view class="router-view-wrap"></router-view>
        </v-content>
        <componentInjection />
    </v-app>

</template>

<script>
    import navigation from "./components/navigation/navigation.vue"
    import componentInjection from "./components/componentInjection/componentInjection.vue"

    export default {
        components: { navigation, componentInjection }
    }
</script>
<style lang="less">
    body {
         font-family: 'Roboto', Helvetica, Arial, sans-serif;
    }
    </style>
    <!-- <style lang="less" >
    html, body {
        //breaks scroll, cannot listen scroll if parent is scrolling
        /*height: 100%;*/
        margin: 0;
        padding: 0;
        /*background-color: #e0e0e0;*/
    }

    body {
        flex-direction: column;
        display: flex;
    }


    #template-app {
        font-family: 'Roboto', Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
        color: rgba(0, 0, 0, 0.87);
        display: flex;
        height: 100%;
        display: flex;
        justify-content: space-between;
        overflow: auto;
        .nav-container

    {
        min-width: 200px;
    }

    }

    .router-view-wrap {
        display: flex;
        flex-grow: 1;
        justify-content: center;
        align-items: flex-start;
        order: 1;
        background-color: #ffffff;
    }

    .navigation-wrap {
        min-height: 100%;
        flex-basis: 15%;
        order: -1;
    }

    #nav {
        position: fixed;
        height: 100%;
        padding: 2px;
        flex-grow: 1;
        border-right: 2px solid #7c7979;
        width: 200px;
        max-width: 200px;
        min-width: 200px;
        top: 0;
        border-radius: 25px;
        a

    {
        font-weight: bold;
        color: #5d5d5d;
        display: block;
        height: 45px;
        line-height: 45px;
        width: 100%;
        margin-bottom: 2px;
        text-decoration: none;
        border-radius: 25px;
        &:hover

    {
        background-color: #7c7979;
    }

    }

    a.router-link-active {
        color: #97ed82;
        background-color: #7c7979;
    }

    }

    #app-body {
        flex-grow: 1;
        /*padding: 2px;*/
        display: flex;
        max-width: 1890px;
        width: auto;
        justify-content: space-between;
        flex-direction: row;
    }
</style>-->
