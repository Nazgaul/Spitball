<template>
    <div class="tutorRequest-container" :class="{'tutorRequest-container--paddingTop': !showHeader}">
        <tutorRequestHeader v-if="!showHeader"></tutorRequestHeader>
        <component :is="currentStep"></component> 
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import analyticsService from '../../services/analytics.service';


// cmps:
import tutorRequestHeader from './components/tutorRequestHeader.vue'
import tutorRequestCourseInfo from './components/tutorRequestCourseInfo.vue'
import tutorRequestUserInfo from './components/tutorRequestUserInfo.vue'
import tutorRequestSuccess from './components/tutorRequestSuccess.vue'


export default {
    components:{
        tutorRequestHeader,
        tutorRequestCourseInfo,
        tutorRequestUserInfo,
        tutorRequestSuccess
    },
    computed:{
        ...mapGetters(['accountUser','getCurrentTutorReqStep','getTutorRequestAnalyticsOpenedFrom']),
        isLoggedIn(){
            return !!this.accountUser
        },
        currentStep(){
            return this.getCurrentTutorReqStep
        },
        showHeader(){
            return this.currentStep === 'tutorRequestSuccess'
        }
    },
    created() {
        let analyticsObject = {
                userId: this.isAuthUser ? this.accountUser.id : 'GUEST',
                course: '',
                fromDialogPath: this.getTutorRequestAnalyticsOpenedFrom.path,
                fromDialogComponent: this.getTutorRequestAnalyticsOpenedFrom.component
            };
        analyticsService.sb_unitedEvent('Request Tutor Dialog Opened', `${analyticsObject.fromDialogPath}-${analyticsObject.fromDialogComponent}`, `USER_ID:${analyticsObject.userId}, T_Course:${analyticsObject.course}`);
    },
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
    .tutorRequest-container{
        width: 510px;
        background-color: #ffffff;
        border-radius: 6px;
        box-shadow: 0 13px 21px 0 rgba(0, 0, 0, 0.51);
        padding: 0px 18px 10px 18px;
        display: flex;
        flex-direction: column;
        align-items: center;
        overflow: hidden;
            @media (max-width: @screen-xs) {
                width: 100%;
                height: 100%;
                box-shadow: none;
            }
        &--paddingTop {
            padding-top: 20px;   
        }
    }
    
</style>