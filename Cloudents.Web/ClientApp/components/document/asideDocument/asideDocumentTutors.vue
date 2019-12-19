<template>
    <v-layout column class="aside-bottom mb-4" v-show="tutorList.length">
        <div class="justify-space-between more-tutors">
            <div class="font-weight-bold mb-2" v-language:inner="'documentPage_more_tutors'"></div>
        </div>
        <div v-for="(tutor, index) in tutorList" :key="index">
            <tutor-result-card-other :tutorData="tutor" />
        </div>
    </v-layout>
</template>

<script>
import tutorResultCardOther from '../../../components/results/tutorCards/tutorResultCardOther/tutorResultCardOther.vue';
import {  mapActions } from 'vuex';

export default {
    props:{
        courseName:{
            type:String,
            required:true
        },
        tutorList:{
            type: Array,
        }
    },
    components: {
        tutorResultCardOther
    },
    methods:{
        ...mapActions(['getTutorListCourse']),
    },
    created() {
        if(this.$vuetify.breakpoint.mdAndUp) {
            this.getTutorListCourse(this.courseName)
        }
    }
}
</script>
<style lang="less">
@import '../../../styles/mixin.less';
    .aside-bottom {
        order: 3;
        .more-tutors {
            display: flex;
            margin: 22px auto 0 auto;
            margin-bottom: 10px;

            div {
                font-size: 18px;
                color: @global-purple;
            }
        }
    }
</style>