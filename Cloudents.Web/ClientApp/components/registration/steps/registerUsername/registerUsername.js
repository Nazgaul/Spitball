import stepTemplate from '../stepTemplate.vue'

﻿import accountService from '../../../../services/accountService'
import disableForm from '../../../mixins/submitDisableMixin'

export default {
    mixins:[disableForm],
    components: {stepTemplate},
    data() {
        return {
            username: '',
            originalUsername:''
        }
    },
    methods: {
        next() {
            this.submitForm();
            var self = this;
            if(this.originalUsername === this.username){
                self.$emit('next');
            }
            else {
                accountService.setUserName(this.username)
                    .then(function () {
                        self.$emit('next');
                    });
            }
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
                self.originalUsername = response.data.name;
            });
    }
}