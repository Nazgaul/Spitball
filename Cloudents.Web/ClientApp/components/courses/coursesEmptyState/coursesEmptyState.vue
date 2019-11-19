<template>
    <div class="empty-courses-wrap">
        <v-layout row class="py-4" :class="[$vuetify.breakpoint.smAndUp ?  'px-4': 'px-3']" align-center justify-center>
            <v-flex grow xs8 sm10 >
                <div class="d-inline-flex justify-center shrink">
                    <span class="subtitle-1 font-weight-bold" v-language:inner>courses_my_courses</span>
                </div>
            </v-flex>
            <v-flex xs4 sm2  shrink class="justify-end d-inline-flex">
                <v-btn rounded color="#4452FC" class="add-btn py-1 px-0 font-weight-bold my-0" @click="goToAddCourses()">
                    <v-icon class="mr-1">sbf-plus-regular</v-icon>
                    <span v-language:inner>courses_add</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout row wrap align-center justify-center>
            <v-flex xs12   class="text-center mb-2">
                <span class="subtitle-1 font-weight-bold dark-text" v-language:inner>courses_empty_pretty</span>
            </v-flex>
            <v-flex xs12  class="text-center mb-2">
                <span class="text-small" v-language:inner>courses_start_adding</span>
            </v-flex>
            <v-flex xs12  class="text-center mb-2">
                <span class="text-small" v-language:inner>courses_out_of</span>
            </v-flex>
            <v-flex xs12  class="text-center mt-3">
                <img src="../images/courses-empty-image.png" alt="Empty courses">
            </v-flex>
        </v-layout>
        <v-layout v-if="$vuetify.breakpoint.xsOnly" class="pt-5" align-center justify-center>
            <v-flex xs12  class="text-center">
                <finishBtn></finishBtn>
            </v-flex>
        </v-layout>

    </div>
</template>

<script>
    import finishBtn from '../helpers/finishBtn.vue';

    export default {
        name: "coursesEmptyState",
        components: {finishBtn},
        data() {
            return {};
        },
        methods: {
            goToAddCourses() {
                this.$router.push({name: 'addCourse'});
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .empty-courses-wrap {
        min-height: 764px;
        .add-btn {
            color: @color-white;
        }
        .text-small {
            font-size: 14px;
            color: @colorBlackNew;
        }
        .dark-text {
            font-size: 18px;
            color: @textColor;
        }
    }

</style>