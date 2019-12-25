<template>
    <v-layout column wrap align-center v-if="tutorList.length >= 1" class="tutor-list-wrap px-2" :class="{'px-0 mx-2 mt-3': $vuetify.breakpoint.xsOnly}">
        <v-flex class="title-holder">
            <span @click="goToTutor()" class="subheading font-weight-bold tutors-title" v-language:inner="'tutorList_title'"></span>
        </v-flex>
        <v-flex>
            <tutor-result-card-other v-for="(singleTutor, index) in tutorList" :tutorData="singleTutor" :key="index" />
        </v-flex>
        <!-- <v-flex>
            <router-link to="/tutor" class="subheading font-weight-bold tutors-footer" v-language:inner="'documentPage_full_list'"></router-link>
        </v-flex> -->
    </v-layout>
</template>
<script>
    import {mapGetters, mapActions} from 'vuex';
    import tutorResultCardOther from "../../results/tutorCards/tutorResultCardOther/tutorResultCardOther.vue";

    export default {
        name: "tutorList",
        components: {tutorResultCardOther},
        computed: {
            ...mapGetters(['tutorList']),
            isMobile(){
                return this.$vuetify.breakpoint.xsOnly;
            }
        },
        watch: {
            '$route.query'() {
                let objReq;
                if(!!this.$route.query) {
                    let course = this.getCourseFromQuery();
                    objReq ={page: 0, courseName: course};
                }else{
                    objReq ={page: 0, courseName: ''};
                }
                this.getList(objReq);
            }
        },
        methods: {
            ...mapActions(['getTutorList', 'resetList']),
            getList(objReq){
                this.resetList();
                this.getTutorList(objReq);
            },
            goToTutor(){
                if(!this.isMobile){
                    this.$router.push({name:'tutors'});
                }
            },
            getCourseFromQuery(){
                if(!!this.$route.query) {
                    if(this.$route.query.hasOwnProperty('Course')){
                        return this.$route.query.Course;
                    }else if(this.$route.query.hasOwnProperty('term')){
                        return this.$route.query.term;
                    }
                }
            }
        },
        created(){
                let courseFromQuery =  this.getCourseFromQuery();
                let objReq ={page: 0, courseName: courseFromQuery};
                this.getList(objReq);
        },
        beforeDestroy(){
            this.resetList();
        }
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.tutor-list-wrap{
    border-radius: 4px;
      @media (min-width: @screen-xs) {
    // background-color: rgba(0, 0, 0, 0.04);
      }
    .title-holder{
        text-align: center;
        margin-bottom: 1px!important;
        width: 100%;
        padding: 10px 12px;
    }
    .tutors-title{
        color: @global-purple;
        vertical-align: middle;
        line-height: 42px;
        cursor: pointer;
    }
    .tutors-footer {
        color: #4452fc;
    }
}
</style>