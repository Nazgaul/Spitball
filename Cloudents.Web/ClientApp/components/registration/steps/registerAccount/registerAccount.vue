<template>
    <div>
        <step-template v-if="!showSummary">
            <div slot="step-data">
                <h1>Your Account Number</h1>
                <p>Here is your account number, don’t lose it! Save it in a place you remember, or screenshot this
                    page.</p>
                <account-num></account-num>
                <button class="continue-btn" @click="showSummary = true">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../../img/account.png`)"/>
        </step-template>
        <step-template v-else>
            <div slot="step-data">
                <h1>CONGRATS!</h1>
                <p>You just earned your first 10 tokens.</p>
                <button class="continue-btn" @click="finishRegistration">Lets shop</button>
            </div>
            <img slot="step-image" :src="require(`../../img/done.png`)"/>
        </step-template>


        <v-dialog v-model="openDialog" max-width="500px">
            <v-card>
                <v-card-text>
                    <h1>Your Account Number</h1>
                    <p>Here is your account number, don’t lose it! Save it in a place you remember, or screenshot this
                        page.</p>
                    <account-num></account-num>
                </v-card-text>
                <v-card-actions>
                    <v-btn flat @click="closeDialog">Close</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>

    </div>
</template>
<script src="./registerAccount.js"></script>
<style src="./registerAccount.less" lang="less"></style>
