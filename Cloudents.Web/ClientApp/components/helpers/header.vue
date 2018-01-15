<template>
    <v-layout column class="header-elements pb-2">
    <v-flex class="line">
        <v-layout row>
            <v-toolbar-title class="ma-0">
                <router-link class="logo-link" :to="{name:'home'}">
                    <app-logo class="logo"></app-logo>
                </router-link>
            </v-toolbar-title>
            <v-toolbar-items>
                <form v-if="$vuetify.breakpoint.mdAndUp" @submit.prevent="submit">
                    <v-text-field type="search" light solo class="search-b"  :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
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
    <v-flex v-if="$vuetify.breakpoint.smAndDown" class="line">
        <form @submit.prevent="submit">
            <v-text-field type="search" light solo class="search-b" :placeholder="placeholders[currentSelection]" v-model="msg" prepend-icon="sbf-search" :append-icon="voiceAppend" :append-icon-cb="$_voiceDetection"></v-text-field>
        </form>
    </v-flex>
    </v-layout>
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
            ...mapGetters(['getUniversityName','globalTerm'])
        },
        components:{AppLogo},
        props:{currentSelection:{type:String,default:'ask'},userText:{type:String}},
        data(){return {settingMenu,placeholders}},
        methods:{
            submit: function () {
                this.$router.push({name:"result",query:{q:this.msg}});
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
        }
    }
</script>
<!--//TODO:seperate the header css -->
<style src="./../header/header.less" lang="less"></style>
