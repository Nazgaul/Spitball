import {mapGetters, mapActions} from 'vuex'

export default {
    props: {
    },
    data: function () {
        return {};
    },
    computed: {
        ...mapGetters(['getLoginDialog'])
    },
    methods: {
        ...mapActions(["updateLoginDialog"]),
        //close dialog
        requestDialogClose() {
            this.updateLoginDialog(false);
        },
        goToRegister(){
            this.requestDialogClose();
            this.$router.push({path: '/register', query:{returnUrl : this.$route.path}});

        },
        goToSignIn(){
            this.requestDialogClose();
            this.$router.push({path: '/signin', query:{returnUrl : this.$route.path}});

        }
    },

    beforeDestroy(){
        this.updateLoginDialog(false);
    }
}