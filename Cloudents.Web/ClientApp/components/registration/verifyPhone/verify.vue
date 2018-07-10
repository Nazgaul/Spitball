<template>
    <div class="step-phone">
        <button class="back-button verify-close" @click="showDialogFunc()">
            <v-icon right>sbf-close</v-icon>
        </button>
        <v-dialog v-model="showDialog" max-width="600px" content-class="registration-dialog">
            <v-card>
                <button class="close-btn" @click="hideDialog()">
                    <v-icon>sbf-close</v-icon>
                </button>
                <v-card-text class="limited-width">
                    <h1>Are you sure you want to exit?</h1>
                    <p>Exit from this process will delete all your progress and information</p>
                    <button class="continue-btn" @click="$_back">Exit</button>
                </v-card-text>
            </v-card>
        </v-dialog>
        <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
            <div class="text-wrap" v-html="getToasterText"></div>
        </v-snackbar>
        <step-template v-if="!codeSent">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Enter your phone number </h1>
                <p class="sub-title">We need to send you a confirmation code.</p>
                <select v-model="phone.countryCode" class="mb-1">
                    <option value="" disabled hidden>Select your country code</option>
                    <option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}
                        ({{item.callingCode}})
                    </option>
                </select>
                <sb-input class="phone-field" icon="sbf-phone" :errorMessage="errorMessage.phone" v-model="phone.phoneNum" placeholder="Enter phone number" name="email" type="tel" :autofocus="true"></sb-input>

                <!--<div class="input-wrapper">-->
                <!--<input class="phone-field input-field" v-model="phone.phoneNum" placeholder="Enter phone number"/>-->
                <!--<v-icon>sbf-phone</v-icon>-->
                <!--</div>-->
                <button class="continue-btn" @click="sendCode()" :disabled="submitted||!(phone.phoneNum&&phone.countryCode)">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../img/enter-phone.png`)"/>
        </step-template>

        <step-template v-else>
            <v-snackbar absolute top :timeout="toasterTimeout" :value="getShowToaster">
                <div class="text-wrap" v-html="getToasterText"></div>
            </v-snackbar>
            <div slot="step-data" class="limited-width wide">
                <h1 class="step-title">Enter the confirmation code</h1>
                <p class="sub-title">We sent the code to you by SMS to (+{{this.phone.countryCode}}) {{this.phone.phoneNum}}</p>
                <p class="confirm-title">We sent a confirmation code to your mobile phone.</p>
                <sb-input class="code-field" icon="sbf-key" :errorMessage="errorMessage.code" v-model="confirmationCode" placeholder="Enter confirmation code" type="number" :autofocus="true"></sb-input>
                <!--<div class="input-wrapper">-->
                <!--<input class="code-field input-field" v-model="confirmationCode" placeholder="Confirmation code"></input>-->
                <!--<v-icon>sbf-key</v-icon>-->
                <!--</div>-->
                <button class="continue-btn submit-code" @click="next" :disabled="submitted||!confirmationCode">Continue</button>

                <div class="bottom-text">
                    <p class="inline">Didn't get an sms?</p><p class="email-text inline click" @click="sendCode">&nbsp;Click here to resend.</p>
                </div>
            </div>
            <img slot="step-image" :src="require(`../img/confirm-phone.png`)"/>
        </step-template>
    </div>
</template>
<script src="./verify.js"></script>
<style src="./verify.less" lang="less"></style>