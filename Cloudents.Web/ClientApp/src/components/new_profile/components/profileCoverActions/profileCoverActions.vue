    <template>
    <div class="profileCoverActions text-center">
        <h1 class="mainTitle mb-2 white--text">{{description}}</h1>
        <h2 class="subTitle white--text">{{bio}}</h2>

        <div class="mt-5">
            <v-btn class="btn white--text me-3" @click="sendMessage" rounded depressed color="#ff6927" width="200" height="46" :block="isMobile">
                <chatIcon class="me-2" width="23" />
                <span v-t="'message_me'"></span>
            </v-btn>
            <v-btn class="btn white--text" @click="openCalendar" rounded depressed color="#4c59ff" width="200" height="46" :block="isMobile">
                <calendarIcon class="me-2" width="23" />
                <span v-t="'book_lesson'"></span>
            </v-btn>
        </div>
    </div>
</template>

<script>
import chatService from '../../../../services/chatService.js';
import { MessageCenter } from '../../../../routes/routeNames.js';

import chatIcon from './chat.svg'
import calendarIcon from './calendar.svg'

export default {
    name: 'profileCoverActions',
    components: {
        chatIcon,
        calendarIcon
    },
    computed: {
        description() {
            return this.$store.getters.getProfileDescription
        },
        user() {
            return this.$store.getters.getProfile
        },
        bio() {
            return this.$store.getters.getProfileBio
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        }
    },
    methods: {
        sendMessage() {
            if(this.isMyProfile) {return}
            if(!this.isLogged) {
                let profile = this.user
                this.$ga.event('Tutor_Engagement', 'contact_BTN_profile_page', `userId:GUEST`);
                this.$store.dispatch('updateCurrTutor', profile.user)
                this.$store.dispatch('setTutorRequestAnalyticsOpenedFrom', {
                    component: 'profileContactBtn',
                    path: this.$route.path
                });
                this.$store.dispatch('updateRequestDialog', true);
            } else {
                this.$ga.event('Request Tutor Submit', 'Send_Chat_Message', `${this.$route.path}`);
                let currentProfile = this.user;
                let conversationObj = {
                    userId: currentProfile.user.id,
                    image: currentProfile.user.image,
                    name: currentProfile.user.name,
                    conversationId: chatService.createConversationId([currentProfile.user.id, this.$store.getters.accountUser.id]),
                }
                let isNewConversation = !(this.$store.getters.getIsActiveConversationTutor(conversationObj.conversationId))
                if(isNewConversation){
                    let tutorInfo = {
                    id: currentProfile.user.id,
                    name: currentProfile.user.name,
                    image: currentProfile.user.image,
                    calendar: currentProfile.user.calendarShared,
                    }
                    this.$store.commit('ACTIVE_CONVERSATION_TUTOR', { tutorInfo, conversationId:conversationObj.conversationId })
                }
                let currentConversationObj = chatService.createActiveConversationObj(conversationObj)
                this.$store.dispatch('setActiveConversationObj', currentConversationObj);
                this.$router.push({name: MessageCenter, params: { id:currentConversationObj.conversationId }})
            }
        },
        openCalendar() {
            if(this.isLogged) {
                this.$emit('setCalendarActive', true)
                // this.activeTab = 5;
                this.$nextTick(() => {
                    this.$vuetify.goTo(this.$parent.$refs.calendarTab)
                })
            } else {
                this.$store.commit('setComponent', 'register')
                // setTimeout(()=>{
                //     document.getElementById(`tab-${this.activeTab}`).lastChild.click();
                // },200);
            }
        }
    },
    created() {
        if(this.$route.params.openCalendar) {
            this.openCalendar();
        }
    },
}
</script>

<style lang="less">
.profileCoverActions {
    position: absolute;
    left: 0;
    right: 0;
    top: 350px;
    .mainTitle {
        font-size: 44px;
        font-weight: 600;
    }
    .subTitle {
        max-width: 450px;
        margin: 0 auto;
        font-size: 16px;
        font-weight: 600;
    }
    .btn {
        font-size: 16px;
        font-weight: 600;
    }
}
</style>