<template>
    <div>
        <div class="form" v-if="!emailSent">
            <div>
                <h1>Get started</h1>
                <p>Start with your email. We need to know how to contact you</p>
                <button>Sign in with google</button>
                <div>or use your email</div>
                <input v-model="userEmail" type="email"/>
            </div>
            <button @click="next">Continue</button>
        </div>
        <div class="message" v-else>
            <h1>Check your email</h1>
            <p>You're a few steps away from veri... we sent email to {{userEmail}}
                <button @click="emailSent=false">edit</button>
            </p>
        </div>
    </div>
</template>
<script>
    import {mapGetters, mapActions} from 'vuex'

    export default {
        data() {
            return {
                userEmail: this.$store.getters.getEmail || '',
                emailSent: false
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
