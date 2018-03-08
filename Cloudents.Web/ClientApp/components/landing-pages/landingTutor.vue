<template>
    <div class="landing-page-wrapper">
        <div class="landing-page">

            <router-link class="logo-link" :to="{name:'home'}">
                <app-logo class="logo"></app-logo>
            </router-link>
            <div class="page-data">
                <h1>Get<br/>a tutor<br/>now!</h1>
                <div class="content-wrapper">
                    <h2>We teamed up with the most trusted <br class="hidden-xs-only"/>
                        tutoring services out there so you can <br class="hidden-xs-only"/>
                        enjoy it all. In one place.</h2>
                    <p class="hidden-xs-only">Fill out the field and start spitballing!</p>
                    <div class="search-wrapper">
                        <v-text-field class="search-b" type="search" solo
                                      @keyup.enter="search" autocomplete="off"
                                      required name="q"
                                      v-model.trim="msg" placeholder="Enter your subject"></v-text-field>
                        <button @click="search">Get 30 Minutes Free</button>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";

    export default {
        data: () => ({
            msg: ''
        }),
        components: {
            AppLogo
        }
        ,
        methods: {
            search() {
                this.$router.push({path: "/tutor", query: {q: this.msg}});
            }
        }
    }
</script>
<style src="./landingTutor.less" lang="less"></style>

