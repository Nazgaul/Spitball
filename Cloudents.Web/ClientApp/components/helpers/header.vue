<template>
    <div :class="{'hide-header': hideHeader}">
        <!--TODO check if worsk well-->
        <v-toolbar :app="!isMobile" :fixed="!isMobile" :height="height" class="header" clipped-left clipped-right>
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
                                              :placeholder="this.$options.placeholders['all']"
                                              :submit-route="submitRoute"></search-input>
                                <v-spacer ></v-spacer>
                                <div class="settings-wrapper d-flex align-center">
                                    <!--TODO HIDDEN FOR NOW-->
                                    <div class="header-messages" v-if="loggedIn && !isMobile">
                                        <span @click="openChatWindow" class="header-messages-text" v-language:inner>chat_messages</span>
                                        <v-icon @click="openChatWindow">sbf-forum-icon</v-icon>
                                        <span @click="openChatWindow" class="unread-circle" v-show="totalUnread > 0" :class="[totalUnread > 9 ? 'longer' :'']">{{totalUnread}}</span>
                                    </div>
                                    <div class="header-wallet" v-if="loggedIn">
                                        <button class="setting-buysbl-button" @click="openSblToken()"><span v-language:inner>buyTokens_buy_points_button</span></button>     
                                        <span class="header-wallet-text">{{balance | currencyLocalyFilter}}</span>                                        
                                    </div>
                                    <div class="header-rocket" v-if="loggedIn">
                                        <v-menu close-on-content-click bottom left offset-y :content-class="'fixed-content'">
                                            <user-avatar slot="activator" @click.native="drawer = !drawer" size="32"
                                                         :userImageUrl="userImageUrl" :user-name="accountUser.name"/>

                                            <menu-list :isAuthUser="loggedIn" v-if=!$vuetify.breakpoint.xsOnly></menu-list>
                                        </v-menu>
                                        <span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span>
                                    </div>
                                    

                                    <router-link v-if="!loggedIn" class="header-login" :to="{ path: '/register', query:{returnUrl : $route.path}  }" v-language:inner>header_sign_up</router-link>
                                    <router-link v-if="!loggedIn" class="header-login" :to="{ path: '/signin', query:{returnUrl : $route.path} }" v-language:inner>header_login</router-link>

                                    <v-menu close-on-content-click bottom left offset-y :content-class="'fixed-content'" class="gamburger"
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
                        <search-input :user-text="userText" :placeholder="this.$options.placeholders['all']"
                                      :submit-route="submitRoute"></search-input>
                    </v-flex>
                    <slot name="extraHeader"></slot>
                </div>
                
            </v-layout>            
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
    import {notRegMenu} from '../settings/consts';
    import SearchInput from '../helpers/searchInput/searchInput.vue';
    import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import menuList from "./menu-list/menu-list.vue";
    import {mapActions, mapGetters, mapMutations} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";

    import {LanguageService } from "../../services/language/languageService";
    import analyticsService from "../../services/analytics.service";

    export default {
        components: {
            AppLogo,
            SearchInput,
            UserAvatar,
            menuList,
        },
        placeholders: {
            all: LanguageService.getValueByKey("header_Search"),
            job: LanguageService.getValueByKey("header_placeholder_job"),
            tutor: LanguageService.getValueByKey("header_placeholder_tutor"),
            note: LanguageService.getValueByKey("header_placeholder_note"),
            book: LanguageService.getValueByKey("header_placeholder_book"),
            ask: LanguageService.getValueByKey("header_placeholder_ask"),
            flashcard: LanguageService.getValueByKey("header_placeholder_flashcard"),
        },
        data() {
            return {
                notRegMenu,
                clickOnce: false,
                drawer: false,
                showDialogLogin: false,
                isRtl: global.isRtl
            }
        },
        props: {
            currentSelection: {
                type: String,
                default: 'all'
            },
            userText: {type: String},
            submitRoute: {type: String, default: '/ask'},
            toolbarHeight: {},
            layoutClass: {}
        },
        computed: {
            ...mapGetters([
                'accountUser',
                'unreadMessages',
                'showMobileFeed',
                'getTotalUnread'
            ]),
            userImageUrl(){
                if(this.accountUser.image.length > 1){
                    return `${this.accountUser.image}`
                }
                return ''

            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            loggedIn() {
                return this.accountUser !== null
            },
            hideHeader(){
                if(this.$vuetify.breakpoint.xsOnly){
                   let matchedCoursesRoute = this.$route.name === 'courses' || this.$route.name === 'addCourse' || this.$route.name === 'editCourse';
                    let matchedUniRoute = this.$route.name === 'university' || this.$route.name === 'addUniversity';
                    return  matchedCoursesRoute || matchedUniRoute || !this.showMobileFeed;;
                }else{
                    return false;
                }
                
            },
            totalUnread(){
                return this.getTotalUnread
            },
            balance(){
                return this.accountUser.balance || 0
            }
            //myMoney(){return this.accountUser.balance / 40}

        },
        watch: {
            toolbarHeight(val) {
                this.height = val;
            },
            drawer(val){
                if(!!val && this.$vuetify.breakpoint.xsOnly){
                    document.getElementsByTagName("body")[0].className="noscroll";
                    console.log("drawer open")
                }else{
                    document.body.removeAttribute("class","noscroll");
                    console.log("drawer closed")
                }
            }

        },
        methods: {
            ...mapActions(['updateToasterParams', 'updateNewQuestionDialogState', 'updateLoginDialogState', 'updateUserProfileData', 'updateShowBuyDialog','openChatInterface']),
               
            ...mapMutations(['UPDATE_SEARCH_LOADING']),
            openNewQuestionDialog(){
                    if(this.accountUser == null){
                        this.updateLoginDialogState(true);
                        //set user profile
                        this.updateUserProfileData('profileHWH')
                    }else{
                        //ab test original do not delete
                        let Obj = {
                            status:true,
                            from: 1
                        };
                        this.updateNewQuestionDialogState(Obj)
                    }
            },
            openChatWindow(){
                this.openChatInterface();
            },
            openSblToken(){
                analyticsService.sb_unitedEvent("BUY_POINTS", "ENTER");
                this.updateShowBuyDialog(true)
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
                this.UPDATE_SEARCH_LOADING(true);
                this.$router.push('/');
            },
            closeDrawer(){
                this.drawer = !this.drawer;
            }
        },
        created() {
            this.$root.$on("closeDrawer", ()=>{
                this.$nextTick(() => {
                    this.closeDrawer();
                })
            })
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
        beforeDestroy(){
             document.body.removeAttribute("class","noscroll");
        }
    }
</script>
<style src="./header.less" lang="less"></style>
