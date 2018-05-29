<template>
    <div class="step-email">
        <step-template v-if="!emailSent">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Get started</h1>
                <p class="sub-title">Start with your email. We need to know how to contact you.</p>
                <button class="google-signin" @click="googleLogIn">Sign in with google
                    <v-icon>sbf-google-icon</v-icon>
                </button>
                <div class="seperator-text"><span>or use your email</span></div>
                <form @submit.prevent="next">
                    <div class="input-wrapper">
                        <input required class="email-field input-field" name="email" v-model="userEmail" type="email"
                               placeholder="Enter your email address">
                        <v-icon>sbf-email</v-icon>
                    </div>
                    <vue-recaptcha class="recaptcha-wrapper" sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input :disabled="!userEmail || disableSubmit" class="continue-btn input-field" type="submit" value="Continue">
                </form>
                <div class="signin-strip">Do you already have an account?
                    <router-link to="signin">Sign in</router-link>
                </div>
            </div>
            <img slot="step-image" :src="require(`../../img/registerEmail.png`)"/>
        </step-template>

        <step-template v-else>
            <v-icon>sbf-email</v-icon>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Check your email</h1>
                <p>You’re a few steps away from verifying your account. We sent an email to: </p>
                <p class="email">{{userEmail}}</p>
                <button class="small-button" @click="emailSent=false">Edit</button>
                <p class="email-check">Check your email and click the email validation link.</p>
                <p class="resend-title">Didn’t get our email?</p>
                <button class="small-button" @click="emailSent=false">Resend</button>
            </div>
            <img slot="step-image" :src="require(`../../img/checkEmail.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerEmail.js"></script>

<style src="./registerEmail.less" lang="less"></style>

