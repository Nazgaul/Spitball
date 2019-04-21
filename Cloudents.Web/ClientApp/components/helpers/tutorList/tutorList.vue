<template>
    <v-layout column wrap align-center v-if="tutorList.length >= 1" class="tutor-list-wrap" :class="{'mx-2 mt-3': $vuetify.breakpoint.xsOnly}">
        <v-flex class="title-holder">
            <span class="subheading font-weight-bold tutors-title" v-language:inner>tutorList_title</span>
        </v-flex>
        <v-flex>
            <tutorCard v-for="singleTutor in tutorList" :tutorData="singleTutor"></tutorCard>
        </v-flex>
    </v-layout>
</template>
<script>
    import {mapGetters, mapActions} from 'vuex';
    import tutorCard from "./tutorCard.vue";
    export default {
        name: "tutorList",
        components: {tutorCard},
        data() {
            return {
            };
        },
        computed: {
            ...mapGetters(['tutorList']),
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
                this.getTutorList(objReq);
            }
        },
        methods: {
            ...mapActions(['getTutorList'])
        },
        created(){
                let courseInFilter =  this.$route.query.Course ? this.$route.query.Course : '';
                let objReq ={page: 0, courseName: courseInFilter};
                this.getTutorList(objReq);
        }
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.tutor-list-wrap{
    .title-holder{
        text-align: center;
        background: #fff;
        border-radius: 4px 4px 0 0;
        margin-bottom: 1px!important;
        width: 100%;
    }
    .tutors-title{
        color: @profileTextColor;
        vertical-align: middle;
        line-height: 42px;
    }
}
</style>