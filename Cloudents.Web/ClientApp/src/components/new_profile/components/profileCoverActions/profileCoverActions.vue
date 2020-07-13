    <template>
    <div class="profileCoverActions text-sm-center">
        <h1 dir="auto" class="mainTitle mb-sm-4 mb-2 white--text px-4">{{title}}</h1>
        <h2 dir="auto" class="subTitle white--text mb-sm-7 mb-5 px-4">{{paragraph}}</h2>
        <div class="mb-sm-5 actionWrapper text-center d-flex d-sm-block flex-wrap align-end">
            <v-btn
                @click="sendMessage"
                class="btn white--text me-sm-4 mb-sm-0"
                :width="isMobile ? '166' : '200'"
                height="46"
                color="#ff6927"
                rounded
                depressed
            >
                <chatIcon class="me-2" width="23" />
                <span class="flex-grow-1 flex-sm-grow-0 pe-sm-0" v-t="'message_me'"></span>
            </v-btn>
            <v-btn
                v-if="isCalendar && !isMyProfile"
                @click="openCalendar"
                class="btn white--text mt-4 mt-sm-0 mb-sm-0"
                :width="isMobile ? '166' : '200'"
                height="46"
                color="#4c59ff"
                rounded
                depressed
            >
                <calendarIcon class="me-2" width="23" />
                <span class="flex-grow-1 flex-sm-grow-0 pe-sm-0" v-t="'book_lesson'"></span>
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
        isCalendar() {
            return this.$store.getters.getProfileIsCalendar
        },
        isMyProfile() {
            return this.$store.getters.getIsMyProfile
        },
        title() {
            return this.$store.getters.getProfileTitle
        },
        user() {
            return this.$store.getters.getProfile
        },
        paragraph() {
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
            if(this.isMyProfile) return
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
            if(this.isMyProfile) return
            if(this.isLogged) {
                this.$emit('setCalendarActive', true)
                this.$nextTick(() => {
                    this.$vuetify.goTo(this.$parent.$refs.calendarTab)
                })
            } else {
                sessionStorage.setItem('calendar', true)
                this.$store.commit('setComponent', 'register')
                // setTimeout(()=>{
                //     document.getElementById(`tab-${this.activeTab}`).lastChild.click();
                // },200);
            }
        }
    },
    created() {
        if(this.$route.params.openCalendar || sessionStorage.getItem('calendar')) {
            this.openCalendar();
            sessionStorage.clear()
        }
    },
}
</script>

<style lang="less">
@import '../../../../styles/mixin.less';

.profileCoverActions {
    position: absolute;
    left: 0;
    right: 0;
    bottom: 16px;
    .mainTitle {
        text-shadow: 0 2px 8px rgba(0, 0, 0, 0.21);
        max-width: 753px;
        line-height: 1.2;
        margin: 0 auto;
        font-weight: 600;
        .responsive-property(font-size, 50px, null, 30px);
    }
    .subTitle {
        max-width: 565px;
        margin: 0 auto;
        font-weight: 500;
        .responsive-property(font-size, 20px, null, 18px);
    }
    .actionWrapper {
        justify-content: space-evenly;
        .btn {
            text-transform: none;
            font-size: 16px;
            font-weight: 600;
        }
    }
}
</style>