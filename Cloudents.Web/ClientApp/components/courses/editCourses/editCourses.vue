<template>
    <div class="courses-list-wrap">
        <div v-if="!isEmpty">
        <v-layout row class="py-4 px-4" align-center justify-center>
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
                    <div class="list-item search-class-item cursor-pointer"
                         v-for="singleClass in classesSelected">
                        <div>
                            {{ singleClass.text }}
                        </div>
                        <v-layout row align-center justify-end>
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
                                             @click="checkCanTeach(singleClass)">sbf-delete-outline</v-icon>
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
            ...mapActions(["updateClasses", "updateSelectedClasses", "assignClasses", "pushClassToSelectedClasses"]),
            toggleTeaching(course) {
                course.isTeaching = !course.isTeaching;
                console.log('can teach', course);
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
        .solid-btn{
            &:not(.v-btn--flat){
                background-color: @purpleLight!important;
                color: @color-white;
            }
        }
        .outline-btn{
            background-color:@color-white;
            color: @purpleLight;

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
            max-height: 250px;
            overflow-y: scroll;
            padding-left: 0;
        }
        .list-item {
            align-items: center;
            justify-content: space-between;
            color: inherit;
            display: flex;
            font-size: 16px;
            font-weight: 400;
            height: 48px;
            margin: 0;
            padding: 0 16px;
            position: relative;
            text-decoration: none;
            transition: background .3s cubic-bezier(.25, .8, .5, 1);
        }
        .add-item {
            color: @colorBlue;
        }
    }


</style>