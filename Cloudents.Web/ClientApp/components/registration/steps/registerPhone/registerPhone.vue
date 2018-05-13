<template>
    <div>
        <step-template v-if="!codeSent">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Enter your phone number</h1>
                <p class="sub-title">We need to send you a confirmation code.</p>
                <select v-model="phone.countryCode">
                    <option value="" disabled hidden>Select your country code</option>
                    <option v-for="item in countryCodesList" :value="item.callingCode">{{item.name}}
                        ({{item.callingCode}})
                    </option>
                </select>
                <input class="phone-field" v-model="phone.phoneNum" placeholder="Enter phone number"/>
                <button class="continue-btn" @click="sendCode">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/enter-phone.png`)"/>
        </step-template>

        <step-template v-else>
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Enter the confirmation code</h1>
                <p class="sub-title">We sent the code to you by SMS to</p>
                <p class="phone-num">(+{{this.phone.countryCode}}) {{this.phone.phoneNum}}</p>
                <button class="small-button" @click="codeSent = false">Edit</button>
                <p class="confirm-title">Please enter the code below to confirm it.</p>
                <input class="code-field" v-model="confirmationCode" placeholder="Confirmation code"></input>
                <button class="small-button" @click="sendCode">Resend</button>
                <button class="continue-btn submit-code" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/confirm-phone.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerPhone.js"></script>
<style src="./registerPhone.less" lang="less"></style>