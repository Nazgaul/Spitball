<template>
    <div class="tutorRequest-top">
        <v-progress-circular v-if="!isLoaded" indeterminate v-bind:size="66" width="3" color="info"></v-progress-circular>
        <user-avatar class="tutorRequest-img"
                v-if="isCurrentTutor && isLoaded" :size="'66'" 
                :user-name="getCurrTutor.name"  
                :userImageUrl="getCurrTutor.image"/>
       
            <img v-else  class="tutorRequest-img" v-show="isLoaded"  @load="loaded" :src="defaultImage" alt="Yaniv Image" width="66" height="66">
      
        <p class="text-center" v-if="!getCurrTutor" v-t="'tutorRequest_send_msg_yaniv'"/>
        <p class="text-center" v-else>{{getText}}</p>
    </div>
</template>

<script>
import { mapGetters } from 'vuex';

export default {
    data() {
        return {
            isLoaded: false,
        }
    },
    methods: {
        loaded() {
            this.isLoaded = true;
        }
    },
    computed: {
        ...mapGetters(['getCurrTutor', 'isFrymo']),
        getText() {
            if (this.isMobile) {
              return  this.$t('tutorRequest_send_msg_tutor_mobile', [ this.getCurrTutor.name] )
            }
            return this.$t('tutorRequest_send_msg_tutor' [this.getCurrTutor.name])
        },
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        },
        isCurrentTutor(){
            return !! this.getCurrTutor
        },
        defaultImage() {
            if(this.isFrymo) {
                return require('../images/indiaGuy.jpeg');
            }
            return require('../images/yaniv.jpg');
        }
    },
}
</script>

<style lang="less">
@import '../../../styles/mixin.less';
    .tutorRequest-top{
       // display: flex;
       // flex-direction: column;
       // align-items: center;
        width: 100%;
       // line-height: normal;
        .tutorRequest-img{
            display: block; //to honor the margin
            border-radius: 50%;
            width: 66px;
            height: 66px;
            object-fit: cover;
            margin: 10px auto 0;

        }
        p{
            margin-top: 8px;
            font-size: 20px;
            font-weight: 600;
            color: @global-purple;
            width: 100%;
            @media (max-width: @screen-xs) {
                margin-top: 18px;
                text-align: center;
                line-height: 1.5;
                letter-spacing: -0.38px;
                padding: 0 20px;
                white-space: pre-line;
            }
        }

    }
</style>