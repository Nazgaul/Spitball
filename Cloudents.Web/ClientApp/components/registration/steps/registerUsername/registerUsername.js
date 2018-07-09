import stepTemplate from '../stepTemplate.vue'
import sbInput from "../../../question/helpers/sbInput/sbInput.vue"

﻿import accountService from '../../../../services/accountService'
import disableForm from '../../../mixins/submitDisableMixin'

export default {
    mixins: [disableForm],
    components: {stepTemplate, sbInput},
    data() {
        return {
            username: '',
            originalUsername: '',
            errorMessage: '',
            focus: false,
            editable: false
        }
    },
    methods: {
        next() {
            if(this.submitForm()) {
                var self = this;
                if (this.originalUsername === this.username) {
                    self.$emit('next');
                }
                else {
                    accountService.setUserName(this.username)
                        .then(function () {
                            self.$emit('next');
                        }, function (error) {
                            self.submitForm(false);
                            self.errorMessage = error.response.data ? error.response.data : error.message
                        });
                }
            }
        },
    },
    beforeCreate() {
        var self = this;
        accountService.getUserName()
            .then(function (response) {
                self.username = response.data.name;
                self.originalUsername = response.data.name;
            });
    }
}