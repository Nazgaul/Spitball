<template>
    <div class="register-account">
        <step-template v-if="!showSummary">
            <div slot="step-data" class="limited-width">
                <h1 class="step-title">Your Account Number</h1>
                <p class="sub-title">Here is your account number, don’t lose it! Save it in a place you remember, or screenshot this
                    page.</p>
                <account-num :account-num="accountNum"></account-num>
                <button class="continue-btn" @click="next()">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/account.png`)"/>
        </step-template>
        <step-template v-else>
            <div slot="step-data" class="limited-width done">
                <h1>CONGRATS!</h1>
                <p>You just earned your first 10 tokens.</p>
                <button class="continue-btn" @click="finishRegistration">Lets shop</button>
            </div>
            <img slot="step-image" :src="require(`../../img/done.png`)"/>
        </step-template>


        <v-dialog v-model="openDialog" max-width="600px">
            <v-card>
                <v-card-text class="limited-width">
                    <h1>Beware…</h1>
                    <p><b>Save this number or screenshot your screen.</b>
                        You will need it  to login to the Spitball and
                        retrieve your funds.</p>
                    <account-num :account-num="accountNum"></account-num>
                    <button class="continue-btn" @click="closeDialog">ok</button>
                </v-card-text>
            </v-card>
        </v-dialog>

    </div>
</template>
<script src="./registerAccount.js"></script>
<style src="./registerAccount.less" lang="less"></style>
