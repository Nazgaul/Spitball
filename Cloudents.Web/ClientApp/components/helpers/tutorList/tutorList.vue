<template>
    <v-layout column wrap align-center v-if="tutorList.length >= 1" class="tutor-list-wrap px-2" :class="{'px-0 mx-2 mt-3': $vuetify.breakpoint.xsOnly}">
        <v-flex class="title-holder">
            <span @click="goToTutor()" class="subheading font-weight-bold tutors-title" v-language:inner>tutorList_title</span>
        </v-flex>
        <v-flex>
            <tutorCard v-for="singleTutor in tutorList" :tutorData="singleTutor" :isInTutorList="true"></tutorCard>
        </v-flex>
    </v-layout>
</template>
<script>
    import {mapGetters, mapActions} from 'vuex';
    import tutorCard from "../../results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue";
    export default {
        name: "tutorList",
        components: {tutorCard},
        computed: {
            ...mapGetters(['tutorList']),
            isMobile(){
                return this.$vuetify.breakpoint.xsOnly;
            }
        },
        watch: {
            '$route.query'(val) {
                let objReq;
                if(!!this.$route.query && this.$route.query.hasOwnProperty('Course')) {
                    let courseInFilter =  this.$route.query.Course;
                    objReq ={page: 0, courseName: courseInFilter};
                }else{
                    objReq ={page: 0, courseName: ''};
                }
                this.getList(objReq);
            }
        },
        methods: {
            ...mapActions(['getTutorList', 'resetList']),
            getList(objReq){
                this.resetList([]);
                this.getTutorList(objReq);
            },
            goToTutor(){
                if(!this.isMobile){
                    this.$router.push({name:'tutors'});
                }
            }
        },
        created(){
                let courseInFilter =  this.$route.query.Course ? this.$route.query.Course : '';
                let objReq ={page: 0, courseName: courseInFilter};
                this.getList(objReq);
        }
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.tutor-list-wrap{
    border-radius: 4px;
      @media (min-width: @screen-xs) {
    background-color: rgba(0, 0, 0, 0.04);
      }
    .title-holder{
        text-align: center;
        margin-bottom: 1px!important;
        width: 100%;
        padding: 10px 12px;
    }
    .tutors-title{
        color: @profileTextColor;
        vertical-align: middle;
        line-height: 42px;
        cursor: pointer;
    }
}
</style>