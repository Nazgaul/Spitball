<template>
    <!-- <div> -->
        <v-bottom-navigation
                height="62px"
                :v-model="activeTab"
                :value="activeTab"
                fixed
                grow
                color="white"
                :app="$vuetify.breakpoint.xsOnly"
                class="notransition mobileFooter">
            <v-btn :ripple="false" class="mF_btns" text value="feed" @click="changeActiveTab('feed')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_home'"/>
                <v-icon class="mF_icon" v-html="'sbf-home-tab'"/>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" text value="tutorLandingPage" @click="changeActiveTab('tutorLandingPage')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_tutors'"/>
                <v-icon class="mF_icon" v-html="'sbf-account-group'"/>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" text value='upload' v-openDialog="uploadDialog">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_upload'"/>
                <v-icon class="mF_icon" v-html="'sbf-button-add'" />
            </v-btn> -->
            <v-btn :ripple="false" class="mF_btns" text value="chat" @click="openChat">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_chat'"/>
                <span class="mF_chat">
                    <v-icon class="mF_icon" v-html="'sbf-btm-msg'"/>
                    <span class="mF_chat_unread" v-show="totalUnread > 0">{{totalUnread}}</span>
                </span>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" text value="profile" @click="changeActiveTab('profile')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_profile'"/>
                <v-icon class="mF_icon" v-html="'sbf-account'"/>
            </v-btn>
        </v-bottom-navigation>
    <!-- </div> -->
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import * as dialogNames from '../../global/dialogInjection/dialogNames.js'
export default {
    name: "mobileFooter",
    data() {
        return {
            currentActiveTab:this.$route.name,
            uploadDialog: dialogNames.Upload,
        }
    },
    computed: {
        ...mapGetters(['getTotalUnread', 'accountUser']),
        totalUnread(){
            return this.getTotalUnread
        },
        activeTab:{
            get(){
                return this.currentActiveTab;
            },
            set(tabName){
                this.currentActiveTab = tabName;
            }
        }
    },
        watch:{
        '$route'(route){
            if(this.$route.name === 'profile'){
                if(!!this.accountUser){
                    if(this.$route.params.id == this.accountUser.id){
                        this.currentActiveTab = 'profile';
                        }else{
                            this.currentActiveTab = null;
                        }
                }else{
                    this.currentActiveTab = route.name;
                }
            }else{
                this.currentActiveTab = route.name;
            }
        },
        currentActiveTab(newVal, oldVal){
            if(newVal !== this.$route.name){
                setTimeout(()=>{
                    this.currentActiveTab = oldVal;
                }, 500);
                
            }
        }
    },
    methods: {
        ...mapActions(['openChatInterface']),
        openChat(){
            if (this.accountUser == null) {
                // this.$openDialog('login');
                this.$store.commit('setComponent', 'register')
            }else{
                this.openChatInterface();
            }
        },
        changeActiveTab(tabName){
            if(tabName === 'feed' && this.activeTab !== tabName){
                this.$router.push({name:'feed'})
            }
            if(tabName === 'tutorLandingPage' && this.activeTab !== tabName){
                this.$router.push({name:'tutorLandingPage'});
            }
            if(tabName === 'profile' && this.activeTab !== tabName){
                if(this.accountUser == null) {
                    // this.$openDialog('login');
                    this.$store.commit('setComponent', 'register')
                }else{
                    this.$router.push({name:'profile',params:{id: this.accountUser.id,name: this.accountUser.name}})
                }
            }
            this.activeTab = 'feed'
        },
    },
}
</script>

<style lang="less">
@import "../../../../styles/mixin.less";
.v-bottom-navigation {
    &.mobileFooter{
        z-index:11;
        box-shadow: none;
        border-top: solid 1px  #C7C7CD!important;
        .mF_btns{
            min-width: 64px;
            padding-top: 2px;
            .v-btn__content{
                margin-top: -2px;
            }
            .mF_title{
                font-size: 12px;
                font-weight: 600;
                font-stretch: normal;
                font-style: normal;
                line-height: normal;
                letter-spacing: normal;
            }
            .mF_icon{
                font-size: 26px;
                margin-top: 2px;
                margin-bottom: 2px !important;
            }
            .mF_chat{
                position: relative;
                .mF_chat_unread{
                    position: absolute;
                    top: -2px;
                    right: -6px;
                    background: #ce3333;
                    color: #fff;
                    border-radius: 50%;
                    height: 16px;
                    width: 16px;
                    line-height: 16px;
                    display: flex;
                    font-size: 12px;
                    justify-content: center;
                    flex-direction: column;
                    text-align: center;
                }
            }
        }
    }
}
    
.v-btn--active {
        padding-top: 2px;
    .v-btn__content {
        color: #5560ff;
    }
}
</style>