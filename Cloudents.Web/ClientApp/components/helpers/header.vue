<template>
    <div :class="{'hide-header': hideHeader}">
        <!--TODO check if worsk well-->
        <v-toolbar :app="!isMobile" :fixed="!isMobile" class="header" clipped-left clipped-right>
            <v-layout column :class="layoutClass?layoutClass:'header-elements'" class="mx-0">
                <div class="main">
                    <v-flex class="line top">
                        <v-layout row>
                            <v-toolbar-title>
                                <router-link @click.prevent="resetItems()" to="/" class="logo-link">
                                    <logoComponent></logoComponent>
                                </router-link>
                            </v-toolbar-title>
                            <v-toolbar-items>
                                <search-input 
                                    v-if="$vuetify.breakpoint.smAndUp && !hideSearch" 
                                    :user-text="userText"
                                    :placeholder="placeholder"
                                    :submit-route="submitRoute">
                                </search-input>
                                <v-spacer ></v-spacer>
                                <div class="settings-wrapper d-flex align-center">
                                    <!--TODO HIDDEN FOR NOW-->
                                    <div class="header-messages" v-if="loggedIn && !isMobile && hasConversations">
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
                                            <user-avatar 
                                                slot="activator" 
                                                @click.native="drawer = !drawer" 
                                                size="32"
                                                :userImageUrl="userImageUrl" 
                                                :user-name="accountUser.name"
                                            />
                                            <menu-list :isAuthUser="loggedIn" v-if=!$vuetify.breakpoint.xsOnly></menu-list>
                                        </v-menu>
                                        <span class="red-counter" v-if="unreadMessages">{{unreadMessages}}</span>
                                    </div>
                                    

                                    <router-link rel="nofollow" v-if="!loggedIn" class="header-login" :to="{ path: '/register', query:{returnUrl : $route.path}  }" v-language:inner>header_sign_up</router-link>
                                    <router-link rel="nofollow" v-if="!loggedIn" class="header-login" :to="{ path: '/signin', query:{returnUrl : $route.path} }" v-language:inner>header_login</router-link>

                                    <v-menu close-on-content-click bottom left offset-y :content-class="'fixed-content'" class="gamburger"
                                            v-if="!loggedIn">
                                        <v-btn :ripple="false" icon slot="activator" @click.native="drawer = !drawer">
                                            <v-icon>sbf-menu</v-icon>
                                        </v-btn>
                                        <menu-list :isAuthUser="loggedIn"
                                                   v-if="$vuetify.breakpoint.smAndUp"></menu-list>
                                    </v-menu>
                                   
                                </div>
                            </v-toolbar-items>
                        </v-layout>
                    </v-flex>
                    <v-flex v-if="$vuetify.breakpoint.xsOnly && !hideSearch" class="line search-wrapper">
                        <search-input :user-text="userText" :placeholder="placeholder"
                                      :submit-route="submitRoute"></search-input>
                    </v-flex>
                    <slot name="extraHeader"></slot>
                </div>
                
            </v-layout>            
        </v-toolbar>

        <v-navigation-drawer temporary v-model="drawer" light :right="!isRtl"
                             fixed app v-if="$vuetify.breakpoint.xsOnly"
                             :class="isRtl ? 'hebrew-drawer' : ''"
                             width="280">
            <menu-list :isAuthUser="loggedIn"></menu-list>
        </v-navigation-drawer>
    </div>
</template>

<script>
    // Store
    import {mapActions, mapGetters, mapMutations} from 'vuex';

    // Consts
    import {notRegMenu} from '../settings/consts';

    // Components
    import SearchInput from '../helpers/searchInput/searchInput.vue';
    import UserAvatar from '../helpers/UserAvatar/UserAvatar.vue';
    import menuList from "./menu-list/menu-list.vue";
    import logoComponent from '../app/logo/logo.vue';

    // Services
    import {LanguageService } from "../../services/language/languageService";
    import analyticsService from "../../services/analytics.service";

    export default {
        components: {
            SearchInput,
            UserAvatar,
            menuList,
            logoComponent
        },
        data() {
            return {
                path: '',
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
                default: 'feed'
            },
            userText: {type: String},
            submitRoute: {type: String, default: '/feed'},
            layoutClass: {}
        },
        computed: {
            ...mapGetters([
                'accountUser',
                'unreadMessages',
                'showMobileFeed',
                'getTotalUnread',
                'getConversations',
                'showLeaderBoard',
            ]),
            userImageUrl(){
                if(this.accountUser.image.length > 1){
                    return `${this.accountUser.image}`
                }
                return ''
            },
            hasConversations(){
                return Object.keys(this.getConversations).length > 0;
            },
            isMobile() {
                return this.$vuetify.breakpoint.xsOnly;
            },
            loggedIn() {
                return this.accountUser !== null
            },
            hideHeader(){
                if(this.$vuetify.breakpoint.xsOnly){
                    let routesToHide = ['courses', 'addCourse', 'editCourse', 'university', 'addUniversity'];
                    return routesToHide.indexOf(this.$route.name) > -1;
                }else{
                    return false;
                }
                
            },
            totalUnread(){
                return this.getTotalUnread
            },
            balance(){
                return this.accountUser.balance || 0
            },
            hideSearch(){
                if(this.isMobile && this.showLeaderBoard){
                    return true;
                }
                let filteredRoutes = ['editCourse', 'addCourse', 'document', 'about', 'faq', 'partners', 'reps', 'privacy', 'terms', 'contact', 'profile', 'wallet', 'addUniversity', 'studyRooms'];
                return filteredRoutes.indexOf(this.$route.name) > -1;
            },
            placeholder() {
                return LanguageService.getValueByKey(`header_placeholder_feed`);
                // return LanguageService.getValueByKey(`header_placeholder_${this.currentSelection}`);
                }
        },
        watch: {
            drawer(val){
                if(!!val && this.$vuetify.breakpoint.xsOnly){
                    document.body.className="noscroll";
                }else{
                    document.body.removeAttribute("class","noscroll");
                }
            },
            '$route': function(val){
                if(val.name === 'tutors'){
                    this.path = 'tutor';
                } else{
                    this.path = val.name
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
            if(this.$route.name === 'tutors'){
                this.path = 'tutor';
            } else{
                this.path = this.$route.name
            }

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
        },
        beforeDestroy(){
             document.body.removeAttribute("class","noscroll");
        }
    }
</script>
<style src="./header.less" lang="less"></style>
