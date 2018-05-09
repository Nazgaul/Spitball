import stepTemplate from '../stepTemplate.vue'

export default {
    components: {stepTemplate},
    data() {
        return {
            countryCodesList: ['001', '002', '003'],
            codeSent: false,
            confirmationCode: '',
            phone: {
                phoneNum: '',
                countryCode: ''
            }
        }
    },
    methods: {
        updateEmail() {
            this.$emit('updateEmail');
        },
        sendCode() {
            this.updatePhone({countryCode: this.phone.countryCode, phoneNum: this.phone.phoneNum})
            this.codeSent = true;
        },
        next() {
            this.$emit('next');

        }
    },
}