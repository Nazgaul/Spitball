import stepTemplate from '../stepTemplate.vue'

﻿import registrationService from '../../../../services/registrationService'

export default {
    components: {stepTemplate},
    data() {
        return {
            countryCodesList: ['001', '002', '003'],
            codeSent: false,
            username: ''
        }
    },
    methods: {
        next() {
            var self = this;
            registrationService.setUserName(this.username)
                .then(function () {
                    self.$emit('next');
                });
        },
        editUsername(){
            var userNameField = this.$el.querySelector('.username-field');
            userNameField.disabled = false;
            userNameField.focus();

        }
    },
    created() {
        // this.username = "woop";
        var self = this;
        registrationService.getUserName()
            .then(function (response) {
                self.username = response.data.name;
            });
    }
}