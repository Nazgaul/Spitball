<template>
    <div class="courses-list-wrap">
        <div v-if="!isEmpty">
            <v-layout class="py-6 pl-6 pr-4" align-center justify-center>
                <v-flex grow>
                    <div class="d-inline-flex justify-center shrink courses-list-wrap-title">
                        <span class="font-weight-bold" v-language:inner>courses_my_courses</span>
                        <span class="font-weight-bold" v-if="coursesQuantaty">&nbsp;({{coursesQuantaty}})</span>
                    </div>
                </v-flex>
                <v-flex xs2 shrink class="text-center hidden-xs-only" >
                    <finishBtn></finishBtn>
                </v-flex>
                <v-flex shrink class="d-flex justify-start">
                    <v-btn sel="add_courses_button" rounded color="#4452FC" class="add-btn py-1 my-0 elevation-0"
                           :class="{'mr-2': $vuetify.breakpoint.xsOnly }"
                           @click="goToAddMore()">
                        <v-icon class="mr-1 vicon">sbf-plus-regular</v-icon>
                        <span v-language:inner>courses_add</span>
                    </v-btn>
                </v-flex>
            </v-layout>
            <v-layout align-center>
                <v-flex class="search-classes-container">
                    <div class="class-list search-classes-list">
                        <div class="list-item search-class-item py-2 mx-2 justify-space-between align-center font-weight-regular"
                             v-for="(singleClass, index) in classesSelected" :key="index">
                            <v-layout column class="pl-6 limit-width">
                                <v-flex shrink class="text-truncate course-name-wrap">
                                    {{ singleClass.text }}
                                </v-flex>
                                <v-flex class="label-text pt-1" v-if="singleClass.isPending">
                                    <span v-language:inner>courses_pending</span>
                                    <span class="d-inline-flex badge font-weight-bold px-2 align-center justify-center ml-1"
                                          v-language:inner>courses_new</span>
                                </v-flex>
                                <v-flex class="label-text  pt-1" v-else>
                                    {{singleClass.students}}
                                    <span class="label-text" v-language:inner>courses_students</span>
                                </v-flex>
                            </v-layout>

                            <v-layout align-center justify-end class="pr-2 grow">
                                <v-flex shrink class="d-flex align-center" v-if="!singleClass.isLoading">
                                    <div v-show="isUserTutor">
                                    <v-btn v-if="!singleClass.isTeaching" rounded @click="teachCourseToggle(singleClass)"
                                           :loading="singleClass.isLoading && teachingActive"
                                           class="outline-btn elevation-0 text-none align-center justify-center rounded-btn">
                                        <span>
                                            <v-icon color="#a3a0fb" class="btn-icon mr-1">sbf-face-icon</v-icon>
                                            <span class="purple-text caption" v-html="$Ph('courses_teach')"></span>
                                        </span>
                                    </v-btn>

                                    <v-btn v-else rounded @click="teachCourseToggle(singleClass)"
                                           class="solid-btn elevation-0 text-none align-center justify-center rounded-btn">
                                        <span>
                                            <v-icon class="btn-icon mr-1">sbf-checkmark</v-icon>
                                            <span class="caption" v-html="$Ph('courses_teaching')"></span>
                                        </span>
                                    </v-btn>
                                    </div>
                                    <span>
                                        <v-icon @click="removeClass(singleClass)" v-show="!singleClass.isLoading" class="delete-sbf-icon">sbf-delete-outline</v-icon>
                                    </span>
                                </v-flex>
                                <v-flex v-else shrink class="d-flex align-center px-5">
                                    <v-progress-circular
                                            indeterminate
                                            :width="2"
                                            :size="24"
                                            color="primary"
                                            v-show="singleClass.isLoading"

                                    ></v-progress-circular>
                                </v-flex>
                            </v-layout>
                        </div>
                    </div>
                </v-flex>
            </v-layout>
            <v-layout  align-center justify-center class="hidden-sm-and-up fixed-bottom-wrap elevation-2">
                <v-flex xs12 class="text-center pt-4">
                    <finishBtn></finishBtn>
                </v-flex>
            </v-layout>
        </div>
        <div v-else>
            <courses-empty-state></courses-empty-state>
        </div>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex';
    import coursesEmptyState from '../coursesEmptyState/coursesEmptyState.vue';
    import universityService from '../../../services/universityService';
    import finishBtn from  '../helpers/finishBtn.vue';
    export default {
        name: "selectedCourses",
        components: {coursesEmptyState, finishBtn},
        data() {
            return {
                btnLoading: false,
                removingActive: false,
                teachingActive: false
            };
        },
        computed: {
            ...mapGetters(['getSelectedClasses', 'accountUser', 'getIsTutorState', 'getIsSelectedClassLocked']),
            classesSelected() {
                return this.getSelectedClasses;
            },
            isUserTutor(){
                return this.accountUser.isTutor || (this.getIsTutorState && this.getIsTutorState === 'pending');            
            },
            isEmpty() {
                if(!this.getIsSelectedClassLocked){
                    if(this.getSelectedClasses.length < 1){
                        this.goToAddMore();
                    }
                }
                return false;
            },
            coursesQuantaty() {
                return this.getSelectedClasses.length;
            }

        },
        methods: {
            ...mapActions(["updateClasses",
                              "syncCoursesData",
                              "deleteClass",
                              "updateSelectedClasses",
                              "pushClassToSelectedClasses",
                          ]),
            teachCourseToggle(course) {
                this.teachingActive = true;
                course.isLoading = true;
                universityService.teachCourse(course.text)
                                 .then(() => {
                                     course.isLoading = false;
                                     this.teachingActive = false;
                                     return course.isTeaching = !course.isTeaching;
                                 }, () => {
                                     course.isLoading = false;
                                     this.teachingActive = false;
                                 }).finally(() => {
                    course.isLoading = false;
                    this.teachingActive = false;
                });
            },
            removeClass(classDelete) {
                classDelete.isLoading = true;
                this.removingActive = true;
                this.deleteClass(classDelete).then(() => {
                    classDelete.isLoading = false;

                }, () => {
                    classDelete.isLoading = false;
                    this.removingActive = false;
                }).finally(() => {
                    classDelete.isLoading = false;
                    this.removingActive = false;
                });
            },
            goToAddMore() {
                this.$router.push({name: 'addCourse'});
            }
        },

    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .courses-list-wrap {
        .scrollBarStyle(6px, #a2a2a9, inset 0 0 0px,  inset 0 0 0px);
        .rounded-btn {
            margin: 6px 8px;
            border-radius: 36px!important; //vuetify
            min-width: 108px !important; //vuetify
        }
        .fixed-bottom-wrap{
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background-color: @color-white;
        }
        .badge {
            max-height: 48px;
            font-size: 12px;
            padding: 2px 0;
            border-radius: 8px;
            background-color: @purpleLight;
            color: @color-white;
        }
        .courses-list-wrap-title {
            font-size: 16px;
        }
        .label-text {
            color: rgba(128, 128, 128, 0.87);
            font-size: 12px;
        }
        .solid-btn {
            &:not(.v-btn--flat) {
                background-color: @purpleLight !important;
                color: @color-white;
            }
        }
        .outline-btn {
            background-color: transparent !important;
            color: @purpleLight;
            border-radius: 16px;
            border: 1px solid @purpleLight;
        }
        .vicon{
            font-size: 21px;
        }
        .add-btn {
            color: @color-white;
            text-transform: initial;
            margin: 6px 8px;
        }
        .delete-sbf-icon {
            color: lighten(@color-black, 75%);
        }
        .purple-text {
            color: @purpleLight;
        }
        .btn-icon {
            font-size: 18px;
        }
        .class-list {
            background-color: #ffffff;
            max-height: 664px;
            min-height: calc(~"100vh - 350px");
            overflow-y: scroll;
            padding-left: 0;
            @media(max-width: @screen-xs){
                height: ~"calc(100% - 202px)";
                padding-bottom: 64px;
                min-height: unset;

            }
        }
        .list-item {
            display: flex;
            position: relative;
            margin: 0;
            border-bottom: solid 1px #f0f0f7;
            font-size: 16px;
            text-decoration: none;
            transition: background .3s cubic-bezier(.25, .8, .5, 1);
            &:first-child {
                border-top: solid 1px #f0f0f7;
            }

        }
        .limit-width {
            @media (max-width: @screen-xs) {
                max-width: 60% !important;
            }
        }
        .add-item {
            color: @global-blue;
        }
    }


</style>