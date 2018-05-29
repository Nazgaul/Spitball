import stepTemplate from '../stepTemplate.vue'

﻿import accountService from '../../../../services/accountService'

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
            accountService.setUserName(this.username)
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
        accountService.getUserName()
            .then(function (response) {
                self.username = response.data.name;
            });
    }
}