<template>
    <div v-if="!isHideHeader">
    <v-app-bar :class="{'homePageWrapper': isHomePage}" class="globalHeader elevation-0" color="white" :height="isMobile? 60 : 70" app fixed clipped-left clipped-right>
        <router-link @click.prevent="resetItems()" to="/" class="globalHeader_logo">
            <logoComponent/>
        </router-link>
        <div class="globalHeader_items">
            <div class="globalHeader_items_left" v-if="!isMobile && showSearch">
                <searchCMP :placeholder="searchPlaceholder"/>
            </div>
            <v-spacer v-else></v-spacer>
            <div class="globalHeader_items_right">
                <router-link v-show="!isMobile && shouldShowFindTutor" :to="{name:'tutorLandingPage'}" class="gH_i_r_findTutor" >
                    <findSVG/>
                    <span v-language:inner="'header_find_tutors'"/>
                </router-link>
                <template v-if="!isMobile" >
                    <v-tooltip bottom>
                        <template v-slot:activator="{on}">
                            <v-icon @click="startIntercom" v-on="on" v-if="!$vuetify.breakpoint.smAndDown" class="gH_i_r_intercom" :class="{'gH_i_r_intercom--margin': !loggedIn}" v-html="'sbf-help'"/>
                        </template>
                        <span v-language:inner="'header_tooltip_help'"/>
                    </v-tooltip>
                    
                    <v-tooltip bottom v-if="loggedIn">
                        <template v-slot:activator="{on}">
                            <div v-on="on" class="gH_i_r_chat">
                                <v-icon class="gH_i_r_chat_i" @click="openChatWindow" v-html="'sbf-forum-icon'"/>
                                <span @click="openChatWindow" class="unread_circle_nav" v-show="totalUnread > 0" :class="[totalUnread > 9 ? 'longer_nav' :'']">{{totalUnread}}</span>
                            </div>
                        </template>
                        <span v-language:inner="'header_tooltip_chat'"/>
                    </v-tooltip>
                </template>
                <template v-if="!$vuetify.breakpoint.smAndDown && !loggedIn">
                    <button class="gH_i_r_btns gH_i_r_btn_in mr-2" @click="$router.push({path:'/signin'})" v-language:inner="'tutorListLanding_topnav_btn_login'"/>
                    <button class="gH_i_r_btns gH_i_r_btn_up mr-4" @click="$router.push({path:'/register'})" v-language:inner="'tutorListLanding_topnav_btn_signup'"/>
                    <a class="gH_i_lang" @click="changeLanguage()" v-if="!isFrymo && isHomePage" sel="language" v-html="currLanguage !== languageChoisesAval.id? languageChoisesAval.title : ''"/>
                </template>
                <v-menu fixed close-on-content-click bottom right offset-y :content-class="getBannerSatus? 'fixed-content-banner':'fixed-content'" sel="menu">
                    <template v-slot:activator="{on}">
                        <div v-on="on" class="gH_i_r_menuList" >
                            <div @click.prevent="drawer=!drawer">
                                <user-avatar
                                    size="40"
                                    :userImageUrl="userImageUrl"
                                    :user-name="userName"
                                />
                            </div>
                            <template v-if="loggedIn">
                                <div v-if="!$vuetify.breakpoint.mdAndDown" class="gh_i_r_userInfo text-truncate" @click.prevent="drawer=!drawer">
                                    <span class="ur_greets" v-html="$Ph('header_greets', accountUser.name)"/>
                                    <div class="ur_balance">
                                        <span v-html="$Ph('header_balance', userBalance(accountUser.balance))"/>
                                        <v-icon v-if="!isMobile" class="ur_balance_drawer ml-2" color="#43425d" v-html="'sbf-arrow-fill'"/>
                                    </div>
                                </div>
                            </template>
                            <template>
                                <v-btn :class="[{'hidden-md-and-up': isHomePage},{'d-none':!isHomePage && loggedIn}]" :ripple="false" icon @click.native="drawer = !drawer">
                                    <v-icon small v-html="'sbf-menu'"/>
                                </v-btn>
                            </template>
                        </div>
                    </template>
                    <menuList v-if="!$vuetify.breakpoint.xsOnly"/>
                </v-menu>
            </div>
        </div>
        <template v-slot:extension v-if="isMobile && showSearch">
            <div class="mobileHeaderSearch">
                <searchCMP :placeholder="searchPlaceholder"/>
            </div>
        </template>
    </v-app-bar>
            <v-navigation-drawer temporary v-model="drawer" light :right="isMobile ? !isRtl : false"
                             fixed app v-if="$vuetify.breakpoint.xsOnly" class="drawerIndex"
                             width="280">
            <menuList @closeMenu="closeDrawer"/>
        </v-navigation-drawer>
    </div>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from 'vuex';
import {LanguageChange, LanguageService } from "../../../../services/language/languageService";
import languagesLocales from "../../../../services/language/localeLanguage";

import searchCMP from '../../global/search/search.vue';
import UserAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
import menuList from '../menuList/menuList.vue';

import logoComponent from '../../../app/logo/logo.vue';
import findSVG from './images/findSVG.svg'

export default {
    components: {searchCMP,UserAvatar,menuList,logoComponent,findSVG},
    data() {
        return {
            drawer: false,
            currentRoute: this.$route.name,
            languageChoisesAval: [],
            currLanguage: document.documentElement.lang,
            clickOnce: false,
            isRtl: global.isRtl
        }
    },
    props: {
        layoutClass: {}
    },
    computed: {
        ...mapGetters(['accountUser','getTotalUnread','isFrymo','getBannerSatus']),
        isTablet(){
            return this.$vuetify.breakpoint.smAndDown;
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl(){
            return this.accountUser && this.accountUser.image.length > 1 ? this.accountUser.image : '';
        },
        userName(){
            return this.accountUser && this.accountUser.name ? this.accountUser.name : '';
        },
        loggedIn() {
            return this.accountUser !== null;
        },
        totalUnread(){
            return this.getTotalUnread;
        },
        isHideHeader(){
            let filteredRoutes = ['profile'];
            return filteredRoutes.indexOf(this.$route.name) > -1 && this.$vuetify.breakpoint.xsOnly;
        },
        searchPlaceholder(){
            return this.isTablet ? LanguageService.getValueByKey(`header_placeholder_search`) : LanguageService.getValueByKey(`header_placeholder_search_m`);
        },
        showSearch(){
            let showRoutes = ['feed'];
            return showRoutes.includes(this.currentRoute)
        },
        isHomePage(){
            return this.currentRoute === undefined;
        },
        shouldShowFindTutor(){ 
            let hiddenRoutes = ['tutorLandingPage']
            return !hiddenRoutes.includes(this.currentRoute)
        },
    },
    watch: {
    '$route'(){
      this.$nextTick(()=>{
          console.log(this.$route.name)
            this.drawer = false;
            this.currentRoute = this.$route.name
      })
    },
    },
    methods: {
        ...mapActions(['openChatInterface']),
        ...mapMutations(['UPDATE_SEARCH_LOADING']),
        openChatWindow(){
            this.openChatInterface();
        },
        resetItems(){
            this.UPDATE_SEARCH_LOADING(true);
            this.$router.push('/');
        },
        closeDrawer() {
            this.drawer = !this.drawer;
        },       
        startIntercom() {
            if(this.isFrymo){
                window.open('mailto: support@frymo.com', '_blank');
            }else{
                Intercom("showNewMessage");
            }
        },  
        userBalance(balance){
            let balanceFixed = +balance.toFixed()
            return balanceFixed.toLocaleString(`${global.lang}`)
        },
        changeLanguage() {
        LanguageChange.setUserLanguage(this.languageChoisesAval.id).then(
            resp => {
            console.log("language responce success", resp);
            global.location.reload(true);
            },
            error => {
            console.log("language error error", error);
            }
        );
        },
    },
    created() {
        this.$root.$on("closeDrawer", ()=>{
            this.$nextTick(() => {
                this.closeDrawer();
            })
        })
        let currentLocHTML = document.documentElement.lang;
        this.languageChoisesAval = languagesLocales.filter(lan => {
            return lan.locale !== currentLocHTML;
        })[0];
    },
    beforeDestroy(){
            document.body.removeAttribute("class","noscroll");
    }
}
</script>
<style src="./header.less" lang="less"></style>
<style lang="less">
@import '../../../../styles/mixin.less';
.globalHeader{
    z-index: 200 !important;
    .v-toolbar__extension{
        @media (max-width: @screen-xs) {
            padding: 0 8px
    }
    }
    .v-toolbar__content{
        border-bottom: solid 1px #dadada;
        padding: 0 24px 0 16px;
        @media (max-width: @screen-xs) {
            padding: 0 8px 0 4px; 
        }  
    }
    .globalHeader_logo{
        width: 20%;
        @media (min-width: @screen-md) {
            width: calc(~"276px - 16px");
            flex-grow: 0;
            flex-shrink: 0;
            margin-right: 34px;
        }
        
    }    
    .mobileHeaderSearch{
        width: 100%;
        height: 40px;
        border: solid 1px #c1c3ce;
        border-radius: 8px;
        margin-bottom: 10px;
        .searchCMP{
            border-radius: 7px;
            ::placeholder {
                color: #6a697f !important;
                font-stretch: normal;
                font-style: normal;
                letter-spacing: normal;
                font-size: 14px;
            }
            .v-text-field__slot{
                color: #6a697f !important;
                // input{

                // }
            }
            .v-text-field{
                input{
                    max-height: initial;
                    padding: initial;
                }
            }
            .v-input__slot{
                padding-left: 4px;
            }
            .v-input__icon{
                i{
                    color: #43425d !important;
                }
            } 
            .searchCMP-btn{
                max-width: 72px;
                font-size: 14px;
            }
            .searchCMP-input{
                .v-text-field__slot{
                    line-height: 16px;
                    align-items: normal;
                }

            } 
        }
    }
    .globalHeader_items{
        width: 100%;
        display: flex;
        align-items: center;
        justify-content: space-between;
        @media (max-width: @screen-mds) {
            margin-left: 32px; 
        }
        .globalHeader_items_left{
            width: 100%;
            max-width: 564px;
            height: 38px;
            border: solid 1px #c1c3ce;
            border-radius: 8px;
            margin-right: 18px;
            .searchCMP{
                border-radius: 7px;
                .v-input__icon{
                    i{
                        color: #43425d !important;
                    }
                } 
                .searchCMP-btn{
                    max-width: 72px;
                    font-size: 14px;
                }
                .v-input__slot{
                    padding-left: 8px;
                }
                ::placeholder {
                    color: #6a697f !important;
                    font-weight: normal;
                    font-stretch: normal;
                    font-style: normal;
                    letter-spacing: normal;
                    font-size: 14px;
                }
                .v-text-field{
                    input{
                        padding: initial;
                    }
                }
                .v-text-field__slot{
                    color: #6a697f !important;
                    font-size: 14px;
                }
            .searchCMP-input{
                .v-text-field__slot{
                    line-height: 18px;
                    //margin-bottom: 2px;
                    // height: 18px;
                    //align-items: normal;
                }

            } 
            }
        }
        .globalHeader_items_right{
            .flexSameSize();
            display: flex;
            align-items: center;
            .gH_i_lang{
                text-decoration: none;
                font-size: 16px;
                color: #43425d;
                font-weight: bold;
                margin-bottom: 1px;
            }
            .gH_i_r_btns{
                text-align: center;
                border-radius: 6px;
                font-size: 14px;
                outline: none;
                &.gH_i_r_btn_up {
                    padding: 9px 14px;
                    margin: 8px;
                    background-color: #4c59ff;
                    color: white;
                }
                &.gH_i_r_btn_in {
                    padding: 8px 20px;
                    margin: 5px;
                    border: solid 1px #43425d;
                    color: #43425d;
                    background-color: transparent;
                }
            }
            .gH_i_r_findTutor{
                margin-right: 26px;
                background: #69687d;
                border-radius: 8px;
                padding: 2px 10px 3px 4px;
                svg{
                    fill: white !important;
                    vertical-align: middle;
                }
                span{
                    font-size: 14px;
                    font-weight: normal;
                    color: white;
                    vertical-align: middle;
                    padding-left: 2px;
                }
            }
            .gH_i_r_intercom{
                cursor: pointer;
                color: #bdc0d1;
                font-size: 22px;
                padding-top: 4px;
                margin-right: 26px;

                &--margin {
                    margin-right: 20px;
                    margin-bottom: 1px;
                }
            }
            .gH_i_r_chat{
                position: relative;
                margin-right: 26px;
                .gH_i_r_chat_i{
                    color: #bdc0d1;
                    font-size: 22px;
                    padding-top: 8px;
                }
                .unread_circle_nav{
                    position: absolute;
                    top: 0px;
                    right: 12px;
                    background: #ce3333;
                    color: white;
                    border-radius: 50%;
                    height: 18px;
                    width: 18px;
                    line-height: 12px;
                    font-size: 12px;
                    display: flex;
                    font-weight: 600;
                    justify-content: center;
                    flex-direction: column;
                    text-align: center;
                    border: 1px solid white;
                    cursor: pointer;
                    &.longer_nav{
                        top: -5px;
                        right: 10px;
                        height: 24px;
                        width: 24px;
                    }
                }
            }
            .gh_i_r_userInfo{
                margin-left: 12px;
                color: #43425d;
                font-weight: 600;
                max-width: 150px;
                  @media (max-width: @screen-xs) {
                    max-width: 100px;
                    margin-bottom: 4px;
                  }
                
                .ur_greets{
                    font-size: 14px;
                }
                .ur_balance{
                    font-size: 12px;
                    .fixed-content{
                        position:fixed;
                        top: 50px !important;
                    }
                    .ur_balance_drawer{
                        font-size: 6px;
                        vertical-align: baseline;
                        cursor: pointer;
                    }
                }
            }
            .gH_i_r_menuList {
                display: flex;
                cursor: pointer;
            }
        }
    }
}
</style>
