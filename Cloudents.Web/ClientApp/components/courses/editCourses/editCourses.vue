<template>
    <div class="courses-list-wrap">
        <div v-if="!isEmpty">
        <v-layout row class="py-4 pl-4 pr-3" align-center justify-center>
            <v-flex grow xs10>
                <div class="d-inline-flex justify-center shrink">
                    <span class="subheading font-weight-bold">My Courses</span>
                    <span class="subheading font-weight-bold" v-if="coursesQuantaty">({{coursesQuantaty}})</span>
                </div>
            </v-flex>
            <v-flex xs2 shrink class="d-flex justify-end">
                <v-btn round  color="#4452FC" class="add-btn py-1 font-weight-bold my-0" @click="goToAddMore()">
                    <v-icon left>sbf-plus-regular</v-icon>
                    <span>Add</span>
                </v-btn>
            </v-flex>
        </v-layout>
        <v-layout align-center>
            <v-flex class="search-classes-container">
                <div class="class-list search-classes-list">
                    <div class="list-item search-class-item cursor-pointer py-2 mx-2 justify-space-between align-center font-weight-regular"
                         v-for="singleClass in classesSelected">
                        <v-layout column class="pl-4">
                            <v-flex class="text-truncate course-name-wrap">
                                {{ singleClass.text }}
                            </v-flex>
                            <v-flex class="students-enrolled pt-1">
                                {{singleClass.students}}
                                <span class="students-enrolled">students</span>
                            </v-flex>
                        </v-layout>

                        <v-layout row align-center justify-end class="pr-2">
                            <v-flex shrink class="d-flex align-center">
                                <v-btn v-if="!singleClass.isTeaching" round @click="toggleTeaching(singleClass)"
                                       class="outline-btn elevation-0 text-none align-center justify-center rounded-btn">
                                    <span>
                                    <v-icon color="#a3a0fb" class="btn-icon mr-1" >sbf-face-icon</v-icon>
                                        <span class="purple-text caption">Teach</span>
                                    </span>
                                </v-btn>

                                <v-btn v-else round @click="toggleTeaching(singleClass)"
                                       class="solid-btn elevation-0 text-none align-center justify-center rounded-btn">
                                    <span>
                                       <v-icon class="btn-icon mr-1" >sbf-checkmark</v-icon>
                                        <span class="caption">Teaching</span>
                                  </span>
                                </v-btn>

                                <span>
                                     <v-icon class="delete-sbf-icon"
                                             @click="removeClass(singleClass)">sbf-delete-outline</v-icon>
                                   </span>
                            </v-flex>
                        </v-layout>
                    </div>
                </div>
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
    export default {
        name: "selectedCourses",
        components: {coursesEmptyState},
        data() {
            return {};
        },
        computed: {
            ...mapGetters(['getSelectedClasses']),
            classesSelected() {
                return this.getSelectedClasses;
            },
            isEmpty(){
                return this.getSelectedClasses.length < 1
            },
            coursesQuantaty() {
                return this.getSelectedClasses.length;
            }

        },
        methods: {
            ...mapActions(["updateClasses", "deleteClass", "updateSelectedClasses", "assignClasses", "pushClassToSelectedClasses"]),
            toggleTeaching(course) {
                course.isTeaching = !course.isTeaching;
                console.log('can teach', course);
            },
            removeClass(classDelete){
              this.deleteClass(classDelete);
            },
            deleteFromList(classToDelete, from) {
                let index = from.indexOf(classToDelete);
                from.splice(index, 1);
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
        .scrollBarStyle(0px, #0085D1);
        .rounded-btn {
            border-radius: 16px;
        }
        .students-enrolled{
            color: rgba(128, 128, 128, 0.87);
            font-size: 10px;
        }
        .solid-btn{
            &:not(.v-btn--flat){
                background-color: @purpleLight!important;
                color: @color-white;
            }
        }
        .outline-btn{
            background-color:transparent!important;
            color: @purpleLight;
            border-radius: 16px;
            border: 1px solid @purpleLight;

        }
        .add-btn{
            color: @color-white;
        }
        .delete-sbf-icon{
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
            overflow-y: scroll;
            padding-left: 0;
        }
        .list-item {
            display: flex;
            position: relative;
            margin: 0;
            border-bottom: solid 1px #f0f0f7;
            font-size: 16px;
            text-decoration: none;
            transition: background .3s cubic-bezier(.25, .8, .5, 1);

        }
        .course-name-wrap{
            @media(max-width: @screen-xs){
                max-width: 60%;
            }
        }
        .add-item {
            color: @colorBlue;
        }
    }


</style>