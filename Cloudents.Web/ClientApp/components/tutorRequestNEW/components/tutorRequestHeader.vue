<template>
    <div class="tutorRequest-top">

        <user-avatar class="tutorRequest-img"
                     v-if="isCurrentTutor" :size="'66'" 
                     :user-name="getCurrTutor.name" 
                     :user-id="getCurrTutor.userId" 
                     :userImageUrl="getCurrTutor.image"/>

        <img v-else class="tutorRequest-img" src="../images/yaniv.jpg" alt="../images/yaniv.jpg">

        <p v-if="!getCurrTutor" v-language:inner="'tutorRequest_send_msg_yaniv'"/>
        <p v-else v-html="$Ph(isMobile? 'tutorRequest_send_msg_tutor_mobile' :'tutorRequest_send_msg_tutor',this.getCurrTutor.name)" />
    </div>
</template>

<script>
import { mapGetters } from 'vuex';
import userAvatar from '../../helpers/UserAvatar/UserAvatar.vue'

export default {
    components:{userAvatar},
    computed: {
        ...mapGetters(['getCurrTutor']),
        // currentTutorImg(){
        //     if(!!this.getCurrTutor && this.getCurrTutor.image){
        //         return this.getCurrTutor.image
        //     } else{
        //         return require('../images/yaniv.jpg')
        //     }
        // },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isCurrentTutor(){
            return !! this.getCurrTutor
        }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutorRequest-top{
        display: flex;
        flex-direction: column;
        align-items: center;
        .tutorRequest-img{
            border-radius: 50%;
            width: 66px;
            height: 66px;
            object-fit: cover;
            @media (max-width: @screen-xs) {
                margin-top: 30px;
            }
        }
        p{
            margin-top: 8px;
            font-size: 20px;
            font-weight: 600;
            color: @global-purple;
            @media (max-width: @screen-xs) {
                margin-top: 18px;
                text-align: center;
                line-height: 1.5;
                letter-spacing: -0.38px;
                padding: 0 40px;
                white-space: pre-line;
            }
        }

    }
</style>