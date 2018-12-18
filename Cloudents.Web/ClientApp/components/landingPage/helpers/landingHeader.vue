<template>
    <div :class="['landing-header-wrap', isSolidHeader ? 'solid-header' : '']">
        <v-toolbar  flat class="landing-toolbar" v-if="$vuetify.breakpoint.smAndUp">
            <v-toolbar-title clas="landing-header-logo">
                <footer-logo></footer-logo>
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items class="landing-header-items">
                <a class="white-text lp-header-link mr-3">Learn faster</a>
                <a class="yellow-text lp-header-link bold mr-3">Earn money</a>
                <router-link :to="{path: '/signin'}" class="login-action login mr-3">Login</router-link>
                <router-link :to="{path: '/register'}" class="login-action signup mr-3">Sign Up</router-link>
                <v-menu close-on-content-click bottom left offset-y :content-class="'fixed-content'"
                        v-if="!loggedIn">
                    <v-btn :ripple="false" icon slot="activator" @click.native="drawer = !drawer" class="gamburger">
                        <v-icon>sbf-menu</v-icon>
                    </v-btn>
                    <menu-list :isAuthUser="loggedIn"></menu-list>
                </v-menu>
            </v-toolbar-items>
        </v-toolbar>
        <v-toolbar  flat class="mobile-landing-toolbar" v-else>
            <v-toolbar-title clas="landing-header-logo">
                SB
                <!--<footer-logo></footer-logo>-->
            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items class="landing-header-items">
                <a  class="login-action login mr-3" @click.native="goToLogin">Login</a>
                <a  class="login-action signup mr-3"  @click.native="goToRegister">Sign Up</a>
                <v-menu close-on-content-click bottom left offset-y :content-class="'fixed-content'"
                        v-if="!loggedIn">
                    <v-btn :ripple="false" icon slot="activator" @click.native="drawer = !drawer" class="gamburger">
                        <v-icon>sbf-menu</v-icon>
                    </v-btn>
                    <menu-list :isAuthUser="loggedIn"></menu-list>
                </v-menu>
            </v-toolbar-items>

        </v-toolbar>

        <v-navigation-drawer temporary v-model="drawer" light :right="!isRtl"
                             fixed app v-if=$vuetify.breakpoint.xsOnly
                             :class="isRtl ? 'hebrew-drawer' : ''"
                             width="280">
            <menu-list :isAuthUser="loggedIn"></menu-list>
        </v-navigation-drawer>
    </div>
</template>
<script>
    import { mapGetters } from 'vuex';
    import footerLogo from '../images/footerLogo.svg';
    import menuList from "../../helpers/menu-list/menu-list.vue";
    export default {
        name: "landingHeader",
        components: {
            footerLogo,
            menuList
        },
        data() {
            return {
                drawer: false,
                offset: 0,
                isRtl: global.isRtl
            }
        },
        props: {},
        computed: {
            ...mapGetters([
                "accounUser",
            ]),
            loggedIn() {
                return this.accountUser
            },
            isSolidHeader(){
                return this.offset > 120
            }

        },
        methods: {
            goToLogin() {
                console.log( 'login sdfsdf')
                this.$router.push({path: '/signin'});
            },
            goToRegister() {
                console.log( 'register sdfsdf')

                this.$router.push({path: '/register'});
            },
            calculateOffSet() {
                this.offset = window.pageYOffset || document.documentElement.scrollTop;
            },
        },
        beforeMount: function () {
            if (window) {
                window.addEventListener('scroll', this.calculateOffSet)
            }
        },
        beforeDestroy: function () {
            if (window) {
                window.removeEventListener('scroll', this.calculateOffSet)
            }
        }
    }
</script>

<style scoped lang="less">
    @import "../../../styles/mixin.less";

    @yellowColor: #ffdf54;
    .yellow-text {
        color: @yellowColor;
    }
    .white-text {
        color: @color-white;
    }
    .v-navigation-drawer {
        &.hebrew-drawer{
            // swap of right and left is going to be done by webpack RTL, so real vals are oposite
            right: 0;
            left: unset;
        }
    }

    .mobile-landing-toolbar{
        width: 100%;
    }

    .landing-header-wrap {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        display: flex;
        flex-direction: row;
        align-items: center;
        justify-content: center;
        padding-top: 24px;
        &.solid-header{
            background-color: rgba(0, 0, 0, 0.3);
            box-shadow: 0 1px 2px 0 rgba(0,0,0,.2);
            z-index: 99;
        }
        .landing-toolbar {
            width: 80%;
            background: transparent;
            .landing-header-logo {
                height: 36px;
                max-height: 36px;
                svg {

                }
            }
            .landing-header-items {
                display: flex;
                flex-direction: row;
                align-items: center;
                .login-action {
                    display: flex;
                    flex-direction: row;
                    border-radius: 56.5px;
                    font-size: 16px;
                    font-family: @fontFiraSans;
                    padding: 7px 20px;
                    &.login {
                        background: transparent;
                        color: @color-white;
                        border: 1px solid #787878;
                    }
                    &.signup {
                        background: @yellowColor;
                        color: @textColor;
                        font-weight: 500;
                    }
                }
                .lp-header-link {
                    font-size: 16px;
                    &.bold {
                        font-weight: 600;
                    }
                }
                .gamburger {
                    color: @color-white;
                }

            }
        }
    }

</style>