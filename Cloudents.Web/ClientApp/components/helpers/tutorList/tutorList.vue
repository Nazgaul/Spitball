<template>
    <v-layout column wrap align-center class="tutor-list-wrap" :class="{'mx-2': $vuetify.breakpoint.xsOnly}">
        <v-flex class="mb-3">
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
        methods: {
            ...mapActions(['getTutorList'])
        },
        created(){
                this.getTutorList();
        }
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';
.tutor-list-wrap{
    .tutors-title{
        color: @profileTextColor;
    }
}
</style>