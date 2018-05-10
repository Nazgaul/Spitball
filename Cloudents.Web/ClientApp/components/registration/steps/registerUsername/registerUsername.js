import stepTemplate from '../stepTemplate.vue'

export default {
    components:{stepTemplate},
    data() {
        return {
            countryCodesList: ['001', '002', '003'],
            codeSent: false,
            username: 'test123'
        }
    },
    methods: {
        next() {
            this.updateUserName(this.username);
            this.$emit('next');
        }
    },
}