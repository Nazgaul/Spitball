<template>
    <div>
        <v-bottom-nav
                height="62px"
                :active.sync="activeTab"
                :value="true"
                fixed
                color="white"
                :app="$vuetify.breakpoint.xsOnly"
                class="notransition mobileFooter">
            <v-btn :ripple="false" class="mF_btns" flat color="#69687d" value="feed" @click="changeActiveTab('feed')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_home'"/>
                <v-icon class="mF_icon" v-html="'sbf-home-tab'"/>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" flat color="#69687d" value="tutorLandingPage" @click="changeActiveTab('tutorLandingPage')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_tutors'"/>
                <v-icon class="mF_icon" v-html="'sbf-account-group'"/>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" flat color="#69687d" value='upload' @click="openUpload">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_upload'"/>
                <v-icon class="mF_icon" v-html="'sbf-button-add'" />
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" flat color="#69687d" value="chat" @click="openChat">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_chat'"/>
                <span class="mF_chat">
                    <v-icon class="mF_icon" v-html="'sbf-btm-msg'"/>
                    <span class="mF_chat_unread" v-show="totalUnread > 0">{{totalUnread}}</span>
                </span>
            </v-btn>
            <v-btn :ripple="false" class="mF_btns" flat color="#69687d" value="profile" @click="changeActiveTab('profile')">
                <span class="mF_title" v-language:inner="'mobileFooter_btn_profile'"/>
                <v-icon class="mF_icon" v-html="'sbf-account'"/>
            </v-btn>
        </v-bottom-nav>
    </div>
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
export default {
    name: "mobileFooter",
    data() {
        return {
            lastTab:null,
            currentActiveTab:this.$route.name,
        }
    },
    computed: {
        ...mapGetters(['getSelectedClasses','getTotalUnread', 'accountUser','getSchoolName']),
        totalUnread(){
            return this.getTotalUnread
        },
        activeTab:{
            get(){
                if(this.$route.name === 'profile'){
                    if(!!this.accountUser){
                        if(this.$route.params.id == this.accountUser.id){
                           return this.$route.name;
                        }else{
                            setTimeout(() => {
                                return ''
                            }, 200);
                        }
                    }else{
                        setTimeout(() => {
                            return ''
                        }, 200);
                    }
                }
                else{
                    return this.currentActiveTab 
                }
            },
            set(tabName){
                let self = this;
                this.currentActiveTab = tabName;
                setTimeout(() => {
                    self.currentActiveTab = this.$route.name
                }, 200);
            }
        }
    },
    watch:{
        currentActiveTab(newVal,oldVal){
        
        }
    },
    methods: {
        ...mapActions(['updateDialogState','setReturnToUpload', 'updateLoginDialogState','openChatInterface']),
        openChat(){
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
            }else{
                this.openChatInterface();
            }
        },
        openUpload(){
            let schoolName = this.getSchoolName;
            if(this.accountUser == null) {
                this.updateLoginDialogState(true);
            } else if(!schoolName.length) {
                this.$router.push({name: 'addUniversity'});
                this.setReturnToUpload(true);
            } else if(!this.getSelectedClasses.length) {
                this.$router.push({name: 'addCourse'});
                this.setReturnToUpload(true);
            } else if(schoolName.length > 0 && this.getSelectedClasses.length > 0) {
                this.updateDialogState(true);
                this.setReturnToUpload(false);
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
                    this.updateLoginDialogState(true);
                }else{
                    let user = this.accountUser;
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
.v-bottom-nav{
    &.mobileFooter{
        box-shadow: none;
        border-top: solid 1px  #C7C7CD!important;
        .mF_btns{
            min-width: 50px;
            padding-top: 2px;
            .v-btn__content{
                margin-top: -2px;
            }
            .mF_title{
                font-size: 10px;
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
                    top: -6px;
                    right: -6px;
                    background: #ce3333;
                    color: #fff;
                    border-radius: 50%;
                    height: 13px;
                    width: 13px;
                    line-height: 13px;
                    display: flex;
                    font-size: 10px;
                    justify-content: center;
                    flex-direction: column;
                    text-align: center;
                }
            }
        }
    }
}
.v-item-group.v-bottom-nav .v-btn--active .v-btn__content {
    color: #5560ff;
}
.v-item-group.v-bottom-nav .v-btn--active {
    padding-top: 2px;
}
</style>