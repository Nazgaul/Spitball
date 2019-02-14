<template>
    <div class="step-verifyEmail">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading">
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width wide">
                <h1 v-if="!isMobile" class="step-title"><span v-html="meta.heading"></span> <br/></h1>
                <p class="inline" v-language:inner>login_an_activation_email_has_been_sent_to</p>
                <div class="email-hold">
                    <p class="email-text inline">{{userEmail}}
                        <span class="email-change" @click="showRegistration()" v-language:inner>login_Change</span>
                    </p>
                </div>
                <p v-language:inner>login_until_activate_account</p>
                <div class="bottom-text">
                    <p class="inline"> <span v-language:inner>login_didnt_get_an_email</span>
                        <span class="email-text inline click"  @click="resendEmail()" v-language:inner>login_click_here_to_send_email</span>
                    </p>
                </div>
            </div>
            <img slot="step-image" :src="require(`../img/checkEmail.png`)"/>
        </step-template>
    </div>

</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";
    import { LanguageService } from "../../../services/language/languageService";
    export default {
        components: {stepTemplate, SbInput},
        name: "step_3",
        data() {
            return {
            }
        },
        props: {
            isMobile: {
                type: Boolean,
                default: false
            },
            isCampaignOn: {
                type: Boolean,
                default: false
            },
            meta: {
            },
            userEmail: ""
        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            ...mapActions({updateToasterParams: 'updateToasterParams'}),
            showRegistration() {
                this.$parent.$emit('changeStep', 'termandstart');
            },
            resendEmail() {
                var self = this;
                self.updateLoading(true);
                analyticsService.sb_unitedEvent('Registration', 'Resend Email');
                registrationService.emailResend()
                    .then(response => {
                            self.updateLoading(false);
                            self.updateToasterParams({
                                toasterText: LanguageService.getValueByKey("login_email_sent"),
                                showToaster: true,
                            })
                        },
                        error => {
                            self.updateLoading(false);
                            console.error('resent error', error)
                        })
            },
        },
    }

</script>

<style scoped>

</style>