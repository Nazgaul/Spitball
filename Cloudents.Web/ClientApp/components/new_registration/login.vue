<template>
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <!--!!!step terms and first screen-->
        <div class="step-terms-firstscreen" v-if="stepNumber === 0">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <h1 class="text-block-title">Make money</h1>
                        <p class="text-block-sub-title">while helping<br/> others with<br/> their homework.
                        </p>
                    </div>
                    <!--<p class="text-block-sub-title">-->
                    <!--{{campaignData.stepOne.text}}</p>-->
                    <div class="checkbox-terms">
                        <input type="checkbox" v-model="agreeTerms" id="agreeTerm"/>
                        <label for="agreeTerm"></label>
                        <span>I agree to Spitball's<br/><router-link
                                to="terms">Terms of Services</router-link> and <router-link
                                to="privacy">Privacy Policy</router-link></span>

                    </div>
                    <span class="has-error" v-if="confirmCheckbox"
                          style="background: white; display: block; color:red; text-align: center;">
                        Please agree to Terms And Services in order to proceed</span>
                </div>
                <div slot="step-data" class="limited-width form-wrap">
                    <div class="checkbox-terms">
                        <input type="checkbox" v-model="agreeTerms" id="agreeTermDesk"/>
                        <label for="agreeTermDesk"></label>
                        <span >I agree to Spitball's <router-link
                                to="terms">Terms of Services</router-link> and <router-link
                                to="privacy">Privacy Policy</router-link></span>
                    </div>
                    <div class="has-error" v-if="confirmCheckbox"
                         style="background: white; display: block; color:red; text-align: center;">
                        Please agree to Terms And Services in order to proceed</div>
                    <button class="google-signin" @click="googleLogIn">
                        <span>Sign Up with Google</span>
                        <span>
                            <v-icon>sbf-google-icon</v-icon>
                        </span>
                    </button>
                    <div class="seperator-text"><span>or</span></div>
                    <v-btn class="sign-with-email"
                           value="Login"
                           :loading="loading"
                           @click="goToEmailLogin()"
                    >Sign in with Email
                    </v-btn>
                    <div class="signin-strip">Do you already have an account?
                        <p class="click" @click="goToLogin()">Sign in</p>
                    </div>
                </div>
                <div slot="step-image">
                    <div class="text">
                        <h1 class="step-title">Get started</h1>
                        <p class="sub-title">{{campaignData.stepOne.text}}</p>
                    </div>
                    <img :src="require(`./img/registerEmail.png`)"/>
                </div>
            </step-template>
        </div>

        <!--!!!end terms and first screen-->

        <!--step email start-->
        <div class="step-email" v-if="stepNumber === 1">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <!--<h1 class="text-block-title">Make money</h1>-->
                        <p class="text-block-sub-title">We need to know<br/>
                            how to <b>contact you.</b>
                        </p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width form-wrap">
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
                        <v-btn class="continue-btn"
                               value="Login"
                               :loading="loading"
                               :disabled="!userEmail || !recaptcha "
                               type="submit"
                        >Continue
                        </v-btn>
                        <div class="checkbox-terms">
                        </div>
                    </form>
                </div>

                <div slot="step-image">
                    <div class="text">
                        <h1 class="step-title">Get started</h1>
                        <p class="sub-title">{{campaignData.stepOne.text}}</p>
                    </div>
                    <img :src="require(`./img/registerEmail.png`)"/>
                </div>
            </step-template>
        </div>
        <!--step email end-->

        <!--step login-->
        <div class="step-login" v-else-if="stepNumber === 6 ">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <h2 class="text-block-title">Welcome back</h2>
                        <p class="text-block-sub-title">please login</p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width">
                    <h2 v-if="!isMobile" class="step-title">Login</h2>
                    <form @submit.prevent="submit">
                        <sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email"
                                  name="email" id="input-url" v-model="userEmail"
                                  placeholder="Enter your email address"></sb-input>
                        <vue-recaptcha class="recaptcha-wrapper"
                                       ref="recaptcha"
                                       :sitekey="siteKey"
                                       @verify="onVerify"
                                       @expired="onExpired">

                        </vue-recaptcha>
                        <v-btn class="continue-btn loginBtn"
                               value="Login"
                               :loading="loading"
                               :disabled="!userEmail || !recaptcha "
                               type="submit"
                        >Login
                        </v-btn>
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
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <!--<h1 class="text-block-title">Make money</h1>-->
                        <p class="text-block-sub-title">Check your <br/> email to<br/> <b>activate your<br/> account</b>
                        </p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width wide">
                    <!--<h1 class="step-title">Check your email to activate your account</h1>-->
                    <p class="inline">An activation email has been sent to</p>
                    <div class="email-hold">
                        <p class="email-text inline">{{userEmail}}</p>
                        <span class="email-change" @click="showRegistration()">Change</span>
                    </div>
                    <p>You will not be able to log into Spitball.co until you activate your account.</p>
                    <!--<img :src="require(`./img/checkEmail.png`)"/>-->
                    <div class="bottom-text">
                        <p class="inline">Didn’t get an email?  <span class="email-text inline click"  @click="resendEmail()">Click here to resend.</span>
                        </p>
                   </div>
                </div>
                <img slot="step-image" :src="require(`./img/checkEmail.png`)"/>
            </step-template>
        </div>
        <!--step verify email end-->


        <!--step phone number-->
        <div class="step-phone" v-if="stepNumber === 3 ">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <!--<h1 class="text-block-title">Make money</h1>-->
                        <p class="text-block-sub-title"><b>Enter your phone<br/> number</b> <br/>We need to send you <br/>a confirmation code</p>
                        <!--<p class="text-block-sub-title">We need to send you a confirmation code-->
                        <!--</p>-->
                    </div>
                </div>
                <div slot="step-data" class="limited-width">
                    <h1 v-if="!isMobile" class="step-title">Enter your phone number</h1>
                    <p v-if="!isMobile" class="sub-title">{{campaignData.stepTwo.text}}</p>
                    <select v-model="phone.countryCode" class="mb-1">
                        <option value="" disabled hidden>Select your country code</option>
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
                    >Continue
                    </v-btn>
                    <!--<button class="continue-btn" @click="sendCode()"-->
                    <!--:disabled="!(phone.phoneNum&&phone.countryCode)">Continue-->
                    <!--</button>-->
                </div>
                <img slot="step-image" :src="require(`./img/enter-phone.png`)"/>
            </step-template>
        </div>
        <!--step phone number end-->

        <!--step verify phone number-->
        <div class="step-phone-confirm" v-if="stepNumber === 4 ">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <!--<h1 class="text-block-title">Make money</h1>-->
                        <p class="text-block-sub-title"><b>Enter the<br/> confirmation code</b><br/>We sent the code to you <br/>by SMS to</p>
                        <p class="text-block-sub-title" v-if="phone.phoneNum">(+{{phone.countryCode}})
                            {{phone.phoneNum }} <span class="phone-change" @click="changePhone()">Change</span>
                        </p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width wide">
                    <h1 v-if="!isMobile" class="step-title">Enter the confirmation code</h1>
                    <p v-if="phone.phoneNum" class="sub-title">We sent the code to you by SMS to
                        (+{{phone.countryCode}})
                        {{phone.phoneNum}} </p>
                    <p v-if="!isMobile" class="confirm-title">We sent a confirmation code<br/> to your mobile phone.</p>
                    <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"
                              v-model="confirmationCode" placeholder="Enter confirmation code" type="number"
                              :autofocus="true" @keyup.enter.native="smsCodeVerify()"></sb-input>
                    <v-btn class="continue-btn submit-code"
                           value="Login"
                           :loading="loading"
                           :disabled="!confirmationCode"
                           @click="smsCodeVerify()"
                    >Continue
                    </v-btn>
                    <!--<button class="continue-btn submit-code" @click="smsCodeVerify()" :disabled="!confirmationCode">-->
                    <!--Continue-->
                    <!--</button>-->

                    <div class="bottom-text">
                        <p class="inline">Didn't get an sms?
                            <span class="email-text inline click" @click="resendSms()"> Click here to resend.</span>
                        </p>
                    </div>
                </div>
                <img slot="step-image" :src="require(`./img/confirm-phone.png`)"/>
            </step-template>
        </div>
        <!--step verify phone number end-->

        <!--step congrats -->
        <div class="step-account" v-if="stepNumber === 5 ">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <h2 class="text-block-title">CONGRATS!</h2>
                        <p class="text-block-sub-title">You are<br/> rewarded with<br/> <b>100 SBL</b></p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width done">
                    <h1 v-if="!isMobile" class="congrats-heading">CONGRATS!</h1>
                    <h2 v-if="!isMobile" class="congrats-heading">You are rewarded with</h2>
                    <h2 v-if="!isMobile" class="congrats-heading"><span class="blue-points">{{initialPointsNum}} SBL</span></h2>
                    <img v-if="!isMobile" class="money-done-img" :src="require(`./img/money-done.png`)"/>
                    <p class="congrats">You can spend them to get help with your <br/> Homework questions.</p>
                    <v-btn class="continue-btn submit-code"
                           value="congrats"
                           :loading="loading"
                           @click="finishRegistration">Let's Start
                    </v-btn>
                    <!--<button class="continue-btn" @click="finishRegistration">Let's Start</button>-->
                </div>
                <img slot="step-image" :src="require(`./img/done.png`)"/>
            </step-template>
        </div>
        <!--step congrats end-->

        <!--step expired link-->
        <div class="step-expired" v-if="stepNumber === 7 ">
            <step-template>
                <div slot="step-text" class="text-block-slot" v-if="isMobile">
                    <div class="text-wrap-top">
                        <h2 class="text-block-title">Your<br/> confirmation link has expired</h2>
                        <p class="text-block-sub-title">You will not be able<br/> to log in to Spitball.co<br/> until you activate your account.</p>
                    </div>
                </div>
                <div slot="step-data" class="limited-width wide">
                    <h1 v-if="!isMobile" class="step-title">You didn't complete the registration process </h1>
                    <p v-if="!isMobile" class="inline">Your confirmation link has expired</p>
                    <p v-if="!isMobile">  You will not be able to log into Spitball.co until you activate your account.</p>
                    <img :src="require(`./img/checkEmail.png`)"/>
                    <button class="continue-btn" @click="showRegistration()">Register</button>

                    <!--<div class="bottom-text">-->
                    <!--<p class="inline">Didn’t get an email?</p>-->
                    <!--<p class="email-text inline click"  @click="resendEmail()">&nbsp;Click here to resend.</p>-->
                    <!--</div>-->
                </div>
                <img slot="step-image" :src="require(`./img/checkEmail.png`)"/>
            </step-template>
        </div>
        <!--step expired link end-->

        <div class="progress" v-if="stepNumber !== 6 && stepNumber !== 7 && stepNumber !== 0">
            <div v-for="page  in  progressSteps" :class="{highlighted: page===stepNumber}"></div>
        </div>

        <button class="back-button" @click="showDialog = true" v-if="stepNumber !== 5">
            <v-icon right>sbf-close</v-icon>
        </button>
        <!--exit dialog-->
        <v-dialog v-model="showDialog"  max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you<br/> sure <br/>you want<br/> to <b>exit?</b></h1>
                    <p>Exiting from this process will delete all your<br/> progress and information</p>

                    <v-btn v-if="isMobile" class="continue-registr"
                           @click="showDialog = false">Continue with registration
                    </v-btn>

                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>
<script src="./login.js"></script>

<style lang="less" src="./login.less">

</style>