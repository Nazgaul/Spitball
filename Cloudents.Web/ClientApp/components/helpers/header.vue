<template>
    <div>
        <!--TODO check if worsk well-->
        <v-toolbar :app="!isMobile" :fixed="!isMobile" :height="height" class="header">
            <v-layout column :class="layoutClass?layoutClass:'header-elements'" class="mx-0">
                <div class="main">
                    <v-flex class="line top">
                        <v-layout row>
                            <v-toolbar-title>
                                 <a @click="resetItems()" class="logo-link">
                                    <app-logo class="logo"></app-logo>
                                </a> 
                            </v-toolbar-title>
                            <v-toolbar-items>
                                <search-input v-if="$vuetify.breakpoint.smAndUp" :user-text="userText"
                                              :placeholder="this.$options.placeholders[currentSelection]"
                                              :submit-route="submitRoute"></search-input>
                                <v-spacer v-if="$vuetify.breakpoint.xsOnly"></v-spacer>
                                <div class="settings-wrapper d-flex align-center">
                                    <!--TODO AB-Test-->
                                    <!--<div class="ab-test static-card-what-is-hw-question hidden-sm-and-down" v-show="loggedIn && $route.path.slice(1) === 'ask'">-->
                                    <div class="ab-test static-card-what-is-hw-question hidden-sm-and-down">
                                    <button class="ab-test-button" @click="openNewQuestionDialog()">
                                        <v-icon class="edit-icon">sbf-edit-icon</v-icon>
                                        <span v-language:inner>abTest_addQuestion</span>
                                    </button>
                                    </div>
                                    <router-link to="/wallet" class="header-wallet" v-if="loggedIn">
                                        <span class="bold" style="direction:ltr;">{{accountUser.balance | currencyLocalyFilter}}</span>
                                        <span>${{accountUser.balance | dollarVal}}</span>
                                    </router-link>
                                    <div class="header-rocket" v-if="loggedIn">
                                        <v-menu bottom left offset-y>
                                            <user-avatar slot="activator" @click.native="drawer = !drawer" size="32"
                                                         :user-name="accountUser.name"/>

                                            <menu-list :isAuthUser="loggedIn"
                                                       v-if=!$vuetify.breakpoint.xsOnly></menu-list>
                                        </v-menu>
                                        <span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span>
                                    </div>

                                    <router-link v-if="!loggedIn" class="header-login body-1" :to="{ path: '/register', query:{returnUrl : $route.path}  }" v-language:inner>header_sign_up</router-link>
                                    <router-link v-if="!loggedIn" class="header-login body-1" :to="{ path: '/signin'}" v-language:inner>header_login</router-link>

                                    <v-menu bottom left offset-y class="gamburger"
                                            v-if="!loggedIn">
                                        <v-btn :ripple="false" icon slot="activator" @click.native="drawer = !drawer">
                                            <v-icon>sbf-menu</v-icon>
                                        </v-btn>
                                        <menu-list :isAuthUser="loggedIn"
                                                   v-if="$vuetify.breakpoint.smAndUp"></menu-list>
                                    </v-menu>
                                    <!--<v-menu bottom left offset-y class="gamburger"-->
                                            <!--v-if="!loggedIn && $vuetify.breakpoint.xsOnly">-->
                                        <!--<v-btn icon slot="activator" @click.native="drawer = !drawer">-->
                                            <!--<v-icon>sbf-menu</v-icon>-->
                                        <!--</v-btn>-->
                                        <!--<menu-list :isAuthUser="loggedIn"-->
                                                   <!--v-if="$vuetify.breakpoint.smAndUp"></menu-list>-->
                                    <!--</v-menu>-->

                                </div>
                            </v-toolbar-items>
                        </v-layout>
                    </v-flex>
                    <v-flex v-if="$vuetify.breakpoint.xsOnly" class="line search-wrapper">
                        <search-input :user-text="userText" :placeholder="this.$options.placeholders[currentSelection]"
                                      :submit-route="submitRoute"></search-input>
                    </v-flex>
                </div>
                <slot name="extraHeader"></slot>
            </v-layout>
            <v-snackbar absolute :timeout="toasterTimeout" :value="getShowToaster">
                <div class="text-wrap" v-html="getToasterText"></div>
            </v-snackbar>
            <personalize-dialog ref="personalize" :value="clickOnce"></personalize-dialog>
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
    import {settingMenu, notRegMenu} from '../settings/consts';
    import SearchInput from '../helpers/searchInput.vue';
    import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import menuList from "./menu-list/menu-list.vue";
    import {mapActions, mapGetters} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";

    const PersonalizeDialog = () => import('./ResultPersonalize.vue');
    import {LanguageService } from "../../services/language/languageService";

    export default {
        components: {
            PersonalizeDialog,
            AppLogo,
            SearchInput,
            UserAvatar,
            menuList,
        },
        placeholders: {
            job: LanguageService.getValueByKey("header_placeholder_job"),
            tutor: LanguageService.getValueByKey("header_placeholder_tutor"),
            note: LanguageService.getValueByKey("header_placeholder_note"),
            book: LanguageService.getValueByKey("header_placeholder_book"),
            ask: LanguageService.getValueByKey("header_placeholder_ask"),
            flashcard: LanguageService.getValueByKey("header_placeholder_flashcard"),
        },
        data() {
            return {
                settingMenu,
                notRegMenu,
                clickOnce: false,
                drawer: null,
                toasterTimeout: 5000,
                showDialogLogin: false,
                isRtl: global.isRtl
            }
        },
        props: {
            currentSelection: {
            type: String,
            default: 'ask'
            },
            userText: {type: String},
            submitRoute: {type: String, default: '/ask'},
            toolbarHeight: {},
            layoutClass: {}
        },
        computed: {
            ...mapGetters(['getUniversityName', 'accountUser', 'unreadMessages', 'getShowToaster', 'getToasterText']),

            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            loggedIn() {
                return this.accountUser !== null
            },
            //myMoney(){return this.accountUser.balance / 40}

        },
        watch: {
            toolbarHeight(val) {
                this.height = val;
            },
            getShowToaster: function (val) {
                if (val) {
                    var self = this;
                    setTimeout(function () {
                        self.updateToasterParams({
                            showToaster: false
                        })
                    }, this.toasterTimeout)
                }
            }

        },
        methods: {
            ...mapActions(['updateToasterParams', 'resetData', 'updateNewQuestionDialogState']),

            openNewQuestionDialog(){
                let Obj = {
                    status:true,
                    from: 1
                }
                this.updateNewQuestionDialogState(Obj)
            },
            //TODO: what is that
            $_currentClick({id, name}) {
                if (name === 'Feedback') {
                    Intercom('showNewMessage', '');
                } else {
                    this.clickOnce = true;
                    this.$nextTick(() => {
                        this.$refs.personalize.openDialog(id);
                    })
                }
            },

            resetItems(){
                if(this.$route.path === '/ask'){
                    if(this.$route.fullPath === '/ask'){
                        global.location.reload();
                    }else{
                        this.resetData();
                        this.$router.push('/');
                    }
                }else{
                    this.resetData();
                    this.$router.push('/');
                }
            }
        },
        created() {
            this.$root.$on("personalize",
                (type) => {
                    this.clickOnce = true;
                    this.$nextTick(() => {
                        if (this.$refs.personalize) {
                            this.$refs.personalize.openDialog(type);
                        }
                    })
                });
            let headerHeight = this.toolbarHeight ? this.toolbarHeight : (this.$vuetify.breakpoint.smAndUp ? 60 : 115)
            this.height = headerHeight;
        },

    }
</script>
<style src="./header.less" lang="less"></style>
