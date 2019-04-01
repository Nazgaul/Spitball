<template>
    <div class="empty-courses-wrap">
        <v-layout row class="py-4 px-4" align-center justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <span class="subheading font-weight-bold">My Courses </span>
                    <span class="subheading font-weight-bold" v-if="coursesQuantaty">({{coursesQuantaty}})</span>
                </div>
            </v-flex>
            <v-flex xs2 shrink class="justify-end">
                <v-btn round  color="#4452FC" class="add-btn py-1 font-weight-bold my-0" @click="goToAddCourses()">
                    <v-icon left>sbf-plus-regular</v-icon>
                    <span>Add</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout row wrap align-center justify-center>
            <v-flex xs12 sm12 md12 class="text-xs-center mb-2">
                <span class="subheading font-weight-bold dark-text">Its Pretty empty here…</span>
            </v-flex>
            <v-flex xs12 sm12 md12 class="text-xs-center mb-2">
                <span class="text-small">Start Adding Courses To Get The Most</span>
            </v-flex>
            <v-flex xs12 sm12 md12 class="text-xs-center mb-2">
                <span class="text-small">Out of Spitball</span>
            </v-flex>
            <v-flex xs12 sm12 md12 class="text-xs-center mt-3">
                <img src="../images/courses-empty-image.png" alt="Empty courses">
            </v-flex>
        </v-layout>

    </div>
</template>

<script>
    export default {
        name: "coursesEmptyState",
        data() {
            return {

            };
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
    .empty-courses-wrap{
        .add-btn{
            color: @color-white;
        }
        .text-small{
            font-size: 14px;
            color: @colorBlackNew;
        }
        .dark-text{
            font-size: 18px;
            color: @textColor;
        }
    }

</style>