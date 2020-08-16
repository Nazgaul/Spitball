<template>
    <div class="profileCoverActions text-sm-center">
        <h1 dir="auto" class="mainTitle mb-sm-4 mb-2 white--text px-4">{{title}}</h1>
        <h2 dir="auto" class="subTitle white--text mb-sm-7 mb-5 px-4">{{paragraph}}</h2>
        <div class="mb-sm-5 actionWrapper text-center d-flex d-sm-block flex-wrap align-end">
            <v-btn
                class="btn white--text me-sm-4 mb-sm-0"
                v-if="isCalendar && !isMyProfile"
                @click="openCalendar"
                :width="isMobile ? '166' : '200'"
                height="46"
                color="#4c59ff"
                rounded
                depressed
            >
                <calendarIcon class="me-2" width="23" />
                <span class="flex-grow-1 flex-sm-grow-0 pe-sm-0" v-t="'book_lesson'"></span>
            </v-btn>
            <v-btn
                :loading="isLoading"
                @click="toggleFollowing"
                class="btn white--text mt-4 mt-sm-0 mb-sm-0"
                :width="isMobile ? '166' : '200'"
                height="46"
                color="#ff6927"
                rounded
                depressed>
                <v-icon size="18" class="me-0 me-sm-2">sbf-person</v-icon>
                <span class="flex-grow-1 flex-sm-grow-0 pe-sm-0">{{isFollowing ? $t('unfollow_me') : $t('follow_me')}}</span>
            </v-btn>
        </div>
    </div>
</template>

<script>

import calendarIcon from './calendar.svg'

export default {
    name: 'profileCoverActions',
    data() {
        return {
            isLoading:false,
        }
    },
    components: {
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
        paragraph() {
            return this.$store.getters.getProfileBio
        },
        isMobile() {
            return this.$vuetify.breakpoint.xsOnly
        },
        isLogged() {
            return this.$store.getters.getUserLoggedInStatus
        },
        isFollowing(){
            return this.$store.getters.getIsProfileFollowing
        }
    },
    methods: {
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
        },
        toggleFollowing(){
            if(this.isMyProfile) return;
            if(this.isLogged){
                if(this.isLoading) return;
                let self = this;
                this.isLoading = true;
                this.$store.dispatch('toggleProfileFollower',!this.isFollowing).then(()=>{
                    self.isLoading = false;
                })
            }else{
                this.$store.commit('setComponent', 'register')
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