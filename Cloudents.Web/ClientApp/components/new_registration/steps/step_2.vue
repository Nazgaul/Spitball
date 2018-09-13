<template>
    <div class="step-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">
                        {{meta.heading}}</p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <div class="text">
                    <h1 class="step-title" v-language:inner>login_get_started</h1>
                    <p class="sub-title mb-3">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>
                </div>
                <form @submit.prevent="emailSend" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.phone"
                              placeholder="Enter your email address" v-model="userEmail" name="email" type="email"
                              :autofocus="true"></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper"
                                   :sitekey="siteKey"
                                   ref="recaptcha"
                                   @verify="onVerify"
                                   @expired="onExpired()">
                    </vue-recaptcha>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!userEmail || !recaptcha "
                           type="submit"
                    ><span v-language:inner>login_continue</span></v-btn>
                    <div class="checkbox-terms">
                    </div>
                </form>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/registerEmail.png`)"/>
            </div>
        </step-template>
    </div>

</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import VueRecaptcha from 'vue-recaptcha';
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";
    import { mapActions, mapMutations } from 'vuex'
    import registrationService from "../../../services/registrationService";

    var auth2;
    export default {
        components: {stepTemplate, SbInput, VueRecaptcha},

        name: "step_2",
        data() {
            return {
                siteKey: '6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1',
                errorMessage: {
                    phone: "",
                    email: ""
                },
                recaptcha: '',
                loading: false,
                userEmail: ''
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
            meta: {},

        },

        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            emailSend() {
                let self = this;
                self.loading = true;
                registrationService.emailRegistration(this.userEmail, this.recaptcha)
                    .then(function (resp) {
                        let step = resp.data.step;
                        // self.changeStepNumber(step);
                        self.$parent.$emit('updateEmail', self.userEmail);
                        self.$parent.$emit('changeStep', step);
                        analyticsService.sb_unitedEvent('Registration', 'Start');
                        self.loading = false;
                    }, function (error) {
                        self.recaptcha = "";
                        self.$refs.recaptcha.reset();
                        self.loading = false;
                        self.errorMessage.email = error.response.data ? Object.values(error.response.data)[0][0] : error.message;
                    });
            },
            // captcha events methods
            onVerify(response) {
                this.recaptcha = response;
            },
            onExpired() {
                this.recaptcha = "";
            },
        },
        created() {
        }
    }
</script>

<style scoped>

</style>