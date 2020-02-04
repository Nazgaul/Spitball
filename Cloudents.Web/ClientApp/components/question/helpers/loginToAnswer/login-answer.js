import {mapGetters, mapActions} from 'vuex'

export default {
    props: {
    },
    data: function () {
        return {};
    },
computed: {
        ...mapGetters({
            loginDialogState: 'loginDialogState',
        })
    },
    methods: {
        ...mapActions(["updateLoginDialogState"]),
        //close dialog
        requestDialogClose() {
            this.updateLoginDialogState(false);
        },
        goToRegister(){
            this.requestDialogClose();
            this.$router.push({name: 'register', query:{returnUrl : this.$route.path}});
        },
        goToSignIn(){
            this.requestDialogClose();
            this.$router.push({name: 'login', query:{returnUrl : this.$route.path}});
        }
    },

    beforeDestroy(){
        this.updateLoginDialogState(false);
    }
}