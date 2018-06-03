<template>
    <div>
        <v-toolbar app fixed :height="height" class="header">
            
            <v-layout column :class="layoutClass?layoutClass:'header-elements'" class="mx-0">
                <smart-app-banner class="fixed-top" v-if="showSmartAppBanner && $vuetify.breakpoint.xsOnly"></smart-app-banner>
                <div class="main">
                    <v-flex class="line top">
                        <v-layout row>
                            <!-- :class="{'auth-user':isAuthUser}" -->
                            <v-toolbar-title >
                                <router-link class="logo-link" :to="{name:'home'}">
                                    <app-logo class="logo"></app-logo>
                                </router-link>
                            </v-toolbar-title>
                            <v-toolbar-items>
                                <search-input v-if="$vuetify.breakpoint.mdAndUp" :user-text="userText" :placeholder="this.$options.placeholders[currentSelection]" :submit-route="submitRoute"></search-input>
                                <!--<form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">-->
                                <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                                <!--<div v-for="(s,index) in suggestList">{{s}}</div>-->
                                <!--</form>-->
                                <v-spacer v-if="$vuetify.breakpoint.smAndDown"></v-spacer>
                                <div class="settings-wrapper d-flex align-center">
                                    <!-- <v-menu bottom left>
                                        <v-btn class="share-btn" icon slot="activator">
                                            <share-icon></share-icon>
                                        </v-btn>
                                        <v-list class="sharing-list">
                                            <v-list-tile @click="facebookShare">
                                                <v-list-tile-action>
                                                    <facebook-icon class="facebook-icon"></facebook-icon>
                                                </v-list-tile-action>
                                                <v-list-tile-content>
                                                    <v-list-tile-title>
                                                        <span>Facebook</span>
                                                    </v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                            <v-list-tile @click="twitterShare">
                                                <v-list-tile-action>
                                                    <twitter-icon class="twitter-icon"></twitter-icon>
                                                </v-list-tile-action>
                                                <v-list-tile-content>
                                                    <v-list-tile-title>Twitter</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                            <div @click="$ga.social('Whatsapp', 'Share')">
                                            <v-list-tile :href="whatsappLink()" class="btn-copy hidden-sm-and-up" v-if="$vuetify.breakpoint.xs">
                                                <v-list-tile-action>
                                                    <whatsapp-icon></whatsapp-icon>
                                                </v-list-tile-action>
                                                <v-list-tile-content>
                                                    <v-list-tile-title>Whatsapp</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                            </div>
                                            <v-list-tile @click="copyToClipboard" class="btn-copy">
                                                <v-list-tile-action>
                                                    <copy-link-icon></copy-link-icon>
                                                </v-list-tile-action>
                                                <v-list-tile-content>
                                                    <v-list-tile-title>Copy link</v-list-tile-title>
                                                    <input type="text" id="input-url" value="Copied!">
                                                </v-list-tile-content>
                                            </v-list-tile>
                                        </v-list>
                                    </v-menu>
                                    <v-menu bottom left>
                                        <v-btn icon slot="activator">
                                            <v-icon>sbf-3-dot</v-icon>
                                        </v-btn>
                                        <v-list class="settings-list">
                                            <v-list-tile @click="$_currentClick(item)" v-for="(item,index) in settingMenu" :key="index" :id="item.id">
                                                <v-list-tile-content>
                                                    <v-list-tile-title>{{item.id==='university'&&getUniversityName?getUniversityName:item.name}}</v-list-tile-title>
                                                </v-list-tile-content>
                                            </v-list-tile>
                                        </v-list>
                                    </v-menu> -->

                                    <div class="header-comments" v-if="isAuthUser && !isMobile">
                                        <router-link :to="{name:'conversations'}">
                                            <v-icon>sbf-comment</v-icon>
                                        <span class="red-counter">6</span>
                                    </router-link>
                                    </div>

                                    <div class="header-wallet" v-if="isAuthUser">
                                        <v-btn icon >
                                            <v-icon>sbf-wallet</v-icon>                                    
                                        </v-btn>
                                        <span>$25</span>
                                    </div>                                    
                                                                    
                                    <div class="header-rocket" v-if="isAuthUser && !isMobile">
                                        <v-menu bottom left offset-y >
                                            <v-btn icon slot="activator" @click.native="drawer = !drawer">
                                                <v-icon>sbf-rocket</v-icon>
                                            </v-btn>
                                            <menu-list :isAuthUser="isAuthUser" ></menu-list>                                            
                                        </v-menu>
                                        <span class="red-counter">6</span>                                    
                                    </div>

                                    
                                    <a v-if="!isAuthUser" class="header-login" href="/register">Sign Up</a>
                                    <a v-if="!isAuthUser" class="header-login" href="/signin">Login</a>
                                    
                
                                    <v-menu bottom left offset-y class="gamburger" v-if="!isAuthUser || isMobile">
                                        <v-btn icon slot="activator" @click.native="drawer = !drawer">
                                            <v-icon>sbf-menu</v-icon>
                                        </v-btn>
                                        <menu-list :isAuthUser="isAuthUser" v-if="!isMobile"></menu-list>
                                    </v-menu>   
                                    
                                </div>
                            </v-toolbar-items>
                        </v-layout>
                    </v-flex>
                    <v-flex v-if="$vuetify.breakpoint.smAndDown" class="line search-wrapper">
                        <search-input :user-text="userText" :placeholder="this.$options.placeholders[currentSelection]" :submit-route="submitRoute"></search-input>
                        <!--<form @submit.prevent="submit">-->
                        <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                        <!--</form>-->
                    </v-flex>
                </div>
                <slot name="extraHeader"></slot>
            </v-layout>
            <personalize-dialog ref="personalize" :value="clickOnce"></personalize-dialog>

            
        </v-toolbar>

        <v-navigation-drawer temporary v-model="drawer" light absolute app v-if=isMobile width="280">
            <menu-list :isAuthUser="isAuthUser"></menu-list>
        </v-navigation-drawer>

    </div>
</template>

<script>
    import { settingMenu, notRegMenu } from '../settings/consts';
    import SearchInput from '../helpers/searchInput.vue';
    import smartAppBanner from "../smartAppBanner/smartAppBanner.vue";
    import menuList from "./menu-list/menu-list.vue";    
    
    import {mapGetters} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    const PersonalizeDialog=()=> import('./ResultPersonalize.vue');
    import ShareIcon from "./img/share-icon.svg";
    import FacebookIcon from "../home/svg/facebook-icon.svg"
    import TwitterIcon from "../home/svg/twitter-icon.svg"
    import WhatsappIcon from "./svg/whatsapp-icon.svg"
    import CopyLinkIcon from "./svg/copy-link-icon.svg"

    export default {
        placeholders:{
            job:"Your field of expertise...",
            tutor: "Find a tutor...",
            note:"Find study documents in...",
            book:"Textbook title or ISBN...",
            ask:"Ask anything...",
            flashcard:"Look for flashcards...",
            food:"Search for deals..."
        },
        computed: {
            ...mapGetters(['getUniversityName', 'showSmartAppBanner']),
            isMobile(){return this.$vuetify.breakpoint.xsOnly;}
    },
        watch:{
            toolbarHeight(val) {
                this.height = val;
            },
            showSmartAppBanner(val){
                let headerHeight =this.toolbarHeight?this.toolbarHeight:(this.$vuetify.breakpoint.mdAndUp ? 60 : 115)
                this.height =  this.$vuetify.breakpoint.xsOnly && val? headerHeight + 84 : headerHeight;
            }
        },
        components: {
            PersonalizeDialog, ShareIcon, FacebookIcon, TwitterIcon, WhatsappIcon, CopyLinkIcon,AppLogo,SearchInput,smartAppBanner, menuList
        },
        props:{currentSelection:{type:String,default:'note'},userText:{type:String},submitRoute:{type:String,default:'/result'},toolbarHeight:{},layoutClass:{}},
        data(){
            return {
                settingMenu,
                notRegMenu,
                clickOnce:false,
                drawer: null,
                isAuthUser:true
            }
        },
        methods:{
            $_currentClick({ id,name }) {
                if(name==='Feedback'){
                    Intercom('showNewMessage', '');
                }else{
                    this.clickOnce=true;
                    this.$nextTick(()=> {
                            this.$refs.personalize.openDialog(id);
                    })
                }
            },
            facebookShare() {
                const shareFb = 'https://www.facebook.com/sharer/sharer.php?u=' + encodeURIComponent(window.location.href);
                this.$ga.social('Facebook', 'Share');
                window.open(shareFb, "pop", "width=600, height=400, scrollbars=no");
            },

            twitterShare() {
                const shareTwiiter = "https://twitter.com/intent/tweet?text=" + encodeURIComponent(window.location.href);
                this.$ga.social('Twitter', 'Share');
                window.open(shareTwiiter, "pop", "width=600, height=400, scrollbars=no");
            },

            copyToClipboard() {
                let el = document.getElementById("input-url");
                el.value = window.location.href;
                this.$ga.event('CopyClipboard');

                // handle iOS as a special case
                if (navigator.userAgent.match(/ipad|ipod|iphone/i)) {
                    // convert to editable with readonly to stop iOS keyboard opening
                    el.contentEditable = true;
                    el.readOnly = true;

                    // create a selectable range
                    var range = document.createRange();
                    range.selectNodeContents(el);

                    // select the range
                    var selection = window.getSelection();
                    selection.removeAllRanges();
                    selection.addRange(range);
                    el.setSelectionRange(0, 999999);
                }
                else {
                    el.select();
                }

                // execute copy command
                document.execCommand('copy');
            },


            whatsappLink() {
                return "whatsapp://send?text=" + encodeURIComponent(window.location.href);
            }
        },
        created(){
            this.$root.$on("personalize",
                (type) => {
                    this.clickOnce=true;
                    this.$nextTick(()=>{
                        this.$refs.personalize.openDialog(type);
                    })
                });
            let headerHeight =this.toolbarHeight?this.toolbarHeight:(this.$vuetify.breakpoint.mdAndUp ? 60 : 115)
            this.height =  this.$vuetify.breakpoint.xsOnly && this.showSmartAppBanner? headerHeight + 84 : headerHeight;

            // this.isAuthUser =
        }
    }
</script>
<style src="./header.less" lang="less"></style>
