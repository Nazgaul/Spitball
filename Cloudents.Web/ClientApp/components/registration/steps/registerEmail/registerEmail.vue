<template>
    <div class="step-email">
        <step-template v-if="!emailSent">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title" v-if="$vuetify.breakpoint.smAndDown">Get started</h1>
                <p class="sub-title" v-if="$vuetify.breakpoint.smAndDown">Start with your email. We need to know how to contact you.</p>
                <button class="google-signin" @click="googleLogIn">
                    <span>Sign Up with Google</span>
                    <!--TODO do not use v-icon-->
                    <span>
                        <v-icon>sbf-google-icon</v-icon>
                    </span>
                </button>
                <div class="seperator-text"><span>or use your email</span></div>
                <form @submit.prevent="next">
                    <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage" placeholder="Enter your email address" v-model="userEmail" name="email" type="email" :autofocus="true"></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper" sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1" ref="recaptcha"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input :disabled="submitted||!recaptcha.length" class="continue-btn input-field" type="submit"
                           value="Continue">
                    <div class="checkbox-terms">
                        <span>By joining, I agree to Spitball <router-link to="terms">Terms of Services</router-link> and <router-link to="privacy">Privacy Policy</router-link></span>
                    </div>
                </form>
                <div class="signin-strip">Do you already have an account?
                    <router-link to="signin">Sign in</router-link>
                </div>
            </div>
            <div slot="step-image">
                <div class="text">
                    <h1 class="step-title">Get started</h1>
                    <p class="sub-title">Start with your email. We need to know how to contact you.</p>
                </div>                
                 <img  :src="require(`../../img/registerEmail.png`)"/>
            </div>
            
           
        </step-template>

        <step-template v-else>
            <v-icon>sbf-email</v-icon>
            <div slot="step-data" class="limited-width wide">
                <h1 class="step-title">Check your email to activate your account</h1>
                <p class="inline">An activation email has been sent to</p><p class="email-text inline">&nbsp;{{userEmail}}</p>
                <p>You will not be able to log into Spitball.co until you activate your account.</p>
                
                <!-- <button class="small-button" @click="emailSent=false">Edit</button> -->
                <!-- <p class="email-check">Check your email and click the email validation link.</p> -->
                <img :src="require(`../../img/checkEmail.png`)"/>
                <div class="bottom-text">
                    <p class="inline">Didn’t get an email?</p><p class="email-text inline click" @click="resend()">&nbsp;Click here to resend.</p>
                </div>
                
            </div>
            <img slot="step-image" :src="require(`../../img/checkEmail.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerEmail.js"></script>

<style src="./registerEmail.less" lang="less"></style>

