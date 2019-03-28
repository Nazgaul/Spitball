<template>
    <v-layout column wrap align-center class="tutor-list-wrap" :class="{'mx-2': $vuetify.breakpoint.xsOnly}">
        <v-flex class="mb-3">
            <span class="subheading font-weight-bold tutors-title" v-language:inner>tutorList_title</span>
        </v-flex>
        <v-flex>
            <tutorCard v-for="singleTutor in 10" :tutorData="tutor"></tutorCard>
        </v-flex>
    </v-layout>
</template>

<script>
    import {mapGetters} from 'vuex';
    import tutorCard from "./tutorCard.vue";
    export default {
        name: "tutorList",
        components: {tutorCard},
        data() {
            return {
                tutor:{
                    name : 'Jonathan haisenson',
                    text : 'Course name 1, Course name 2, Course name 3, Course name 4, Course name 5',
                    price : '100.00',
                    score: 650,
                    rating: 4.5
                }
            };
        },
        computed: {
            // ...mapGetters([]);
        },
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