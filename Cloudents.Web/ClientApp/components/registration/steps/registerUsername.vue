<template>
    <div>
        <step-template>
            <div slot="step-data">
                <h1>Your Username</h1>
                <p>We gave you a username. Keep it, or create your own, this is how you’ll be known on the
                    marketplace.</p>
                <v-text-field v-model="username"></v-text-field>

                <button class="continue-btn" @click="next">Continue</button>
            </div>
            <img slot="step-image" :src="require(`../img/username.png`)"/>
        </step-template>
    </div>
</template>


<script>
    import {mapGetters, mapActions} from 'vuex'
    import stepTemplate from './stepTemplate.vue'

    export default {
        components:{stepTemplate},
        data() {
            return {
                countryCodesList: ['001', '002', '003'],
                codeSent: false,
                username: ''
            }
        },
        computed: {
            ...mapGetters(['getUserName']),
        },
        methods: {
            ...mapActions(['updateUserName']),
            next() {
                this.updateUserName(this.username);
                this.$emit('next');
            }
        },
        created: function () {
            this.username = this.getUserName
        }
    }
</script>
