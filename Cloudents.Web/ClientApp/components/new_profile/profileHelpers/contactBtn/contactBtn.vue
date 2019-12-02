<template>
    <div class="contact-btn-wrap">

        <button class="ct-btn" @click="sendMessage">
            <v-icon class="ct-btn-icon mr-2">sbf-message-icon</v-icon>  
            <span class="btn-text text-uppercase" v-language:inner>profile_tutor_contact_btn</span>
        </button>
    </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
import chatService from '../../../../services/chatService'
import analyticsService from '../../../../services/analytics.service';

    export default {
        name: "contactBtn",
        computed:{
            ...mapGetters(['accountUser']),
        },
        methods:{
            ...mapActions(['setActiveConversationObj', 'openChatInterface','updateRequestDialog','updateCurrTutor', 'setTutorRequestAnalyticsOpenedFrom']),
            ...mapGetters(['getProfile']),

            sendMessage(){
                
                if ( this.accountUser == null) {
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
                    let profile = this.getProfile()
                    this.updateCurrTutor(profile.user)    
                    this.setTutorRequestAnalyticsOpenedFrom({
                        component: 'profileContactBtn',
                        path: this.$route.path
                    });
                    this.updateRequestDialog(true);
                } else {
                    analyticsService.sb_unitedEvent('Tutor_Engagement', 'contact_BTN_profile_page', `userId:${this.accountUser.id}`);
                    analyticsService.sb_unitedEvent('Request_Tutor_Submit', 'Send_Chat_Message', `${this.$route.path}`);
                    let currentProfile = this.getProfile();
                    let conversationObj = {
                        userId: currentProfile.user.id,
                        image: currentProfile.user.image,
                        name: currentProfile.user.name,
                        conversationId: chatService.createConversationId([currentProfile.user.id, this.accountUser.id]),
                    }
                    let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
                    this.setActiveConversationObj(currentConversationObj);
                    let isMobile = this.$vuetify.breakpoint.smAndDown;
                    this.openChatInterface();                    
                }
            }
        },
    }
</script>

<style lang="less">
    @import '../../../../styles/mixin.less';
    .contact-btn-wrap {
        @media(max-width: @screen-xs){
            margin-top: 2px;
            margin-bottom: 6px;
        }
        .ct-btn {
            display: flex;
            align-items: center;
            justify-content: center;
            /*padding: 14px 42px 14px 34px;*/
            padding: 14px 60px 14px 50px;
            background-color: #ffca54;
            height: 48px;
            border-radius: 24px;
            box-shadow: 0 3px 13px 0 rgba(0, 0, 0, 0.15);
            outline: none;
            white-space: pre;
            &:hover{
                cursor: pointer;
                color: lighten(#ffca54, 10%);
                .btn-text{
                    color: lighten(@global-purple, 10%);
                }
            }
        }
        .btn-text{
            font-size: 16px;
            font-weight: bold;
            color: @global-purple;
        }
        .ct-btn-icon{
            color: @global-purple;
            font-size: 19px;
            padding-top: 5px;
        }
    }

</style>