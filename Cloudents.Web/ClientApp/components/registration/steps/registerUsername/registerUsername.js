import stepTemplate from '../stepTemplate.vue'

﻿import registrationService from '../../../../services/registrationService'

export default {
    components: {stepTemplate},
    data() {
        return {
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
        var self = this;
        registrationService.getUserName()
            .then(function (response) {
                self.username = response.data.name;
            });
    }
}