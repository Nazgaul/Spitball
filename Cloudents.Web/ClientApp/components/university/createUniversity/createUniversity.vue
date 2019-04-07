<template>
    <v-card class="creation-wrap">
        <v-layout class="close-toolbar limit-height pl-4 pr-3" style="width:100%;" align-center justify-end>
            <v-flex grow>
                <span class="font-weight-bold dialog-heading" v-language:inner>university_new_title</span>
            </v-flex>
            <v-flex shrink class="mr-2">
                <v-icon @click="closeDialog()" class="subheading course-close-icon">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout shrink align-center justify-center class="px-4 mt-4 mb-1">
            <v-flex xs12 sm12 md12 class="text-xs-center">
                <v-text-field v-model="universityName"
                              class="course-input"
                              outline
                              prepend-inner-icon=""
                              :placeholder="newCoursePlaceholder"
                              :rules="[rules.required]"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"></v-text-field>
            </v-flex>
        </v-layout>
        <v-layout align-start justify-start shrink column class="px-4">
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text" v-language:inner>courses_minimum</span>
            </v-flex>
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text" v-language:inner>courses_meaningfull</span>

            </v-flex>
            <v-flex xs12 md6 sm6 class="text-xs-center mb-1">
                <span class="caption helper-text" v-language:inner>courses_third_tip</span>

            </v-flex>
        </v-layout>
        <v-layout align-center justify-center shrink class="pb-5 pt-4">
            <v-flex shrink class="text-xs-center">
                <button @click="createNewCourse()" :disabled="!courseName"
                        class="cursor-pointer create-btn min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-uppercase btn-text" v-language:inner>courses_create_course</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
</template>

<script>
    import { mapActions } from 'vuex';
    import { LanguageService } from "../../../services/language/languageService";

    export default {
        name: "createUniversity",
        data() {
            return {
                courseName: '',
                btnLoad: false,
                rules: {
                    required: value => !!value || LanguageService.getValueByKey("formErrors_required"),
                },
                newCoursePlaceholder: LanguageService.getValueByKey("courses_new_placeholder")
            };
        },
        methods: {
            ...mapActions(['createUniversity', 'changeUniCreateDialogState']),
            createNewCourse() {
                let self = this;
                let course = {name: self.universityName};
                let classesSet = this.getClasses.length > 0;
                //create new uni add action in store needed
                self.createUniversity(course).then((success)=>{
                    // this.changeUniCreateDialogState(false);
                    // self.$root.$emit('courseCreated', self.universityName);
                    classesSet ? this.$router.go(-1) : this.$router.push({name: 'editCourse'});
                });
            },
            closeDialog(){
                this.changeCreateDialogState(false);
                // this.$root.$emit('courseDialogClosed', true);
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
        .limit-height{
            max-height: 54px;
        }

        .course-close-icon{
            height: 54px;
            width: 100%;
            color: @color-white;
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