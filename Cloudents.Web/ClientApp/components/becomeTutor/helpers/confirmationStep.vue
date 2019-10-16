<template>
    <div class="confirmation-step-wrap" :class="[!isMobile ? 'px-0' : '']">
        <v-layout column wrap align-center justify-center :class="[{'px-5':!isMobile},{'pt-4':!isMobile},'confirmation-step-wrap-cont']">
            <div :class="['code-txt',{'pr-5':!isMobile}]">
                <p :class="['code-txt-title','mb-2']" v-language:inner="'becomeTutor_code_of_conduct_title'"/>
                <p v-language:inner="'becomeTutor_code_of_conduct'"/>
            </div>
            <v-checkbox :ripple="false" class="checkbox-confirmation-step"
                        :label="$Ph('becomeTutor_agree')"
                        v-model="isAgree" off-icon="sbf-check-box-un" 
                        on-icon="sbf-check-box-done"/>
            </v-layout>

        <v-layout class="px-1 btns-confirmation-step"
                :class="[isMobile ? 'align-end justify-end' : 'align-center justify-center']">
            <v-btn class="cancel-btn-step elevation-0" round outline flat @click="goToPreviousStep">
                <span v-language:inner="'becomeTutor_btn_back'"/>
            </v-btn>
            <v-btn  color="#4452FC"
                    :loading="isLoading"
                    :disabled="!isAgree"
                    @click="submit"
                    round
                    class="white-text elevation-0">
                    <span v-language:inner="'becomeTutor_btn_done'"/>
            </v-btn>
        </v-layout>
    </div>
</template>

<script>
import { mapActions } from 'vuex';
import { LanguageService } from "../../../services/language/languageService";

export default {
    name: "confirmationStep",
    data() {
        return {
            isAgree: false,
            isLoading: false,
        }
    },
    computed: {
        isMobile(){
            return this.$vuetify.breakpoint.xsOnly;
        }
    },
    methods: {
        ...mapActions(['updateTutorDialog','updateAccountUserToTutor','sendBecomeTutorData','updateTeachingClasses','updateToasterParams']),
        goToPreviousStep() {
            this.$root.$emit('becomeTutorStep', 3);
        },
        submit(){
            let self = this
            this.isLoading = true;
            this.sendBecomeTutorData().then(
                (resp) => {
                    self.$root.$emit('becomeTutorStep', 5);
                    self.updateAccountUserToTutor(true);
                    self.updateToasterParams({
                        toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                        showToaster: true,
                        toasterTimeout: 5000
                    });
                    self.updateTeachingClasses();
                },(error) => {
                    let isConflict = error.response.status === 409;
                    if(isConflict) {
                        self.updateToasterParams({
                            toasterText: LanguageService.getValueByKey("becomeTutor_already_submitted"),
                            showToaster: true,
                            toasterTimeout: 5000
                        });
                        self.updateTutorDialog(false);
                    }
                }).finally(() => {                      
                    self.isLoading = false;
            });
        },
    }
};
</script>

<style lang="less">
    @import '../../../styles/mixin.less';

    .confirmation-step-wrap {
        @media (max-width: @screen-xs) {
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 0 !important;
            overflow: visible;
            justify-content: space-between;
            // height: inherit
        }
        .confirmation-step-wrap-cont{
            .code-txt{
                .code-txt-title{
                    font-size: 16px;
                    font-weight: bold;
                }
                max-height: 250px;
                overflow: auto;
                white-space: pre-line;
                @media (max-width: @screen-xs) {
                    max-height: initial;
                }
            }
            .checkbox-confirmation-step{
                .v-input__slot{
                display: flex;
                align-items: unset;
                    .v-icon{
                        color: @global-blue !important;
                    }
                    .v-messages{
                        display: none;
                    }
                }
            }
        }

        .btns-confirmation-step{
            .cancel-btn-step{
                background: white !important;
                border: solid 1px @global-blue;
                color: @global-blue;
            }
            .v-btn{
                @media (max-width: @screen-xs) {
                    text-transform: capitalize;
                }
            }
        }
    }
</style>