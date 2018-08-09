<template>
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <!--step email-->
        <div class="step-email" v-if="stepNumber === 1">
            <step-template>
                <div slot="step-data" class="limited-width form-wrap">
                    <h1 class="step-title" v-if="$vuetify.breakpoint.smAndDown">Get started</h1>
                    <p class="sub-title" v-if="$vuetify.breakpoint.smAndDown">Start with your email. We need to know how
                        to contact you.</p>
                    <button class="google-signin" @click="googleLogIn">
                        <span>Sign Up with Google</span>
                        <!--TODO do not use v-icon-->
                        <span>
                        <v-icon>sbf-google-icon</v-icon>
                    </span>
                    </button>
                    <div class="seperator-text"><span>or use your email</span></div>
                    <form @submit.prevent="emailSend">
                        <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.phone"
                                  placeholder="Enter your email address" v-model="userEmail" name="email" type="email"
                                  :autofocus="true"></sb-input>
                        <vue-recaptcha class="recaptcha-wrapper" sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                       ref="recaptcha"
                                       @verify="onVerify" @expired="onExpired()"></vue-recaptcha>
                        <input :disabled=" !userEmail || !recaptcha.length" class="continue-btn input-field" type="submit"
                               value="Continue">
                        <div class="checkbox-terms">
                            <span>By joining, I agree to Spitball <router-link
                                    to="terms">Terms of Services</router-link> and <router-link to="privacy">Privacy Policy</router-link></span>
                        </div>
                    </form>
                    <div class="signin-strip">Do you already have an account?
                        <p class="click" @click="goToLogin()">Sign in</p>
                    </div>
                </div>

                <div slot="step-image">
                    <div class="text">
                        <h1 class="step-title">Get started</h1>
                        <p class="sub-title">Start with your email. We need to know how to contact you.</p>
                    </div>
                    <img :src="require(`./img/registerEmail.png`)"/>
                </div>
            </step-template>
        </div>
        <!--step email end-->

        <!--step login-->
        <div class="step-login" v-else-if="stepNumber === 6 ">
        <step-template>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Login</h1>
                <form @submit.prevent="submit">
                    <sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email" name="email" id="input-url" v-model="userEmail" placeholder="Enter your email address"></sb-input>
                    <vue-recaptcha class="recaptcha-wrapper" ref="recaptcha"
                                   sitekey="6LcuVFYUAAAAAOPLI1jZDkFQAdhtU368n2dlM0e1"
                                   @verify="onVerify" @expired="onExpired"></vue-recaptcha>
                    <input class="continue-btn" type="submit" value="Login" :disabled=" !userEmail || !recaptcha">
                </form>
                <div class="signin-strip">Need an account?
                    <a @click="showRegistration">Sign up</a>
                </div>
            </div>
            <img slot="step-image" :src="require(`./img/signin.png`)"/>
        </step-template>
        </div>
        <!--step login end-->

        <!--step verify email-->
        <div class="step-verifyEmail" v-else-if="stepNumber === 2 ">
            <step-template>
                <v-icon>sbf-email</v-icon>
                <div slot="step-data" class="limited-width wide">
                    <h1 class="step-title">Check your email to activate your account</h1>
                    <p class="inline">An activation email has been sent to</p>
                    <p class="email-text inline">&nbsp;{{userEmail}}</p>
                    <p>You will not be able to log into Spitball.co until you activate your account.</p>
                    <img :src="require(`./img/checkEmail.png`)"/>
                    <div class="bottom-text">
                        <p class="inline">Didn’t get an email?</p>
                        <p class="email-text inline click"  @click="resendEmail()">&nbsp;Click here to resend.</p>
                    </div>
                </div>
                <img slot="step-image" :src="require(`./img/checkEmail.png`)"/>
            </step-template>
        </div>
        <!--step verify email end-->



        <!--step phone number-->
        <div class="step-phone" v-if="stepNumber === 3 ">
            <step-template>
                <div slot="step-data" class="limited-width">
                    <h1 class="step-title">Enter your phone number</h1>
                    <p class="sub-title">We need to send you a confirmation code.</p>
                    <select v-model="phone.countryCode" class="mb-1">
                        <option value="" disabled hidden>Select your country code</option>
                        <option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}
                            ({{item.callingCode}})
                        </option>
                    </select>
                    <sb-input class="phone-field" icon="sbf-phone" :errorMessage="errorMessage.phone"
                              v-model="phone.phoneNum" placeholder="Enter phone number" name="email" type="tel"
                              :autofocus="true"></sb-input>
                    <button class="continue-btn"  @click="sendCode()"
                            :disabled="!(phone.phoneNum&&phone.countryCode)">Continue
                    </button>
                </div>
                <img slot="step-image" :src="require(`./img/enter-phone.png`)"/>
            </step-template>
        </div>
        <!--step phone number end-->

        <!--step verify phone number-->
        <div class="step-phone-confirm" v-if="stepNumber === 4 ">
            <step-template>
                <div slot="step-data" class="limited-width wide">
                    <h1 class="step-title">Enter the confirmation code</h1>
                    <p class="sub-title">We sent the code to you by SMS to (+{{phone.countryCode}})
                        {{phone.phoneNum}}</p>
                    <p class="confirm-title">We sent a confirmation code to your mobile phone.</p>
                    <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"
                              v-model="confirmationCode" placeholder="Enter confirmation code" type="number"
                              :autofocus="true"></sb-input>
                            <button class="continue-btn submit-code" @click="smsCodeVerify" :disabled="!confirmationCode">
                        Continue
                    </button>

                    <div class="bottom-text">
                        <p class="inline">Didn't get an sms?</p>
                        <p class="email-text inline click" @click="resendSms()">&nbsp;Click here to resend.</p>
                    </div>
                </div>
                <img slot="step-image" :src="require(`./img/confirm-phone.png`)"/>
            </step-template>
        </div>
        <!--step verify phone number end-->

        <!--step congrats -->
        <div class="step-account" v-if="stepNumber === 5 ">
                <step-template>
                    <div slot="step-data" class="limited-width done">
                        <h1 class="congrats-heading">CONGRATS!</h1>
                        <h2 class="congrats-heading">You are rewarded with</h2>
                        <h2 class="congrats-heading"><span class="blue-points">{{initialPointsNum}} SBL</span></h2>
                        <img class="money-done-img"  :src="require(`./img/money-done.png`)"/>
                        <p class="congrats">You can spend them to get help with your <br/> Homework questions.</p>
                        <button class="continue-btn" @click="finishRegistration">Let's Start</button>
                    </div>
                    <img  slot="step-image" :src="require(`./img/done.png`)"/>
                </step-template>
        </div>
        <!--step congrats end-->

        <div class="progress" v-if="stepNumber !== 6">
            <div v-for="page  in  progressSteps" :class="{highlighted: page===stepNumber}"></div>
        </div>

        <button class="back-button" @click="showDialog = true" v-if="stepNumber !== 5">
            <v-icon right>sbf-close</v-icon>
        </button>
        <!--exit dialog-->
        <v-dialog v-model="showDialog" max-width="600px" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to exit?</h1>
                    <p>Exiting from this process will delete all your progress and information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>
<script src="./login.js"></script>

<style lang="less" src="./login.less">

</style>