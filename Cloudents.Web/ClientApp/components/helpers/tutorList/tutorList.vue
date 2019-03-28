<template>
    <v-layout column wrap>
        <v-flex>
            <span class="subheading">Course Tutors </span>
        </v-flex>
        <v-flex>
            <tutorCard v-for="tutor in 10" :tutorData="tutor"></tutorCard>
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
                    score: 650
                }
            };
        },
        computed: {
            // ...mapGetters([]);
        },
    };
</script>

<style lang="less">

</style>