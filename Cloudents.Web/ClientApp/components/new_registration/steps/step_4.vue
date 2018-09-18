<template>
    <div class="step-phone">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="meta.heading"></p>
                </div>
            </div>
            <div slot="step-data" class="limited-width">
                <h1 v-if="!isMobile" class="step-title" v-html="meta.heading"></h1>
                <!--<p v-if="!isMobile" class="sub-title">-->
                    <!--{{ isCampaignOn ? campaignData.stepTwo.text : meta.text }}-->
                <!--</p>-->
                <select v-model="phone.countryCode" class="mb-1">
                    <option value="" disabled hidden v-language:inner>login_select_your_country_code</option>
                    <option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}
                        ({{item.callingCode}})
                    </option>
                </select>
                <sb-input class="phone-field" icon="sbf-phone" :errorMessage="errorMessage.phone"
                          v-model="phone.phoneNum" placeholder="Enter phone number" name="email" type="tel"
                          :autofocus="true" @keyup.enter.native="sendCode()"></sb-input>
                <v-btn class="continue-btn"
                       value="Login"
                       :loading="loading"
                       :disabled="!(phone.phoneNum&&phone.countryCode)"
                       @click="sendCode()"
                ><span v-language:inner>login_continue</span></v-btn>
            </div>
            <img slot="step-image" :src="require(`../img/enter-phone.png`)"/>
        </step-template>
    </div>

</template>

<script>
    import stepTemplate from '../helpers/stepTemplate.vue'
    import codesJson from '../helpers/CountryCallingCodes';
    import { mapActions, mapGetters, mapMutations } from 'vuex'
    import registrationService from '../../../services/registrationService'
    import analyticsService from '../../../services/analytics.service';
    import SbInput from "../../question/helpers/sbInput/sbInput.vue";

    export default {
        components: {stepTemplate, SbInput},
        name: "step_4",
        data() {
            return {
                countryCodesList: codesJson.sort((a, b) => a.name.localeCompare(b.name)),
                errorMessage: {
                    phone: '',
                    code: '',
                    password: '',
                    confirmPassword: ''
                },
                loading: false
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
            phone: {
                phoneNum: '',
                countryCode: ''
            },
            userEmail: "",
            campaignData: {}
        },
        computed: {

        },
        methods: {
            ...mapMutations({updateLoading: "UPDATE_LOADING"}),
            ...mapActions({updateToasterParams: 'updateToasterParams'}),
            //sms code
            sendCode() {
                let self = this;
                self.loading = true;
                registrationService.smsRegistration(this.phone.countryCode, this.phone.phoneNum)
                    .then(function (resp) {
                        self.errorMessage.code = '';
                        self.updateToasterParams({
                            toasterText: 'A verification code was sent to your phone',
                            showToaster: true,
                        });
                        self.loading = false;
                        analyticsService.sb_unitedEvent('Registration', 'Phone Submitted');
                        self.$parent.$emit('updatePhone', self.phone);
                        self.$parent.$emit('changeStep', 'verifyPhone');
                    }, function (error) {
                        self.loading = false;
                        self.errorMessage.phone = error.response.data["Phone"] ?  error.response.data["Phone"][0] : '';
                    })
            },
        },
    }
</script>

<style scoped>

</style>