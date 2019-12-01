<template>
    <div v-if="!isHideHeader">

    <v-toolbar class="globalHeader elevation-0" color="white" :height="isMobile? 60 : 70" app :fixed="true" clipped-left clipped-right>
            <router-link @click.prevent="resetItems()" to="/" class="globalHeader_logo">
                <logoComponent/>
            </router-link>
            <div class="globalHeader_items">
                <div class="globalHeader_items_left" v-if="!isMobile">
                    <searchCMP :placeholder="searchPlaceholder"/>
                </div>
                <v-spacer v-else></v-spacer>
                <div class="globalHeader_items_right">
                    <router-link v-if="!isMobile" :to="{name:'tutorLandingPage'}" class="gH_i_r_findTutor"  v-language:inner="'header_find_tutors'"/>
                    <template v-if="!isMobile && loggedIn" >
                        <v-tooltip bottom>
                            <template v-slot:activator="{on}">
                                <v-icon v-on="on" v-if="!$vuetify.breakpoint.smAndDown" id="gH_i_r_intercom" class="gH_i_r_intercom" v-html="'sbf-help'"/>
                            </template>
                            <span v-language:inner="'header_tooltip_help'"/>
                        </v-tooltip>
                        
                        <v-tooltip bottom>
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
                        <button class="gH_i_r_btns gH_i_r_btn_in mr-2" @click="$router.push({path:'/signin'})" v-language:inner="'tutorListLanding_topnav_btn_login'">
                        </button>
                        <button class="gH_i_r_btns gH_i_r_btn_up mr-3" @click="$router.push({path:'/register'})" v-language:inner="'tutorListLanding_topnav_btn_signup'">
                        </button>
                    </template>
                    <v-menu close-on-content-click bottom right offset-y :content-class="'fixed-content'" sel="menu">
                        <template v-if="loggedIn" slot="activator">
                            <user-avatar  
                            @click.native="drawer=!drawer" 
                            size="40" 
                            :userImageUrl="userImageUrl" 
                            :user-name="accountUser.name"/>
                            
                            <div v-if="!$vuetify.breakpoint.mdAndDown" class="gh_i_r_userInfo text-truncate" @click.prevent="drawer=!drawer">
                                <span class="ur_greets" v-html="$Ph('header_greets', accountUser.name)"/>
                                <div class="ur_balance">
                                    <span v-html="$Ph('header_balance', userBalance(accountUser.balance))"/>
                                    <v-icon v-if="!isMobile" class="ur_balance_drawer ml-2" color="#43425d" v-html="'sbf-arrow-fill'"/>
                                </div>
                            </div>
                        </template>
                        <template v-else slot="activator">
                            <v-btn :ripple="false" icon @click.native="drawer = !drawer">
                                <v-icon small v-html="'sbf-menu'"/>
                            </v-btn>
                        </template>
                        <menuList v-if="!$vuetify.breakpoint.xsOnly"/>
                    </v-menu>
                </div>
            </div>
             <template v-slot:extension v-if="isMobile">
                 <div class="mobileHeaderSearch">
                    <searchCMP :placeholder="searchPlaceholder"/>
                 </div>
            </template>

    </v-toolbar>
            <v-navigation-drawer temporary v-model="drawer" light :right="!isRtl"
                             fixed app v-if="$vuetify.breakpoint.xsOnly" class="drawerIndex"
                             :class="isRtl ? 'hebrew-drawer' : ''"
                             width="280">
            <menuList @closeMenu="closeDrawer"/>
        </v-navigation-drawer>
    </div>
</template>

<script>
import {mapActions, mapGetters, mapMutations} from 'vuex';
import { LanguageService } from "../../../../services/language/languageService";

import searchCMP from '../../global/search/search.vue';
import UserAvatar from '../../../helpers/UserAvatar/UserAvatar.vue';
import menuList from '../menuList/menuList.vue';

import logoComponent from '../../../app/logo/logo.vue';

export default {
    components: {searchCMP,UserAvatar,menuList,logoComponent},
    data() {
        return {
            drawer: false,


            clickOnce: false,
            isRtl: global.isRtl
        }
    },
    props: {
        layoutClass: {}
    },
    computed: {
        ...mapGetters(['accountUser','getTotalUnread']),
        isTablet(){
            return this.$vuetify.breakpoint.smAndDown;
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly;
        },
        userImageUrl(){
            return this.accountUser.image.length > 1 ? `${this.accountUser.image}` : '';
        },
        loggedIn() {
            return this.accountUser !== null;
        },
        totalUnread(){
            return this.getTotalUnread;
        },
        isHideHeader(){
            let filteredRoutes = ['document','profile'];
            return filteredRoutes.indexOf(this.$route.name) > -1 && this.$vuetify.breakpoint.xsOnly;
        },
        searchPlaceholder(){
            return this.isTablet ? LanguageService.getValueByKey(`header_placeholder_search`) : LanguageService.getValueByKey(`header_placeholder_search_m`);
        },
    },
    watch: {
        drawer(val){
            if(!!val && this.$vuetify.breakpoint.xsOnly){
                document.body.className="noscroll";
            }else{
                document.body.removeAttribute("class","noscroll");
            }
        }
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
        closeDrawer(){
            this.drawer = !this.drawer;
        },         
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
        userBalance(balance){
            let balanceFixed = +balance.toFixed()
            return balanceFixed.toLocaleString(`${global.lang}`)
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
    border: solid 1px #dadada !important;
    z-index: 200;
    .v-toolbar__extension{
    @media (max-width: @screen-xs) {
      padding: 0 8px
    }
    }
    .v-toolbar__content{
        padding-left: 16px;
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
        svg{
            @media (max-width: @screen-xs) {
                width: 94px;
                height: 22px;    
            }  
            width: 120px;
            height: 30px;
            fill: #43425D;
            vertical-align: bottom;
            // margin-right: 32px;
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
                input{

                }
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
                    input{
                        // margin-bottom: 4px;
                    }
                }
            .searchCMP-input{
                .v-text-field__slot{
                    line-height: 18px;
                    // height: 18px;
                    align-items: normal;
                }

            } 
            }
        }
        .globalHeader_items_right{
            .flexSameSize();
            display: flex;
            align-items: center;
            .gH_i_r_btns{
                text-align: center;
                border-radius: 6px;
                font-size: 14px;
                outline: none;
                &.gH_i_r_btn_up {
                    padding: 7px 14px;
                    margin: 8px;
                    background-color: #4c59ff;
                    color: white;
                }
                &.gH_i_r_btn_in {
                    padding: 7px 20px;
                    margin: 5px;
                    border: solid 1px #43425d;
                    color: #43425d;
                    background-color: transparent;
                }
            }
            .gH_i_r_findTutor{
                font-size: 14px;
                font-weight: 600;
                color: #43425d;
                margin-right: 26px;
            }
            .gH_i_r_intercom{
                cursor: pointer;
                color: #bdc0d1;
                font-size: 22px;
                padding-top: 4px;
                margin-right: 26px;
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
                    font-size: 10px;
                    display: flex;
                    font-weight: 500;
                    justify-content: center;
                    flex-direction: column;
                    text-align: center;
                    border: 1px solid white;
                    cursor: pointer;
                    &.longer_nav{
                        top: 1px;
                        right: 4px;
                        height: 16px;
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
        }
    }
}
</style>
