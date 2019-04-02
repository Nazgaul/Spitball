<template>
    <v-card class="creation-wrap">
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
        <v-layout align-start justify-start column class="px-4">
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
        <v-layout align-center justify-center class="pb-5 pt-0">
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
                let course = {name: this.courseName};
                this.createCourse(course).then((success)=>{
                    this.changeCreateDialogState(false);
                    this.$router.push({name: 'editCourse'});
                });
                console.log(this.courseName);
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .creation-wrap {
        min-height: 340px;
        .helper-text {
            color: @colorBlackNew;
        }

        .heading-text {
            font-size: 18px;
        }
        .solid {
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