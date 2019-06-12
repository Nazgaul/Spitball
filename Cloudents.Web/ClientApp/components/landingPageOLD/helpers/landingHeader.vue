<template>
    <div :class="['landing-header-wrap', isSolidHeader ? 'solid-header' : '']">
        <v-toolbar flat class="landing-toolbar">
            <v-toolbar-title class="landing-header-logo">
                <short-logo style="width: 45px;" v-if="isMobileView"></short-logo>
                <footer-logo v-else></footer-logo>

            </v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items class="landing-header-items">
                <a :class="{'white-text': dictionaryType === dictionaryTypesEnum.earn, 'yellow-text': dictionaryType === dictionaryTypesEnum.learn}"
                   v-show="!isMobileView" class="lp-header-link mr-4" @click="changeDictionaryType('learn')"
                   v-language:inner>landingPage_header_learn_faster</a>
                <a :class="{'white-text': dictionaryType === dictionaryTypesEnum.learn, 'yellow-text': dictionaryType === dictionaryTypesEnum.earn}"
                   v-show="!isMobileView" class="lp-header-link mr-4" @click="changeDictionaryType('earn')"
                   v-language:inner>landingPage_header_earn_money</a>
                <router-link v-show="!loggedIn" :to="{path: '/signin'}" class="login-action login" v-language:inner>
                    landingPage_header_login
                </router-link>
                <router-link v-show="!loggedIn" :to="{path: '/register'}" class="login-action signup" v-language:inner>
                    landingPage_header_sign_up
                </router-link>
                <v-menu style="margin-top:3px;" close-on-content-click bottom left offset-y
                        :content-class="'fixed-content'">
                    <v-btn :ripple="false" icon slot="activator" @click.native="drawer = !drawer" class="gamburger">
                        <v-icon class="gamburger-icon">sbf-menu</v-icon>
                    </v-btn>
                    <menu-list v-if="!isMobileView" :isAuthUser="loggedIn"></menu-list>
                </v-menu>
            </v-toolbar-items>
        </v-toolbar>

        <v-navigation-drawer temporary v-model="drawer" light :right="!isRtl"
                             fixed app v-if="isMobileView"
                             :class="isRtl ? 'hebrew-drawer' : ''"
                             width="280">
            <menu-list :isAuthUser="loggedIn"></menu-list>
        </v-navigation-drawer>
    </div>
</template>
<script>
    import { mapGetters, mapActions } from 'vuex';
    import footerLogo from '../images/footerLogo.svg';
    import shortLogo from '../images/sb-logo-short.svg';
    import menuList from "../../helpers/menu-list/menu-list.vue";

    export default {
        name: "landingHeader",
        components: {
            footerLogo,
            shortLogo,
            menuList
        },
        data() {
            return {
                drawer: false,
                offset: 0,
                isRtl: global.isRtl,
                dictionaryTypesEnum: this.getDictionaryPrefixEnum()
            }
        },
        props: {},
        computed: {
            ...mapGetters([
                "accountUser",
                "getDictionaryPrefix"
            ]),
            dictionaryType() {
                return this.getDictionaryPrefix
            },
            loggedIn() {
                return !!this.accountUser
            },
            isSolidHeader() {
                return this.$vuetify.breakpoint.xsOnly ? this.offset > 45 : this.offset > 120;
            },
            isMobileView() {
                return this.$vuetify.breakpoint.width < 1024;
            }

        },
        methods: {
            ...mapActions([
                "switchLandingPageText"
            ]),
            ...mapGetters(["getDictionaryPrefixEnum"]),
            changeDictionaryType(val) {
                // this.scrollTop();
                // this.switchLandingPageText(val);
                if(val === 'earn'){
                    let typeObj = {
                    type: val
                }
                this.$router.push({query: typeObj})
                }else{
                    this.$router.push({query: ``})
                }
            },
            scrollTop(){
                setTimeout(()=>{
                this.$nextTick(() => {
                    global.scrollTo(0, 0);
                })
            }, 200)
            },
            goToLogin() {
                this.$router.push({path: '/signin'});
            },
            goToRegister() {
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

    .yellow-text {
        color: @color-yellow;
    }

    .white-text {
        color: @color-white;
    }

    .v-navigation-drawer {
        &.hebrew-drawer {
            // swap of right and left is going to be done by webpack RTL, so real vals are oposite
            right: 0;
            left: unset;
        }
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
        @media (max-width: @screen-mds) {
            padding-top: 8px;
            z-index: 100;
        }
        &.solid-header {
            background-color: fade(#29313F, 85%);
            box-shadow: 0 1px 2px 0 rgba(0, 0, 0, .2);
            z-index: 99;
        }
        .landing-toolbar {
            width: 80%;
            @media (max-width: @screen-mds) {
                width: 100%;
            }
            background: transparent;
            .landing-header-logo {
                height: 32px;
                max-height: 32px;
            }

            .landing-header-items {
                display: flex;
                flex-direction: row;
                align-items: center;
                a {
                    margin-right: 12px;
                    @media (max-width: @screen-mds) {
                        margin-right: 8px;
                    }
                }

                .login-action {
                    display: flex;
                    flex-direction: row;
                    border-radius: 56.5px;
                    font-size: 16px;
                    font-family: @fontFiraSans;
                    padding: 7px 20px;
                    white-space: nowrap;
                    min-width: 73px;
                    @media (max-width: @screen-mds) {
                        padding-top: 6px;
                        font-size: 13px;
                    }
                    &.login {
                        background: transparent;
                        color: @color-white;
                        border: 1px solid rgba(187, 187, 187, 1);
                    }
                    &.signup {
                        background: @color-yellow;
                        color: @textColor;
                        font-weight: 600;
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

                    i {
                        font-size: 14px;
                        @media (max-width: @screen-mds) {
                            font-size: 16px;
                        }
                    }

                }

            }
        }
    }

</style>