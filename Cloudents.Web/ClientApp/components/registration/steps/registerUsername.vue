<template>
    <div>
        <div class="form">
            <h1>Your Username</h1>
            <p>We gave you a username. Keep it, or create your own, this is how you’ll be known on the marketplace.</p>
            <v-text-field v-model="username"></v-text-field>
        </div>
        <button @click="next">Continue</button>
    </div>
</template>


<script>
    import {mapGetters, mapActions} from 'vuex'

    export default {
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
            this.username=  this.getUserName
        }
    }
</script>
