<template>
    <v-layout column class="aside-bottom mb-4">
        <div class="justify-space-between more-tutors">
            <div class="font-weight-bold mb-2" v-language:inner="'documentPage_more_tutors'"></div>
        </div>

        <div v-for="(tutor, index) in tutorList" :key="index">
            <tutor-result-card-other :tutorData="tutor" />
        </div>
        
        <v-flex class="footer-holder text-xs-center mt-2 mb-5" v-if="$vuetify.breakpoint.smAndDown">
            <router-link to="/tutor" class="subheading font-weight-bold tutors-footer" v-language:inner="'documentPage_full_list'"></router-link>
        </v-flex>
    </v-layout>
</template>

<script>
import tutorResultCardMobile from '../../../components/results/tutorCards/tutorResultCardMobile/tutorResultCardMobile.vue';
import tutorResultCardOther from '../../../components/results/tutorCards/tutorResultCardOther/tutorResultCardOther.vue';

import { mapGetters, mapActions } from 'vuex';

export default {
    components: {
        tutorResultCardMobile,
        tutorResultCardOther
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
        if(this.$vuetify.breakpoint.mdAndUp) {
            let course = this.$route.params.courseName.replace(/-/g, ' '); 
            this.getTutorListCourse(course)
        }
    }
}
</script>
<style lang="less">
    .aside-bottom {
        order: 3;
        .more-tutors {
            display: flex;
            margin: 22px auto 0 auto;
            margin-bottom: 10px;

            div {
                font-size: 18px;
                color: #43425d;
            }
            .seeAll {
                color: #4452fc;
            }
        }
        .footer-holder {
            a {
                color: #4452fc;
            }
            
        }
    }
</style>