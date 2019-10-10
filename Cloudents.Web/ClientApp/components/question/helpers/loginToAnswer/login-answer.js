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
            profileData: 'getProfileData'
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
            this.$router.push({path: '/register', query:{returnUrl : this.$route.path}});

        },
        goToSignIn(){
            this.requestDialogClose();
            this.$router.push({path: '/signin', query:{returnUrl : this.$route.path}});

        }
    },

    beforeDestroy(){
        this.updateLoginDialogState(false);
    }
}