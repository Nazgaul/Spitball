import stepTemplate from '../stepTemplate.vue'

﻿import registrationService from '../../../../services/registrationService'

export default {
    components: {stepTemplate},
    data() {
        return {
            countryCodesList: ['001', '002', '003'],
            codeSent: false,
            username: 'test123'
        }
    },
    methods: {
        next() {
            registrationService.getUserName(this.userName)
                .then(function () {
                    this.$emit('next');
                });
        },
    },
    created() {
        this.username = registrationService.getUserName();
    }
}