<template>
    <div class="top-nav-warraper w-nav">
        <div class="container">
            <div class="top-nav-box">
                <router-link class="nav-logo-warraper" to="/">
                    <LogoSvg></LogoSvg>
                </router-link>
                <nav role="navigation" class="top-nav" :class="{'open_nav': navToggle}">
                    <router-link class="nav-link w-nav-link" v-for="(link, index) in links"
                                 :to="`${link.url}`" :key="index">{{link.title}}
                    </router-link>
                    <a href="#" class="nav-link lang-btn w-nav-link">HEB</a>
                </nav>
                <div class="w-nav-button" v-on:click="navToggle = !navToggle" ref="dropdownMenu">
                    <div></div>
                    <div></div>
                    <div></div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import LogoSvg from '../assets/logo.svg';
    export default {
        components:{
            LogoSvg
        },
        name: "TopNav",
        data() {
            return {
                navToggle: false,
                links: [
                    // {title: 'Find a tutor', url: 'find-tutor'},
                    {title: 'How it works', url: 'how-it-works'},
                    {title: 'Become a tutor', url: 'become-tutor'},
                ],
            }
        },
        methods: {
            navToogle(e) {
                let el = this.$refs.dropdownMenu
                let target = e.target
                if (el !== target && !el.contains(target)) {
                    this.navToggle = false
                }
            }
        },
        created() {
            document.addEventListener('click', this.navToogle)
        },
        destroyed() {
            document.removeEventListener('click', this.navToogle)
        }
    }
</script>

<style>
    @media only screen and (max-width: 900px) {

        .top-nav {
            height: 0;
            transition: all .5s ease-in-out;
            display: -webkit-box;
            display: -webkit-flex;
            display: -ms-flexbox;
            display: flex;
            position: absolute;
            left: 0px;
            top: 60px;
            right: 0px;
            overflow: hidden;
            max-height: 400px;
            -webkit-box-orient: vertical;
            -webkit-box-direction: normal;
            -webkit-flex-direction: column;
            -ms-flex-direction: column;
            flex-direction: column;
            -webkit-justify-content: space-around;
            -ms-flex-pack: distribute;
            justify-content: space-around;
            background-color: #fff;
            box-shadow: 0 0 8px 0 rgba(0, 0, 0, .16);
            overflow: hidden;
        }


        .open_nav {
            padding-top: 29px;
            padding-bottom: 38px;
            height: 256px;
        }

        .w-nav-button {
            display: initial;
        }
    }
</style>