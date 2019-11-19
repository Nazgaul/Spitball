<template>
    <v-card class="uni-creation-wrap">
        <v-layout class="close-toolbar limit-height pl-4 pr-4" style="width:100%;" align-center justify-end>
            <v-flex grow>
                <span class="font-weight-bold dialog-heading" v-language:inner>university_create_new_title</span>
            </v-flex>
            <v-flex shrink class="mr-2">
                <v-icon @click="closeDialog()" class="subtitle-1 course-close-icon">sbf-close</v-icon>
            </v-flex>
        </v-layout>
        <v-layout shrink align-center justify-center class="px-4 mt-4 mb-1">
            <v-flex xs12   class="text-center">
                <v-form  ref="uniForm"  v-model="validUniForm">
                <v-text-field v-model="university"
                              class="uni-input"
                              outline
                              prepend-inner-icon=""
                              :placeholder="newUniPlaceholder"
                              :rules="newUniRules"
                              autocomplete="off"
                              autofocus
                              spellcheck="true"></v-text-field>
                </v-form>
            </v-flex>
        </v-layout>
        <v-layout align-start justify-start shrink column class="px-4">
            <v-flex xs12  sm6 class="text-center mb-1">
                <span class="caption helper-text" v-language:inner>courses_minimum</span>
            </v-flex>
            <v-flex xs12  sm6 class="text-center mb-1">
                <span class="caption helper-text" v-language:inner>courses_meaningfull</span>

            </v-flex>
            <v-flex xs12  sm6 class="text-center mb-1">
                <!-- <span class="caption helper-text" v-language:inner>university_third_tip_for_creating</span> -->

            </v-flex>
        </v-layout>
        <v-layout align-center justify-center shrink class="pb-5 pt-4">
            <v-flex shrink class="text-center">
                <button @click="createNewUniversite()" :disabled="!universityName"
                        class="cursor-pointer create-btn min-width solid d-flex align-center justify-center py-2 px-3">
                    <span class="font-weight-bold text-uppercase btn-text" v-language:inner>university_btn_create_university</span>
                </button>
            </v-flex>
        </v-layout>
    </v-card>
</template>

<script>
    import { mapActions, mapGetters } from 'vuex';
    import { LanguageService } from "../../../services/language/languageService";

    export default {
        name: "createUniversity",
        data() {
            return {
                universityName: '',
                btnLoad: false,
                newUniRules: [
                    v => !!v || LanguageService.getValueByKey("formErrors_required"),
                    v => (v && v.length >= 10) || LanguageService.getValueByKey("formErrors_longer_10"),
                ],
                validUniForm: false,
                newUniPlaceholder: LanguageService.getValueByKey("university_create_uni_placeholder")
            };
        },
        computed: {
            ...mapGetters(['getSelectedClasses','getSchoolName']),
            university: {
                get(){
                    return this.universityName || this.getSchoolName
                },
                set(newValue) {
                    this.universityName = newValue;
                }
            }
        },
        methods: {
            ...mapActions(['createUniversity', 'changeUniCreateDialogState', 'updateUniVerification', 'updateToasterParams']),
            createNewUniversite() {
                if (this.$refs.uniForm.validate()) {
                    let self = this;
                    let university = self.universityName;
                    let classesSet =  self.getSelectedClasses && self.getSelectedClasses.length > 0;
                    let toasterText;
                    //create new uni add action in store needed

                    self.createUniversity(university).then((success)=>{
                        self.changeUniCreateDialogState(false);
                        classesSet ?  self.$router.push({name: 'feed'})  : self.$router.push({name: 'editCourse'});
                        toasterText = LanguageService.getValueByKey("university_uni_success");
                    }, (error)=>{
                        toasterText = LanguageService.getValueByKey("university_uni_error");
                    }).finally(() => {
                        self.updateToasterParams({
                            showToaster: true,
                            toasterText,
                            toasterTimeout: 5000
                        });
                    })
                }

            },
            closeDialog(){
                this.changeUniCreateDialogState(false);
                this.updateUniVerification(false);
            }
        },
        mounted() {
            if(this.getSchoolName){
                this.universityName = this.getSchoolName
            }
        },
    };
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .uni-creation-wrap {
        min-height: 340px;
        .close-toolbar{
            height: 54px;
            width: 100%;
            background-color: @global-purple;
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
            background-color: @global-purple;
            .btn-text {
                color: lighten(@color-white, 87%);
            }
        }
        .create-btn {
            &:disabled {
                background-color: lighten(@global-purple, 30%);
                color: lighten(@color-white, 30%);
            }
        }
        .min-width {
            width: 180px;
        }
        .uni-input {
            .v-input__slot {
                border: 1px solid @global-purple !important;
                input {
                    margin-top: 12px;
                    color: @colorBlackNew;
                    font-weight: 600;
                    font-size: 16px;
                }
            }
            .v-messages__message{
                line-height: 1.4;
            }
        }
    }
</style>