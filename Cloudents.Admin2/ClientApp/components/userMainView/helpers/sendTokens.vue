<template>
    <div class="user-tokens-container">
        <div class="user-inputs-container">
            <div><h4 class="text-sm-left text-sm-left text-md-left mb-3">User Id:{{userId}}</h4></div>
            <v-text-field solo class="user-input-text" type="text" v-model.number="tokens"
                          placeholder="Set amount of tokens to apply..."/>
        </div>
        <div class="grant-token-container">
            <v-btn :loading="loading" :disabled="!tokens" color="rgb(0, 188, 212)" @click="sendTokens()">Send</v-btn>
        </div>
    </div>
</template>

<script>
    import { grantTokens } from '../../user/token/tokenUserService';
    import { mapGetters, mapActions } from 'vuex';

    export default {
        data() {
            return {
                tokens: null,
                tokenType: 'Earned',
                types: ['Earned'],
                loading: false
            }
        },
        props: {
            userId: {
                type: Number,
                default: 0
            },
        },
        methods: {
            ...mapActions(['setUserCurrentBalance', 'setTokensDialogState']),
            sendTokens: function () {
                let self = this;
                if (!self.tokens) {
                    self.$toaster.error("you must provide tokens");
                    return;
                }
                self.loading = true;
                grantTokens(self.userId, self.tokens, self.tokenType)
                    .then(() => {
                        self.$toaster.success(`user id ${self.userId} recived ${self.tokens} tokens`);
                        self.setUserCurrentBalance(self.tokens);
                        self.loading = false;
                        self.tokens = null;
                        self.setTokensDialogState(false);
                    }, (err) => {
                        console.log(err);
                        self.loading = false;
                        self.$toaster.error(`Error: couldn't send tokens`)
                    })

            }
        }
    }
</script>

<style scoped>

</style>