<template>
    <div>
        <step-template v-if="!showSummary">
            <div slot="step-data">
                <h1>Your Account Number</h1>
                <p>Here is your account number, don’t lose it! Save it in a place you remember, or screenshot this
                    page.</p>
                <account-num></account-num>
                <button class="continue-btn" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../img/account.png`)"/>
        </step-template>
        <step-template v-else>
            <div slot="step-data">
                <h1>CONGRATS!</h1>
                <p>You just earned your first 10 tokens.</p>
                <button class="continue-btn" @click="finishRegistration">Lets shop</button>
            </div>
            <img slot="step-image" :src="require(`../img/registerEmail.png`)"/>
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

<script>
    import accountNum from "./accountNum.vue";
    import stepTemplate from './stepTemplate.vue'

    export default {
        components: {accountNum, stepTemplate},
        data() {
            return {
                openDialog: false,
                dialogWasViewed: false,
                showSummary: false,
            }
        },
        methods: {
            next() {
                if (!this.dialogWasViewed) {
                    this.openDialog = true;
                }
                else {
                    this.showSummary = true;
                }
            },
            closeDialog() {
                this.openDialog = false;
                this.dialogWasViewed = true
            },
            finishRegistration() {
                this.$router.push({path: '/note', query: {q: ''}});
            }
        }
    }
</script>
