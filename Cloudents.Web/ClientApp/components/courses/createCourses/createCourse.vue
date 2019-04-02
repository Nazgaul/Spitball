<template>
    <v-card class="creation-wrap">
        <v-layout class="close-toolbar pl-4 pr-3" style="width:100%;" align-center justify-end>
            <v-flex grow>
                <span class="font-weight-bold dialog-heading">Create New Course</span>
            </v-flex>
            <v-flex shrink class="mr-2">
                <v-icon @click="closeDialog()" class="subheading course-close-icon">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout shrink align-center justify-center class="px-4 mt-4 mb-3">
            <v-flex xs12 sm12 md12 class="text-xs-center">
                <v-text-field v-model="courseName"
                              class="course-input"
                              outline
                              prepend-inner-icon=""
                              :placeholder="'Type a course name'"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"
                              hide-details></v-text-field>
            </v-flex>
        </v-layout>
        <v-layout align-start justify-start shrink column class="px-4">
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text">Minimum Characters</span>
            </v-flex>
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text">Meaningfull Name</span>

            </v-flex>
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text">Third tip for creating a course</span>

            </v-flex>
        </v-layout>
        <v-layout align-center justify-center shrink class="pb-5 pt-4">
            <v-flex shrink class="text-xs-center">
                <button @click="createNewCourse()" :disabled="!courseName"
                        class="cursor-pointer create-btn min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-uppercase btn-text">Create Course</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
</template>

<script>
    import { mapActions } from 'vuex';

    export default {
        name: "createCourse",
        data() {
            return {
                courseName: ''
            };
        },
        methods: {
            ...mapActions(['createCourse', 'changeCreateDialogState']),
            createNewCourse() {
                let self = this;
                let course = {name: self.courseName};
                self.createCourse(course).then((success)=>{
                    // this.changeCreateDialogState(false);
                    self.$root.$emit('courseCreated', self.courseName);
                    // this.$router.push({name: 'editCourse'});
                });
                console.log(self.courseName);
            },
            closeDialog(){
                this.changeCreateDialogState(false);
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .creation-wrap {
        min-height: 340px;
        .close-toolbar{
            height: 54px;
            width: 100%;
            background-color: @profileTextColor;
            .dialog-heading{
                color: @color-white;
                font-size: 18px;
            }
        }

        .course-close-icon{
            height: 54px;
            width: 100%;
            &.light{
                color: @color-white;
            }
            &.dark{
                color: @profileTextColor;
            }
        }
        .helper-text {
            color: @colorBlackNew;
        }

        .heading-text {
            font-size: 18px;
        }
        .solid {
            outline: none;
            border-radius: 16px;
            background-color: @profileTextColor;
            .btn-text {
                color: lighten(@color-white, 87%);
            }
        }
        .create-btn {
            &:disabled {
                background-color: lighten(@profileTextColor, 30%);
                color: lighten(@color-white, 30%);
            }
        }
        .min-width {
            width: 146px;
        }
        .course-input {
            .v-input__slot {
                border: 1px solid @profileTextColor !important;
                input {
                    margin-top: 12px;
                    color: @colorBlackNew;
                    font-weight: 600;
                    font-size: 16px;
                }
            }
        }
    }
</style>