<template>
    <!--!!!step terms and first screen-->
    <div class="step-terms-firstscreen">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">
                        {{ isCampaignOn ? campaignData.stepOne.text : meta.heading }}
                    </p>
                </div>
                <div class="checkbox-terms">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTerm"/>
                    <label for="agreeTerm"></label>
                    <span><span v-language:inner>login_agree</span><br/><router-link
                            to="terms" v-language:inner>login_terms_of_services</router-link> <span v-language:inner>login_and</span>  <router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link></span>

                </div>
                <span class="has-error" v-if="confirmCheckbox"
                      style="background: white; display: block; color:red; text-align: center;">
                        <span v-language:inner>login_please_agree</span></span>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <div class="text">
                    <h1 class="step-title" v-language:inner>login_get_started</h1>
                    <!--<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>-->
                </div>
                <div class="checkbox-terms" v-if="!isMobile">
                    <input type="checkbox" v-model="agreeTerms" id="agreeTermDesk"/>
                    <label for="agreeTermDesk"></label>
                    <span><span v-language:inner>login_agree</span> <router-link
                            to="terms" v-language:inner>login_terms_of_services</router-link> <span v-language:inner>login_and</span> <router-link
                            to="privacy" v-language:inner>login_privacy_policy</router-link></span>
                </div>
                <div class="has-error" v-if="confirmCheckbox && !isMobile"
                     style="background: white; display: block; color:red; text-align: center;">
                    <span v-language:inner>login_please_agree</span>
                </div>
                <button class="google-signin" @click="googleLogIn">
                    <span v-language:inner>login_sign_up_with_google</span>
                    <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                </button>
                <div class="seperator-text"><span v-language:inner>login_or</span></div>
                <v-btn class="sign-with-email"
                       value="Login"
                       :loading="loading"
                       @click="goToEmailLogin()"
                >
                    <span v-language:inner>login_signin_your_email</span>
                </v-btn>
                <div class="signin-strip"><span v-language:inner>login_already_have_account</span>
                    <p class="click" @click="goToLogin()" v-language:inner>login_sign_in</p>
                </div>
            </div>
            <div slot="step-image">
                <!--<div class="text">-->
                <!--<h1 class="step-title" v-language:inner>login_get_started</h1>-->
                <!--<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>-->
                <!--</div>-->
                <img :src="require(`./img/registerEmail.png`)"/>
            </div>
        </step-template>
    </div>

    <!--!!!end terms and first screen-->
</template>

<script>
    export default {

    }
</script>

<style >

</style>