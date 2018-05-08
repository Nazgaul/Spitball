<template>
    <div>
        <step-template v-if="!codeSent">
            <div slot="step-data">
                <h1>Enter your phone number</h1>
                <p>Last step, we need to send you a confirmation code.</p>
                <v-select :items="countryCodesList" v-model="phone.countryCode" label="Select"></v-select>
                <v-text-field v-model="phone.phoneNum" placeholder="Enter phone number"></v-text-field>
                <button class="continue-btn" @click="sendCode">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/enter-phone.png`)"/>
        </step-template>

        <step-template v-else>
            <div slot="step-data">
                <h1>Enter the confirmation code</h1>
                <p>We sent the code to you by SMS to +{{this.phone.countryCode+this.phone.phoneNum}}
                    <button @click="codeSent = false">Edit</button>
                    Please enter the code below to confirm it.
                </p>
                <v-text-field v-model="confirmationCode" placeholder="Confirmation code"></v-text-field>
                <button class="continue-btn" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/confirm-phone.png`)"/>
        </step-template>
    </div>
</template>
<script src="./registerPhone.js"></script>
<style src="./registerPhone.less" lang="less"></style>