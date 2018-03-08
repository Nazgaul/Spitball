<template>
    <v-toolbar app fixed :height="height" class="header">
        <v-layout column :class="layoutClass?layoutClass:'header-elements'" class="mx-0">
            <div class="main">
                <v-flex class="line top">
                    <v-layout row>
                        <v-toolbar-title class="ma-0">
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
                                <v-menu bottom left>
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
</template>

<script>
    import { settingMenu } from '../settings/consts';
    import SearchInput from '../helpers/searchInput.vue';
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
            ...mapGetters(['getUniversityName']),
        },
        watch:{
            toolbarHeight(val) {
                this.height = val;
            }
        },
        components: {
            PersonalizeDialog, ShareIcon, FacebookIcon, TwitterIcon, WhatsappIcon, CopyLinkIcon,AppLogo,SearchInput
        },
        props:{currentSelection:{type:String,default:'note'},userText:{type:String},submitRoute:{type:String,default:'/result'},toolbarHeight:{},layoutClass:{}},
        data(){return {settingMenu,clickOnce:false}},
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
                let copyText = document.getElementById("input-url");
                copyText.value = window.location.href;
                this.$ga.event('CopyClipboard');
                copyText.select();
                document.execCommand("Copy");
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
            this.height=this.toolbarHeight?this.toolbarHeight:(this.$vuetify.breakpoint.mdAndUp ? 60 : 115)
        }
    }
</script>
<style src="./header.less" lang="less"></style>
