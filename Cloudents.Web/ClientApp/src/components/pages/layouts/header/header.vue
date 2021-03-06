﻿<template>
    <div>
    <v-app-bar :class="{'homePageWrapper': isHomePage}" class="globalHeader borderBottom elevation-0" color="white" :height="isMobile? 60 : 70" app fixed clipped-left clipped-right>
        <v-btn v-if="showHamburgerIcon" class="d-sm-none" :class="[{'d-block': classChangeHamburgerTutorMenu}]" :ripple="false" icon @click="$root.$emit('openSideMenu')">
            <hamburgerIcon class="ms-2 hamburgerIcon"/>
        </v-btn>
        <router-link @click.prevent="resetItems()" to="/" :class="{'globalHeader_logo': !$route.meta.tutorHeaderSlot}">
            <logoComponent/>
        </router-link>
        <template v-if="$route.meta.tutorHeaderSlot">
            <div class="dividerName mx-8" v-show="!isMobile"></div>
            <div class="tutorName text-truncate text-center text-sm-start mb-1">{{tutorName}}</div>
        </template>
        <div class="globalHeader_items" :class="{'tutorProfile': $route.name === profileRoute}">
            <v-spacer></v-spacer>
            <div class="globalHeader_items_right">
                <div>
                    <component :is="$route.meta.headerSlot"/>
                </div>
                <template v-if="!isMobile" >
                    <v-tooltip bottom>
                        <template v-slot:activator="{on}">
                            <helpIcon @click="startIntercom" v-on="on" v-if="!$vuetify.breakpoint.smAndDown" class="gH_i_r_intercom" :class="{'gH_i_r_intercom--margin': !loggedIn}" />
                        </template>
                        <span v-t="'header_tooltip_help'"/>
                    </v-tooltip>
                    <template v-if="showChat">
                        <v-tooltip bottom v-if="loggedIn">
                            <template v-slot:activator="{on}">
                                <div v-on="on" class="gH_i_r_chat">
                                    <chatIcon class="gH_i_r_chat_i" @click="openChatWindow"/>
                                    <span @click="openChatWindow" class="unread_circle_nav" v-show="totalUnread > 0" :class="[totalUnread > 9 ? 'longer_nav' :'']">{{totalUnread}}</span>
                                </div>
                            </template>
                            <span v-t="'header_tooltip_chat'"/>
                        </v-tooltip>
                    </template>
                </template>
                <template v-if="!$vuetify.breakpoint.smAndDown && !loggedIn">
                    <button class="gH_i_r_btns gH_i_r_btn_in me-2" @click="$store.commit('setComponent', 'login')" v-t="'tutorListLanding_topnav_btn_login'"/>
                    <button class="gH_i_r_btns gH_i_r_btn_up me-4" @click="$store.commit('setComponent', 'registerType')" v-t="'tutorListLanding_topnav_btn_signup'"/>
                    <a class="gH_i_lang" @click="changeLanguage()" v-if="showChangeLanguage" sel="language" v-html="currLanguage !== languageChoisesAval.id? languageChoisesAval.title : ''"/>
                </template>
                <v-menu fixed close-on-content-click bottom offset-y :content-class="getBannerParams? 'fixed-content-banner':'fixed-content'">
                    <template v-slot:activator="{on}">
                        <div v-on="on" class="gH_i_r_menuList" sel="menu">
                            <div @click.prevent="drawer=!drawer">
                                <UserAvatarNew
                                    :width="40"
                                    :height="40"
                                    :userImageUrl="userImageUrl"
                                    :userName="userName"
                                    :fontSize="14"
                                />
                                    <!-- :loading="avatarUpdate"
                                    @setAvatarLoaded="val => avatarUpdate = val" -->
                            </div>
                            <!-- <template v-if="loggedIn">
                                <div v-if="!$vuetify.breakpoint.mdAndDown" class="gh_i_r_userInfo text-truncate" @click.prevent="drawer=!drawer">
                                    <span class="ur_greets">{{$t('header_greets', [userName])}}</span>
                                    <div class="ur_balance">
                                        <span>{{$t('header_balance', {'0': $n(getUserBalance)})}}</span>
                                        <arrowDownIcon v-if="!isMobile" class="ur_balance_drawer ms-2"/>
                                    </div>
                                </div>
                            </template> -->
                            <template>
                                <v-btn :class="[{'hidden-md-and-up': isHomePage},{'d-none':!isHomePage && loggedIn}]" :ripple="false" icon @click.native="drawer = !drawer">
                                    <hamburgerIcon class="hamburgerIcon"/>
                                </v-btn>
                            </template>
                        </div>
                    </template>
                    <menuList v-if="!$vuetify.breakpoint.xsOnly"/>
                </v-menu>
            </div>
        </div>
    </v-app-bar>
        <v-navigation-drawer 
            temporary 
            v-model="drawer" 
            light 
            :right="!$vuetify.rtl"
            fixed 
            app 
            v-if="$vuetify.breakpoint.xsOnly" 
            class="drawerIndex"
            width="280">
            <menuList @closeMenu="closeDrawer"/>
        </v-navigation-drawer>
    </div>
</template>

<script>
import {mapGetters} from 'vuex';
import languagesLocales from "../../../../services/language/localeLanguage";
import * as routeNames from '../../../../routes/routeNames.js';

import menuList from '../menuList/menuList.vue';
import intercomService from "../../../../services/intercomService";
import logoComponent from '../../../app/logo/logo.vue';
import findSVG from './images/findSVG.svg'
import helpIcon from '../../../../font-icon/help-icon.svg';
import chatIcon from './images/chatIcon.svg';
import arrowDownIcon from './images/arrowDownIcon.svg';
import hamburgerIcon from './images/hamburgerIcon.svg';
const phoneNumberSlot = () => import('./headerSlots/phoneNumberSlot.vue');


export default {
components: {menuList,logoComponent,findSVG,phoneNumberSlot,helpIcon,chatIcon,arrowDownIcon,hamburgerIcon},
    data() {
        return {
            // avatarUpdate: false,
            drawer: false,
            profileRoute: routeNames.Profile,
            currentRoute: this.$route.name,
            languageChoisesAval: [],
            currLanguage: document.documentElement.lang,
            clickOnce: false,
        }
    },
    props: {
        layoutClass: {}
    },
    computed: {
        ...mapGetters(['accountUser','getTotalUnread','getBannerParams','getUserLoggedInStatus','getUserBalance', 'getIsTeacher']),
        tutorName() {
            return this.$store.getters.getProfileTutorName
        },
        loggedIn() {
            return this.getUserLoggedInStatus;
        },
        // isTablet(){
        //     return this.$vuetify.breakpoint.smAndDown;
        // },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl(){
            return this.loggedIn && this.accountUser?.image.length > 1 ? this.accountUser.image : '';
        },
        userName(){
            return this.loggedIn && this.accountUser?.name ? this.accountUser.name : '';
        },
        totalUnread(){
            return this.getTotalUnread;
        },
        showChat(){
            return this.$store.getters.getIsAccountChat
        },
        // isShowBorderBottom(){
        //     let filteredRoutes = [routeNames.Profile, routeNames.Document];
        //     return filteredRoutes.indexOf(this.$route.name) > -1;
        // },
        isHomePage(){
            let showRoutes = [routeNames.Learning,routeNames.HomePage];
            return showRoutes.indexOf(this.currentRoute) !== -1
        },
     
        showChangeLanguage() {
            return global.country === 'IL' && this.isHomePage;
        },
        showHamburgerIcon() {
            let showRoutes = [routeNames.Profile, routeNames.Document, routeNames.Learning];
            return this.getIsTeacher && showRoutes.indexOf(this.currentRoute) === -1
        },
        classChangeHamburgerTutorMenu() {
            return this.isHomePage || !this.loggedIn
        }
    },
    watch: {
    '$route'(){
      this.$nextTick(()=>{
            this.drawer = false;
            this.currentRoute = this.$route.name
      })
    },
    },
    methods: {
        openChatWindow(){
            this.$router.push({name: routeNames.MessageCenter})
        },
        resetItems(){
            this.$router.push('/');
        },
        closeDrawer() {
            this.drawer = !this.drawer;
        },       
        startIntercom() {
            intercomService.showDialog();
        },
        changeLanguage() {
            this.$store.dispatch('changeLanguage', this.languageChoisesAval.id)
        },
    },
    created() {
        this.$root.$on("closeDrawer", ()=>{
            this.$nextTick(() => {
                this.closeDrawer();
            })
        })
        // this.$root.$on("avatarUpdate", (val) => {
        //     this.avatarUpdate = val
        // })
        let currentLocHTML = document.documentElement.lang;
        this.languageChoisesAval = languagesLocales.filter(lan => {
            return lan.locale !== currentLocHTML;
        })[0];
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
            padding: 0 8px;
            border-bottom: solid 1px #dadada;
        }
    }
    &.borderBottom {
        .v-toolbar__content{
            @media (max-width: @screen-xs) {
                border-bottom: solid 1px #dadada;
            }
        }
    }
    .v-toolbar__content{
        border-bottom: solid 1px #dadada;
        padding: 0 24px 0 16px;
        @media (max-width: @screen-xs) {
            padding: 0 8px 0 4px;
            border-bottom: none;
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
        .dividerName {
            height: 28px;
            width: 4px;
            font-weight: bold;
            background: #000;
        }
        .tutorName {
            width: 100%;
            color: #363637;
            font-weight: bold;
            font-size: 22px;

            @media(max-width: @screen-xs) {
                font-size: 18px;
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
        &.tutorProfile {
            @media (max-width: @screen-mds) {
                width: unset;
                margin-left: 0; 
            }
        }
        /*.globalHeader_items_left{*/
        /*    width: 100%;*/
        /*    max-width: 564px;*/
        /*    height: 38px;*/
        /*    border: solid 1px #c1c3ce;*/
        /*    border-radius: 8px;*/
        /*    margin-right: 18px;*/
        /*    .searchCMP{*/
        /*        border-radius: 7px;*/
        /*        .v-input__icon{*/
        /*            i{*/
        /*                color: #43425d !important;*/
        /*            }*/
        /*        } */
        /*        .searchCMP-btn{*/
        /*            max-width: 72px;*/
        /*            font-size: 14px;*/
        /*        }*/
        /*        .v-input__slot{*/
        /*            padding-left: 8px;*/
        /*        }*/
        /*        ::placeholder {*/
        /*            color: #6a697f !important;*/
        /*            font-weight: normal;*/
        /*            font-stretch: normal;*/
        /*            font-style: normal;*/
        /*            letter-spacing: normal;*/
        /*            font-size: 14px;*/
        /*        }*/
        /*        .v-text-field{*/
        /*            input{*/
        /*                padding: initial;*/
        /*            }*/
        /*        }*/
        /*        .v-text-field__slot{*/
        /*            color: #6a697f !important;*/
        /*            font-size: 14px;*/
        /*        }*/
        /*    .searchCMP-input{*/
        /*        .v-text-field__slot{*/
        /*            line-height: 18px;*/
        /*            //margin-bottom: 2px;*/
        /*            // height: 18px;*/
        /*            //align-items: normal;*/
        /*        }*/

        /*    } */
        /*    }*/
        /*}*/
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
          
            .gH_i_r_intercom{
                cursor: pointer;
                fill: #bdc0d1;
                width: 22px;
                padding-top: 4px;
                margin-right: 26px;
                outline: none;
                &--margin {
                    margin-right: 20px;
                    margin-bottom: 1px;
                }
            }
            .gH_i_r_chat{
                cursor: pointer;
                position: relative;
                margin-right: 26px;
                .gH_i_r_chat_i{
                    fill: #bdc0d1;
                    width: 22px;
                    padding-top: 8px;
                    vertical-align: bottom;
                }
                .unread_circle_nav{
                    position: absolute;
                    top: 0;
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
            .hamburgerIcon{
                fill: rgba(0, 0, 0, 0.54)
            }
            // .gh_i_r_userInfo{
            //     margin-left: 12px;
            //     color: #43425d;
            //     font-weight: 600;
            //     max-width: 150px;
            //       @media (max-width: @screen-xs) {
            //         max-width: 100px;
            //         margin-bottom: 4px;
            //       }
                
            //     .ur_greets{
            //         font-size: 14px;
            //     }
            //     .ur_balance{
            //         font-size: 12px;
            //         .fixed-content{
            //             position:fixed;
            //             top: 50px !important;
            //         }
            //         .ur_balance_drawer{
            //             cursor: pointer;
            //             width: 11px;
            //         }
            //     }
            // }
            .gH_i_r_menuList {
                display: flex;
                cursor: pointer;

                .v-lazy {
                    display: flex;
                }
            }
        }
    }
}
</style>
