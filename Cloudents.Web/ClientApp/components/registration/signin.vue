<template>
    <page-template>
        <step-template v-if="!codeSent">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Login</h1>
                <form @submit.prevent="submit">
                    <sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email" name="email" id="input-url" v-model="userEmail" placeholder="Enter your email address"></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper" ref="recaptcha"
                                   sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input class="continue-btn" type="submit" value="Login" :disabled="submitted || !userEmail || !recaptcha">
                </form>
                <div class="signin-strip">Need an account?
                    <router-link to="register">Sign up</router-link>
                </div>
            </div>
            <img slot="step-image" :src="require(`./img/signin.png`)"/>
        </step-template>
        <step-template v-else>
            <div slot="step-data" class="limited-width wide">
                <h1 class="step-title">Enter the confirmation code</h1>
                <!--<p class="sub-title">We sent the code to you by SMS to (+{{this.phone.countryCode}}) {{this.phone.phoneNum}}</p>-->
                <p class="confirm-title">We sent a confirmation code to your mobile phone.</p>
                <form @submit.prevent="verifyCode">
                <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"  v-model="confirmationCode" placeholder="Enter confirmation code" type="number" :autofocus="true"></sb-input>
                <!--<div class="input-wrapper">-->
                <!--<input class="code-field input-field" v-model="confirmationCode" placeholder="Confirmation code"></input>-->
                <!--<v-icon>sbf-key</v-icon>-->
                <!--</div>-->
                <button class="continue-btn submit-code" :disabled="submitted||!confirmationCode">Continue</button>
                </form>
                <div class="bottom-text signin-strip">
                    <p class="inline">Didn't get an sms?</p><p class="email-text inline click" @click="resendSms()">&nbsp;Click here to resend.</p>
                </div>
            </div>
            <img slot="step-image" :src="require(`../registration/img/confirm-phone.png`)"/>
        </step-template>
    </page-template>
</template>

<script src="./signin.js"></script>
<style src="./registration.less" lang="less"></style>
<style src="./signin.less" lang="less"></style>
