<template>
    <div class="tutorRequest-container">
        <tutorRequestHeader v-if="!showHeader"></tutorRequestHeader>
        <component :is="currentStep"></component> 
    </div>
</template>

<script>
import { mapGetters } from 'vuex';

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
        ...mapGetters(['accountUser','getCurrentTutorReqStep']),
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
}
</script>

<style lang="less">
@import '../../styles/mixin.less';
    .tutorRequest-container{
        width: 510px;
        height: 440px;
        background-color: #ffffff;
        border-radius: 6px;
        box-shadow: 0 13px 21px 0 rgba(0, 0, 0, 0.51);
        padding: 14px 18px 10px 18px;
        display: flex;
        flex-direction: column;
        align-items: center;
            @media (max-width: @screen-xs) {
                width: 100%;
                height: 100%;
            }
    }

</style>