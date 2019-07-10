<template>
    <v-layout column class="aside-bottom mb-5">
        <div class="justify-space-between more-tutors pb-3">
            <span class="font-weight-bold" v-language:inner="'documentPage_more_tutors'"></span>
            <router-link v-language:inner="'documentPage_see_all'" to="/tutor"></router-link>
        </div>

        <div v-for="(tutor, index) in tutorList" :key="index">
            <tutor-result-card-mobile :tutorData="tutor" :isInTutorList="true" />
        </div>
    </v-layout>
</template>

<script>
import tutorResultCardMobile from '../../../components/results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import { mapGetters, mapActions } from 'vuex';

export default {
    components: {
        tutorResultCardMobile
    },
    methods:{
        ...mapActions(['getTutorListCourse']),
    },
    computed: {
        ...mapGetters(['getTutorList']),

        tutorList() {
            return this.getTutorList;           
        },
    },
    created() {      
        let course = this.$route.params.courseName.replace(/-/g, ' '); 
        this.getTutorListCourse(course)
    }
}
</script>
<style lang="less">
    .aside-bottom {
        order: 3;
        .more-tutors {
            display: flex;
            span {
                font-size: 15px;
            }
        }
    }
</style>