export default {
    props: {

    },
    data: function () {
        return {}
    },
    watch: {},
    methods: {
        //close dialog
        requestDialogClose() {
            this.$root.$emit('closePopUp', 'loginPop')
        },
        register(){
            this.$store.dispatch('saveCurrentPathOnPageChange', this.$router);
            this.$router.push({path:'/register'});
        },
        signin(){
            this.$store.dispatch('saveCurrentPathOnPageChange', this.$router);
            this.$router.push({path:'/signin'});
        }
    },
}