<template>
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <!--step email-->
        <div class="step-email" v-if="stepNumber === 1">
            <step-template>
                <div slot="step-data" class="limited-width form-wrap">
                    <h1 class="step-title" v-if="$vuetify.breakpoint.smAndDown" v-language:inner>login_get_started</h1>
                    <p class="sub-title" v-if="$vuetify.breakpoint.smAndDown">{{campaignData.stepOne.text}}</p>
                    <button class="google-signin" @click="googleLogIn">
                        <span v-language:inner>login_sign_up_with_google</span>
                        <span>
                        <v-icon>sbf-google-icon</v-icon>
                    </span>
                    </button>
                    <div class="seperator-text"><span v-language:inner>login_or_use_your_email</span></div>
                    <form @submit.prevent="emailSend">
                        <sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.phone"
                                  placeholder="Enter your email address" v-model="userEmail" name="email" type="email"
                                  :autofocus="true"></sb-input>
                        <vue-recaptcha class="recaptcha-wrapper"
                                       :sitekey="siteKey"
                                       ref="recaptcha"
                                       @verify="onVerify"
                                       @expired="onExpired()">

                        </vue-recaptcha>
                        <!--<div style="width: 300px; height:74px; background: grey;  margin: 24px 0 10px 0;"></div>-->
                        <!--<input :disabled=" !userEmail || !recaptcha" class="continue-btn input-field" type="submit"-->
                               <!--value="Continue">-->
                        <v-btn  class="continue-btn" value="Login" :loading="loading" :disabled="!userEmail || !recaptcha " type="submit" v-language:inner>login_continue</v-btn>
                        <div class="checkbox-terms">
                            <span>
                                <span v-language:inner>login_By_joining_i_agree_to_spitball </span> 
                                <router-link to="terms" v-language:inner>login_terms_of_services</router-link>
                                <span v-language:inner>login_and</span>  
                                <router-link to="privacy" v-language:inner>login_privacy_policy</router-link>
                            </span>
                        </div>
                    </form>
                    <div class="signin-strip">
                        <span v-language:inner>login_do_you_already_have_an_account_?</span>  
                        <p class="click" @click="goToLogin()" v-language:inner>login_sign_in</p>
                    </div>
                </div>

                <div slot="step-image">
                    <div class="text">
                        <h1 class="step-title" v-language:inner>login_get_started</h1>
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
                <div slot="step-data" class="limited-width">
                    <h1 class="step-title" v-language:inner>login_login</h1>
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
                        <!--<div style="width: 300px; height:74px; background: grey;  margin: 24px 0 10px 0;"></div>-->
                        <!--<input class="continue-btn" type="submit" value="Login" :disabled=" !userEmail || !recaptcha">-->
                        <v-btn  class="continue-btn loginBtn"
                                value="Login"
                                :loading="loading"
                                :disabled="!userEmail || !recaptcha "
                                type="submit"
                         v-language:inner>login_login</v-btn>
                    </form>
                    <div class="signin-strip">
                        <span v-language:inner>login_need_an_account_?</span> 
                        <a @click="showRegistration" v-language:inner>login_sign_up</a>
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
                    <h1 class="step-title" v-language:inner>login_check_your_email_to_activate_your_account</h1>
                    <p class="inline" v-language:inner>login_an_activation_email_has_been_sent_to</p>
                    <div class="email-hold">
                        <p class="email-text inline">{{userEmail}}</p>
                        <span class="email-change" @click="showRegistration()" v-language:inner>login_Change</span>

                    </div>
                    <p v-language:inner>login_you_will_not_be_able_to_log_into_spitball_co_until_you_activate_your_account</p>
                    <img :src="require(`./img/checkEmail.png`)"/>
                    <div class="bottom-text">
                        <p class="inline" v-language:inner>login_didnt_get_an_emaill</p>
                        <p class="email-text inline click" @click="resendEmail()">&nbsp; <span v-language:inner>login_click_here_to_send</span> </p>
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
                    <h1 class="step-title" v-language:inner>login_enter_your_phone_number</h1>
                    <p class="sub-title">{{campaignData.stepTwo.text}}</p>
                    <select v-model="phone.countryCode" class="mb-1">
                        <option value="" disabled hidden v-language:inner>login_select_your_country_code</option>
                        <option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}
                            ({{item.callingCode}})
                        </option>
                    </select>
                    <sb-input class="phone-field" icon="sbf-phone" :errorMessage="errorMessage.phone"
                              v-model="phone.phoneNum" placeholder="Enter phone number" name="email" type="tel"
                              :autofocus="true" @keyup.enter.native="sendCode()"></sb-input>
                    <v-btn  class="continue-btn"
                            value="Login"
                            :loading="loading"
                            :disabled="!(phone.phoneNum&&phone.countryCode)"
                            @click="sendCode()" 
                    v-language:inner>login_Continue</v-btn>
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
                <div slot="step-data" class="limited-width wide">
                    <h1 class="step-title" v-language:inner>login_enter_the_confirm_code:"Enter the confirmation code"</h1>
                    <p v-if="phone.phoneNum" class="sub-title">
                        <span v-language:inner>login_we_sent_the_code_to_you_by_sms_to</span> 
                        (+{{phone.countryCode}})
                        {{phone.phoneNum}}</p>
                    <p class="confirm-title">
                        <span v-language:inner>login_we_sent_a_confirmation_code</span>
                        <br/>
                        <span v-language:inner>login_to_your_mobile_phone</span> 
                    </p>
                    <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"
                              v-model="confirmationCode" placeholder="Enter confirmation code" type="number"
                              :autofocus="true" @keyup.enter.native="smsCodeVerify()"></sb-input>
                    <v-btn  class="continue-btn submit-code"
                            value="Login"
                            :loading="loading"
                            :disabled="!confirmationCode"
                            @click="smsCodeVerify()"
                    v-language:inner>login_continue</v-btn>
                    <!--<button class="continue-btn submit-code" @click="smsCodeVerify()" :disabled="!confirmationCode">-->
                        <!--Continue-->
                    <!--</button>-->

                    <div class="bottom-text">
                        <p class="inline" v-language:inner>login_didnt_get_an_sms</p>
                        <p class="email-text inline click" @click="resendSms()">&nbsp; <span v-language:inner>login_click_here_to_resend</span> </p>
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
                    <h1 class="congrats-heading" v-language>login_congrats</h1>
                    <h2 class="congrats-heading" v-language>login_you_are_reward_with</h2>
                    <h2 class="congrats-heading"><span class="blue-points">{{initialPointsNum}}<span v-language>login_sbl</span> </span></h2>
                    <img class="money-done-img" :src="require(`./img/money-done.png`)"/>
                    <p class="congrats"> <span v-language>login_You_can_spend_them_to_get_help_with_your</span> <br/> <span v-language>login_homework_questions</span> </p>
                    <v-btn  class="continue-btn submit-code"
                            value="congrats"
                            :loading="loading"
                            @click="finishRegistration" v-language>login_lets_start</v-btn>
                    <!--<button class="continue-btn" @click="finishRegistration">Let's Start</button>-->
                </div>
                <img slot="step-image" :src="require(`./img/done.png`)"/>
            </step-template>
        </div>
        <!--step congrats end-->

        <!--step expired link-->
        <div class="step-expired" v-if="stepNumber === 7 ">
            <step-template>
                <v-icon>sbf-email</v-icon>
                <div slot="step-data" class="limited-width wide">
                    <h1 class="step-title" v-language:inner>login_you_didnt_complete_the_registration_process</h1>
                    <p class="inline" v-language:inner>login_your_confirmation_link_has_expired</p>
                    <p v-language:inner>login_you_will_not_be_able_to_log_into_spitball_co_until_you_activate_your_account</p>
                    <img :src="require(`./img/checkEmail.png`)"/>
                    <button class="continue-btn" @click="showRegistration()" v-language:inner>login_register</button>

                    <!--<div class="bottom-text">-->
                    <!--<p class="inline">Didn’t get an email?</p>-->
                    <!--<p class="email-text inline click"  @click="resendEmail()">&nbsp;Click here to resend.</p>-->
                    <!--</div>-->
                </div>
                <img slot="step-image" :src="require(`./img/checkEmail.png`)"/>
            </step-template>
        </div>
        <!--step expired link end-->

        <div class="progress" v-if="stepNumber !== 6 && stepNumber !== 7 ">
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
                    <h1 v-language:inner>login_are_you_sure_you_want_to_exit</h1>
                    <p v-language:inner>login_exiting_from_this_process_will_delete_all_your_progress_and_information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>
<script src="./login.js"></script>

<style lang="less" src="./login.less">

</style>