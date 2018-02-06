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
                           <form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">
                               <v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
                                <!--<div v-for="(s,index) in suggestList">{{s}}</div>-->
                           </form>
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
                   <form @submit.prevent="submit">
                       <v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
                   </form>
               </v-flex>
           </div>
        <slot name="extraHeader"></slot>
    </v-layout></v-toolbar>
</template>

<script>
    import { settingMenu } from '../settings/consts';
    import { micMixin } from '../helpers/mic';
    import {mapGetters} from 'vuex';
    import AppLogo from "../../../wwwroot/Images/logo-spitball.svg";

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
        mixins:[micMixin],
        computed: {
            ...mapGetters(['getUniversityName','historyTermSet']),
            ...mapGetters({'globalTerm':'currentText'}),
            suggestList(){
               return this.historyTermSet.filter(i=>i.includes(this.msg)).slice(0,4)
            }
        },
        watch:{
            userText(val){
                this.msg=val;
            },
            toolbarHeight(val) {
                this.height = val;
            }
        },
        components:{AppLogo},
        props:{currentSelection:{type:String,default:'note'},userText:{type:String},submitRoute:{type:String,default:'/result'},toolbarHeight:{},layoutClass:{}},
        data(){return {settingMenu,placeholders}},
        methods:{
            submit: function () {
                this.$router.push({path:this.submitRoute,query:{q:this.msg}});
                // to remove keyboard on mobile
                this.$nextTick(() => {
                    this.$el.querySelector('input').blur();
                    this.$el.querySelector('form').blur();
                });
            },
            //callback for mobile submit mic
            submitMic() {
                this.submit();
            },
            $_currentClick(item){
                this.$root.$emit("personalize", item.id);
            }
        },
        created(){
            this.msg=this.userText?this.userText:this.globalTerm;
            this.height=this.toolbarHeight?this.toolbarHeight:(this.$vuetify.breakpoint.mdAndUp ? 60 : 115)
        }
    }
</script>
<style src="./header.less" lang="less"></style>
