<template xmlns="http://www.w3.org/1999/html">
    <div class="registration">
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <component :is="`step_${stepNumber}`"
                   :isMobile="isMobile"
                   :meta="meta"
                   :isCampaignOn="isCampaignOn"
                   :userEmail="userEmail"
                   :phone="phone"
                   :lastActiveRoute="lastActiveRoute"
                   :toUrl="toUrl"
                   :passResetCode="passResetCode"
                   :ID="ID"
                   :camefromCreate="camefromCreate"
        ></component>
        <!--!!!step terms and first screen-->
        <!--<div class="step-terms-firstscreen" v-if="stepNumber === 1">-->
            <!--<step-template>-->
                <!--<p>{{}}</p>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">-->
                            <!--{{ isCampaignOn ? campaignData.stepOne.text : meta.heading }}-->
                        <!--</p>-->
                    <!--</div>-->
                    <!--<div class="checkbox-terms">-->
                        <!--<input type="checkbox" v-model="agreeTerms" id="agreeTerm"/>-->
                        <!--<label for="agreeTerm"></label>-->
                        <!--<span><span v-language:inner>login_agree</span><br/><router-link-->
                                <!--to="terms" v-language:inner>login_terms_of_services</router-link> <span v-language:inner>login_and</span>  <router-link-->
                                <!--to="privacy" v-language:inner>login_privacy_policy</router-link></span>-->

                    <!--</div>-->
                    <!--<span class="has-error" v-if="confirmCheckbox"-->
                          <!--style="background: white; display: block; color:red; text-align: center;">-->
                        <!--<span v-language:inner>login_please_agree</span></span>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width form-wrap">-->
                    <!--<div class="text">-->
                    <!--<h1 class="step-title" v-language:inner>login_get_started</h1>-->
                    <!--&lt;!&ndash;<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>&ndash;&gt;-->
                    <!--</div>-->
                    <!--<div class="checkbox-terms" v-if="!isMobile">-->
                        <!--<input type="checkbox" v-model="agreeTerms" id="agreeTermDesk"/>-->
                        <!--<label for="agreeTermDesk"></label>-->
                        <!--<span><span v-language:inner>login_agree</span> <router-link-->
                                <!--to="terms" v-language:inner>login_terms_of_services</router-link> <span v-language:inner>login_and</span> <router-link-->
                                <!--to="privacy" v-language:inner>login_privacy_policy</router-link></span>-->
                    <!--</div>-->
                    <!--<div class="has-error" v-if="confirmCheckbox && !isMobile"-->
                         <!--style="background: white; display: block; color:red; text-align: center;">-->
                        <!--<span v-language:inner>login_please_agree</span>-->
                    <!--</div>-->
                    <!--<button class="google-signin" @click="googleLogIn">-->
                        <!--<span v-language:inner>login_sign_up_with_google</span>-->
                        <!--<span>-->
                            <!--<v-icon>sbf-google-icon</v-icon>-->
                        <!--</span>-->
                    <!--</button>-->
                    <!--<div class="seperator-text"><span v-language:inner>login_or</span></div>-->
                    <!--<v-btn class="sign-with-email"-->
                           <!--value="Login"-->
                           <!--:loading="loading"-->
                           <!--@click="goToEmailLogin()"-->
                           <!--&gt;-->
                           <!--<span v-language:inner>login_signin_your_email</span>-->
                    <!--</v-btn>-->
                    <!--<div class="signin-strip"><span v-language:inner>login_already_have_account</span>-->
                        <!--<p class="click" @click="goToLogin()" v-language:inner>login_sign_in</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-image">-->
                    <!--&lt;!&ndash;<div class="text">&ndash;&gt;-->
                        <!--&lt;!&ndash;<h1 class="step-title" v-language:inner>login_get_started</h1>&ndash;&gt;-->
                        <!--&lt;!&ndash;<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>&ndash;&gt;-->
                    <!--&lt;!&ndash;</div>&ndash;&gt;-->
                    <!--<img :src="require(`./img/registerEmail.png`)"/>-->
                <!--</div>-->
            <!--</step-template>-->
        <!--</div>-->

        <!--!!!end terms and first screen-->

        <!--step email start-->
        <!--<div class="step-email" v-if="stepNumber === 2">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">-->
                            <!--{{meta.heading}}</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width form-wrap">-->
                    <!--<div class="text">-->
                    <!--<h1 class="step-title" v-language:inner>login_get_started</h1>-->
                    <!--<p class="sub-title mb-3">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>-->
                    <!--</div>-->
                    <!--<form @submit.prevent="emailSend" class="form-one">-->
                        <!--<sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.phone"-->
                                  <!--placeholder="Enter your email address" v-model="userEmail" name="email" type="email"-->
                                  <!--:autofocus="true"></sb-input>-->
                        <!--<vue-recaptcha class="recaptcha-wrapper"-->
                                       <!--:sitekey="siteKey"-->
                                       <!--ref="recaptcha"-->
                                       <!--@verify="onVerify"-->
                                       <!--@expired="onExpired()">-->
                        <!--</vue-recaptcha>-->
                        <!--<v-btn class="continue-btn" value="Login"-->
                               <!--:loading="loading"-->
                               <!--:disabled="!userEmail || !recaptcha "-->
                               <!--type="submit"-->
                               <!--&gt;<span v-language:inner>login_continue</span></v-btn>-->
                        <!--<div class="checkbox-terms">-->
                        <!--</div>-->
                    <!--</form>-->
                <!--</div>-->
                <!--<div slot="step-image">-->
                    <!--&lt;!&ndash;<div class="text">&ndash;&gt;-->
                        <!--&lt;!&ndash;<h1 class="step-title" v-language:inner>login_get_started</h1>&ndash;&gt;-->
                        <!--&lt;!&ndash;<p class="sub-title">{{ isCampaignOn ? campaignData.stepOne.text : meta.text }}</p>&ndash;&gt;-->
                    <!--&lt;!&ndash;</div>&ndash;&gt;-->
                    <!--<img :src="require(`./img/registerEmail.png`)"/>-->
                <!--</div>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step email end-->

        <!--step verify email-->
        <!--<div class="step-verifyEmail" v-else-if="stepNumber === 3 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">-->
                            <!--{{meta.heading}}-->
                        <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width wide">-->
                    <!--<h1 v-if="!isMobile" class="step-title"><span v-language:inner>login_check_your_email</span> <br/> <span v-language:inner>login_your_account</span></h1>-->
                    <!--<p class="inline" v-language:inner>login_an_activation_email_has_been_sent_to</p>-->
                    <!--<div class="email-hold">-->
                        <!--<p class="email-text inline">{{userEmail}}-->
                            <!--<span class="email-change" @click="showRegistration()" v-language:inner>login_Change</span>-->
                        <!--</p>-->
                    <!--</div>-->
                    <!--<p v-language:inner>login_until_activate_account</p>-->
                    <!--<div class="bottom-text">-->
                        <!--<p class="inline"> <span v-language:inner>login_didnt_get_an_email</span> <span class="email-text inline click"-->
                                                                     <!--@click="resendEmail()" v-language:inner>login_click_here_to_send</span>-->
                        <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/checkEmail.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step verify email end-->

        <!--step phone number-->
        <!--<div class="step-phone" v-if="stepNumber === 4 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">-->
                            <!--{{meta.heading}}</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width">-->
                    <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_enter_your_phone_number</h1>-->
                    <!--<p v-if="!isMobile" class="sub-title">-->
                        <!--{{ isCampaignOn ? campaignData.stepTwo.text : meta.text }}-->
                    <!--</p>-->
                    <!--<select v-model="phone.countryCode" class="mb-1">-->
                        <!--<option value="" disabled hidden v-language:inner>login_select_your_country_code</option>-->
                        <!--<option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}-->
                            <!--({{item.callingCode}})-->
                        <!--</option>-->
                    <!--</select>-->
                    <!--<sb-input class="phone-field" icon="sbf-phone" :errorMessage="errorMessage.phone"-->
                              <!--v-model="phone.phoneNum" placeholder="Enter phone number" name="email" type="tel"-->
                              <!--:autofocus="true" @keyup.enter.native="sendCode()"></sb-input>-->
                    <!--<v-btn class="continue-btn"-->
                           <!--value="Login"-->
                           <!--:loading="loading"-->
                           <!--:disabled="!(phone.phoneNum&&phone.countryCode)"-->
                           <!--@click="sendCode()"-->
                           <!--&gt;<span v-language:inner>login_continue</span></v-btn>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/enter-phone.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step phone number end-->

        <!--step verify phone number-->
        <!--<div class="step-phone-confirm" v-if="stepNumber === 5 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-html="$options.filters.bolder(meta.heading, meta.boldText)">-->
                            <!--{{meta.heading}}</p>-->
                        <!--<p class="text-block-sub-title" v-if="phone.phoneNum">(+{{phone.countryCode}})-->
                            <!--{{phone.phoneNum }} <span class="phone-change" @click="changePhone()" v-language:inner>login_Change</span>-->
                        <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width wide">-->
                    <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_sent_confirmation_code</h1>-->
                    <!--<p v-if="phone.phoneNum" class="sub-title">-->
                        <!--<span v-language:inner>login_sent_code_by_sms</span>-->
                        <!--(+{{phone.countryCode}})-->
                        <!--{{phone.phoneNum}} </p>-->
                    <!--<p v-if="!isMobile" class="confirm-title" v-language:inner>login_sent_confirmation_code</p>-->
                    <!--<sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code"-->
                              <!--v-model="confirmationCode" placeholder="Enter confirmation code" type="number"-->
                              <!--:autofocus="true" @keyup.enter.native="smsCodeVerify()"></sb-input>-->
                    <!--<v-btn class="continue-btn submit-code"-->
                           <!--value="Login"-->
                           <!--:loading="loading"-->
                           <!--:disabled="!confirmationCode"-->
                           <!--@click="smsCodeVerify()"-->
                           <!--&gt;<span v-language:inner>login_continue</span></v-btn>-->
                    <!--<div class="bottom-text">-->
                        <!--<p class="inline"><span v-language:inner>login_didnt_get_sms</span> &nbsp;-->
                            <!--<span class="email-text inline click" @click="resendSms()" v-language:inner>login_click_here_to_send</span>-->
                        <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/confirm-phone.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step verify phone number end-->

        <!--step congrats -->
        <!--<div class="step-account" v-if="stepNumber === 6 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title">{{meta.heading}}</p>-->
                        <!--<p class="text-block-sub-title"-->
                           <!--v-html="$options.filters.bolder(meta.subheading, meta.boldText)">{{meta.subheading}}</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width done">-->
                    <!--<h2 v-if="!isMobile" class="congrats-heading"-->
                        <!--v-html="$options.filters.bolder(meta.heading, meta.boldText)">{{meta.heading}}</h2>-->
                    <!--<h2 v-if="!isMobile" class="congrats-heading">{{meta.subheading}}</h2>-->
                    <!--<img v-if="!isMobile" class="money-done-img" :src="require(`./img/money-done.png`)"/>-->
                    <!--<p class="congrats" v-html="$options.filters.bolder(meta.text, meta.boldText)">{{meta.text}}</p>-->

                    <!--<v-btn class="continue-btn submit-code"-->
                           <!--value="congrats"-->
                           <!--:loading="loading"-->
                           <!--@click="finishRegistration" v-language:inner>login_lets_start-->
                    <!--</v-btn>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/done.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step congrats end-->


        <!--step login-->
        <!--<div class="step-login" v-if="stepNumber === 7 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title"><b v-language:inner>login_welcome_back</b> <br/> <span v-language:inner>login_please_login</span> </p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width">-->
                    <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_welcome_back-->
                        <!--&lt;!&ndash;<br/>&ndash;&gt;-->
                    <!--&lt;!&ndash;<span v-language:inner> login_please_login</span>&ndash;&gt;-->
                    <!--</h1>-->
                    <!--&lt;!&ndash;<h1 v-if="!isMobile" class="step-title" v-language:inner>login_please_login</h1>&ndash;&gt;-->
                    <!--&lt;!&ndash;<p v-if="!isMobile" class="inline" v-language:inner>login_please_login</p>&ndash;&gt;-->
                    <!--<form @submit.prevent="submit">-->
                        <!--<sb-input :errorMessage="errorMessage.email" :required="true" class="email-field" type="email"-->
                                  <!--name="email" id="input-url" v-model="userEmail"-->
                                  <!--placeholder="Enter your mobile or email "></sb-input>-->
                        <!--<sb-input :errorMessage="errorMessage.password" :required="true" class="email-field mt-3" type="password"-->
                                  <!--name="email" id="input-url" v-model="password"-->
                                  <!--placeholder="Enter password"></sb-input>-->
                        <!--<vue-recaptcha class="recaptcha-wrapper"-->
                                       <!--ref="recaptcha"-->
                                       <!--:sitekey="siteKey"-->
                                       <!--@verify="onVerify"-->
                                       <!--@expired="onExpired">-->

                        <!--</vue-recaptcha>-->
                        <!--<v-btn class="continue-btn loginBtn"-->
                               <!--value="Login"-->
                               <!--:loading="loading"-->
                               <!--:disabled="!userEmail || !recaptcha || !password "-->
                               <!--type="submit"-->
                               <!--&gt; <span v-language:inner>login_login</span>-->
                        <!--</v-btn>-->
                    <!--</form>-->
                    <!--<div class="signin-strip"><span v-language:inner>login_need_an_account</span>-->
                        <!--<a @click="showRegistration" v-language:inner>login_sign_up</a>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/signin.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step login end-->

        <!--step expired link-->
        <!--<div class="step-expired" v-if="stepNumber === 8 ">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<h2 class="text-block-title"><span v-language:inner>login_Your</span><br/> <span v-language:inner>login_confirmation_link_expired</span></h2>-->
                            <!--<p class="text-block-sub-title"><span v-language:inner>login_until_activate_account1</span><br/><span v-language:inner>login_until_activate_account2</span><br/><span v-language:inner>login_until_activate_account3</span></p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width wide">-->
                    <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_not_complete_registration </h1>-->
                    <!--<p v-if="!isMobile" class="inline" v-language:inner>login_confirmation_link_expired</p>-->
                    <!--<p v-if="!isMobile"><span v-language:inner>login_until_activate_account1</span><span v-language:inner>login_until_activate_account2</span><span v-language:inner>login_until_activate_account3</span> </p>-->
                    <!--<img :src="require(`./img/checkEmail.png`)"/>-->
                    <!--<button class="continue-btn" @click="showRegistration()" v-language:inner>login_register</button>-->
                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/checkEmail.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step expired link end-->

        <!--step create password-->
        <!--<div class="step-phone" v-if="stepNumber === 9">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-language:inner>login_enter_new_password-->
                        <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width">-->
                    <!--<h1 v-if="!isMobile" class="step-title"  v-language:inner>login_enter_new_password</h1>-->
                    <!--<sb-input class="phone-field" :errorMessage="errorMessage.password"-->
                              <!--v-model="password" placeholder="Enter new password" name="password" type="password"-->
                              <!--:autofocus="true" @keyup.enter.native="" minlength="4"></sb-input>-->
                    <!--<sb-input class="phone-field"   :errorMessage="errorMessage.confirmPassword"-->
                              <!--v-model="confirmPassword" placeholder="Confirm password" name="confirmPassword" type="password"-->
                              <!--:autofocus="true" @keyup.enter.native=""></sb-input>-->
                    <!--<v-btn class="continue-btn"-->
                           <!--value="Password"-->
                           <!--:loading="loading"-->
                           <!--:disabled="!(password && confirmPassword)"-->
                           <!--@click="validatePassword()"-->
                    <!--&gt;<span v-language:inner>login_continue</span></v-btn>-->

                <!--</div>-->
                <!--<img slot="step-image" :src="require(`./img/signin.png`)"/>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step create password-->

        <!--step email password reset start-->
        <!--<div class="step-email" v-if="stepNumber === 10">-->
            <!--<step-template>-->
                <!--<div slot="step-text" class="text-block-slot" v-if="isMobile">-->
                    <!--<div class="text-wrap-top">-->
                        <!--<p class="text-block-sub-title" v-language:inner>login_reset_your_password-->
                           <!--</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-data" class="limited-width form-wrap">-->
                    <!--<h1 v-if="!isMobile" class="step-title" v-language:inner>login_reset_your_password</h1>-->
                    <!--<p v-if="!isMobile" class="sub-title  mb-3"-->
                        <!--v-language:inner>login_happens_to_best-->
                    <!--</p>-->
                    <!--<form @submit.prevent="emailResetPassword" class="form-one">-->
                        <!--<sb-input icon="sbf-email" class="email-field" :errorMessage="errorMessage.email"-->
                                  <!--placeholder="Enter your email address" v-model="userEmail" name="email" type="email"-->
                                  <!--:autofocus="true"></sb-input>-->
                        <!--<div class="recaptcha-wrapper">-->

                        <!--</div>-->
                        <!--<v-btn class="continue-btn" value="Login"-->
                               <!--:loading="loading"-->
                               <!--:disabled="!userEmail"-->
                               <!--type="submit"-->
                        <!--&gt;-->
                            <!--<span>Reset my password</span></v-btn>-->
                    <!--</form>-->
                    <!--<div class="signin-strip">-->
                        <!--<p class="click" @click="goToLogin">I remember now!</p>-->
                    <!--</div>-->
                <!--</div>-->
                <!--<div slot="step-image">-->
                    <!--&lt;!&ndash;<div class="text">&ndash;&gt;-->
                        <!--&lt;!&ndash;<h1 class="step-title" ></h1>&ndash;&gt;-->
                        <!--&lt;!&ndash;<p class="sub-title"></p>&ndash;&gt;-->
                    <!--&lt;!&ndash;</div>&ndash;&gt;-->
                    <!--<img :src="require(`./img/signin.png`)"/>-->
                <!--</div>-->
            <!--</step-template>-->
        <!--</div>-->
        <!--step email password reset-->


        <div class="progress" v-if="isShowProgress">
            <div v-for="page in progressSteps" :class="{highlighted: page===stepNumber}"></div>
        </div>

        <button class="back-button" @click="showDialog = true" v-if="stepNumber !== 6">
            <v-icon right>sbf-close</v-icon>
        </button>
        <!--exit dialog-->
        <v-dialog v-model="showDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="showDialog = false">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1 v-if="isMobile"><span v-language:inner>login_sure_exit1</span><br/><span v-language:inner>login_sure_exit2</span><br/><span v-language:inner>login_sure_exit3</span><br/><span v-language:inner>login_sure_exit4</span>&nbsp;<b><span v-language:inner>login_sure_exit5</span></b></h1>
                    <h1 v-else v-language:inner>login_are_you_sure_you_want_to_exit</h1>
                    <p><span v-language:inner>login_exiting_information1</span><br/> <span v-language:inner>login_exiting_information2</span></p>

                    <v-btn v-if="isMobile" class="continue-registr"
                           @click="showDialog = false"><span v-language:inner>login_continue_registration</span>
                    </v-btn>

                    <button class="continue-btn" @click="$_back" v-language:inner>login_Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>

         <!--password dialog-->
        <!--<v-dialog v-model="passDialog" max-width="600px" :fullscreen="isMobile" content-class="registration-dialog">-->
            <!--<v-card>-->
                <!--<button class="close-btn" @click="passDialog = false">-->
                    <!--<v-icon>sbf-close</v-icon>-->
                <!--</button>-->
                <!--<v-card-text class="limited-width">-->
                    <!--<h1 v-if="isMobile">-->
                        <!--<span v-language:inner>login_sure_exit1</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit2</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit3</span><br/>-->
                        <!--<span v-language:inner>login_sure_exit4</span>&nbsp;<b>-->
                        <!--<span v-language:inner>login_sure_exit5</span></b>-->
                    <!--</h1>-->
                    <!--<h1 v-else v-language:inner>login_are_you_sure_you_want_to_exit</h1>-->
                    <!--<p><span v-language:inner>login_exiting_information1</span><br/>-->
                        <!--<span v-language:inner>login_exiting_information2</span>-->
                    <!--</p>-->

                    <!--<v-btn  class="continue-registr"-->
                           <!--@click="goToResetPassword()">-->
                        <!--<span >Create password</span>-->
                    <!--</v-btn>-->
                    <!--<button class="continue-btn" @click="goToLogin()" >I have password</button>-->
                <!--</v-card-text>-->
            <!--</v-card>-->
        <!--</v-dialog>-->
    </div>
</template>
<script src="./login.js"></script>

<style lang="less" src="./login.less">

</style>