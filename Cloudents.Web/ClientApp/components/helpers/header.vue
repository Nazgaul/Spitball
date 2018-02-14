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
                           <search-input v-if="$vuetify.breakpoint.mdAndUp" :user-text="userText" :placeholder="placeholders[currentSelection]" :submit-route="submitRoute"></search-input>
                           <!--<form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">-->
                               <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                                <!--<div v-for="(s,index) in suggestList">{{s}}</div>-->
                           <!--</form>-->
                           <v-spacer v-if="$vuetify.breakpoint.smAndDown"></v-spacer>
                           <div class="settings-wrapper d-flex align-center">
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
                   <search-input :user-text="userText" :placeholder="placeholders[currentSelection]" :submit-route="submitRoute"></search-input>
                   <!--<form @submit.prevent="submit">-->
                       <!--<v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>-->
                   <!--</form>-->
               </v-flex>
           </div>
        <slot name="extraHeader"></slot>
    </v-layout></v-toolbar>
</template>

<script>
    import { settingMenu } from '../settings/consts';
    import SearchInput from '../helpers/searchInput.vue';
    import {mapGetters} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";
    import PersonalizeDialog from './ResultPersonalize.vue'
    import ShareIcon from "./img/share-icon.svg";
    import FacebookIcon from "../home/svg/facebook-icon.svg"
    import TwitterIcon from "../home/svg/twitter-icon.svg"
    import WhatsappIcon from "./svg/whatsapp-icon.svg"
    import CopyLinkIcon from "./svg/copy-link-icon.svg"

    let placeholders={
        job:"Your field of expertise...",
        tutor: "Find a tutor...",
        note:"Find study documents in...",
        book:"Textbook title or ISBN...",
        ask:"Ask anything...",
        flashcard:"Look for flashcards...",
        food:"Search for deals..."
    };
    export default {
        computed: {
            ...mapGetters(['getUniversityName'])},
        watch:{
            toolbarHeight(val) {
                this.height = val;
            }
        },
        components:{AppLogo,SearchInput},
        props:{currentSelection:{type:String,default:'note'},userText:{type:String},submitRoute:{type:String,default:'/result'},toolbarHeight:{},layoutClass:{}},
        data(){return {settingMenu,placeholders}},
        methods:{
            $_currentClick(item){
                this.$root.$emit("personalize", item.id);
            }
        },
        created(){
            this.height=this.toolbarHeight?this.toolbarHeight:(this.$vuetify.breakpoint.mdAndUp ? 60 : 115)
        }
    }
</script>
<style src="./header.less" lang="less"></style>
