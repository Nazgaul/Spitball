<template>
    <div class="step-user-status-email">
        <step-template>
            <div slot="step-text" class="text-block-slot" v-if="isMobile">
                <div class="text-wrap-top">
                    <p class="text-block-sub-title" v-html="camefromCreate ? meta.createtxt : meta.resettxt">
                    </p>
                </div>
            </div>
            <div slot="step-data" class="limited-width form-wrap">
                <h1 v-if="!isMobile" class="step-title"  v-html="camefromCreate ? meta.createtxt : meta.resettxt"></h1>
                <p v-if="!camefromCreate" class="sub-title  mb-3" v-language:inner>login_happens_to_best</p>
                <form @submit.prevent="emailResetPassword" class="form-one">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.forgotPass" :bottomError="true"
                              placeholder="login_placeholder_email" v-model="userEmail" name="email" type="email"
                              :autofocus="true" v-language:placeholder></sb-input>
                    <div class="recaptcha-wrapper">
                    </div>
                    <v-btn class="continue-btn" value="Login"
                           :loading="loading"
                           :disabled="!userEmail"
                           type="submit"
                    >
                        <span v-language:inner>'login_user_status_next'</span></v-btn>
                </form>
                <div class="signin-strip">
                    <p class="click" @click="goToLogin" v-language:inner>{{camefromCreate ? 'login_already_have_password': 'login_remember_now'}}</p>
                </div>
            </div>
            <div slot="step-image">
                <img :src="require(`../img/signin.png`)"/>
            </div>
        </step-template>
    </div>
</template>

<script>
    export default {
        name: "step_11"
    }
</script>

<style scoped>

</style>