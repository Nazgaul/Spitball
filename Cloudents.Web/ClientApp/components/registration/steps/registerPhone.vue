<template>
    <div>
        <div class="form" v-if="!codeSent">
            <h1>Enter your phone number</h1>
            <p>Last step, we need to send you a confirmation code.</p>
            <v-select :items="countryCodesList" v-model="phone.countryCode" label="Select"></v-select>
            <v-text-field v-model="phone.phoneNum" placeholder="Enter phone number"></v-text-field>
        </div>
        <div v-else>
            <h1>Enter the confirmation code</h1>
            <p>We sent the code to you by SMS to +{{this.phone.countryCode+this.phone.phoneNum}}
                <button @click="codeSent = false">Edit</button>
                Please enter the code below to confirm it.
            </p>
            <v-text-field v-model="confirmationCode" placeholder="Confirmation code"></v-text-field>
        </div>
        <button @click="next">Continue</button>
    </div>
</template>


<script>
    import {mapGetters, mapActions} from 'vuex'

    export default {
        data() {
            return {
                countryCodesList: ['001', '002', '003'],
                codeSent: false,
                confirmationCode: '',
                phone:{}
            }
        },
        computed: {
            ...mapGetters(['getPhone']),
        },
        methods: {
            ...mapActions(['updatePhone']),
            updateEmail() {
                this.$emit('updateEmail');
            },
            next() {
                if (!this.codeSent) {
                    debugger;
                    this.updatePhone({countryCode: this.phone.countryCode, phoneNum: this.phone.phoneNum})
                    this.codeSent = true;
                }
                else {
                    this.$emit('next');
                }
            }
        },
        created: function () {
            this.phone = {
                phoneNum: this.getPhone.phoneNum || '',
                countryCode: this.getPhone.countryCode || ''
            }
        }
    }
</script>
