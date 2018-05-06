<template>
    <div>
        <step-template v-if="!emailSent">
            <div slot="step-data">
                <h1 class="step-title">Get started</h1>
                <p>Start with your email. We need to know how to contact you.</p>
                <button class="google-signin">Sign in with google</button>
                <div class="seperator-text"><span>or use your email</span></div>
                <form @submit.prevent="next">
                    <v-text-field name="email" v-model="userEmail" type="email" single-line
                                  label="Enter your email address"
                                  prepend-icon="sbf-search"></v-text-field>
                    <input class="continue-btn" type="submit" value="Submit">
                </form>
            </div>
            <img slot="step-image" :src="require(`../img/registerEmail.png`)"/>
        </step-template>

        <step-template v-else>
            <div slot="step-data">
                <h1 class="step-title">Check your email</h1>
                <p>You’re a few steps away from verifying your account. We sent an email to: </p>
                <p>{{userEmail}} <button @click="emailSent=false">edit</button></p>
                <p>Check your email and click the email validation link.</p>
                <p>Didn’t get our email? <button @click="emailSent=false">Resend</button></p>
            </div>
            <img slot="step-image" :src="require(`../img/checkEmail.png`)"/>
        </step-template>
    </div>
</template>
<script>
    import {mapGetters, mapActions} from 'vuex'
    import stepTemplate from './stepTemplate.vue'

    export default {
        components:{stepTemplate},
        data() {
            return {
                userEmail: this.$store.getters.getEmail || '',
                emailSent: false,
            }
        },
        computed: {
            ...mapGetters(['getEmail']),
        },
        methods: {
            ...mapActions(['updateEmail']),
            next() {
                this.updateEmail(this.userEmail);
                this.emailSent = true;
            }
        }
    }
</script>
